using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TerrainGenerator : MonoBehaviour
{
    // 观察者移动超过该阈值距离才会更新地图块
    private const float viewerMoveThresholdForChunkUpdate = 25f;
    private const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    // 生成地形所需 settings
    [Header("Terrain Settings")]
    public MeshSettings meshSettings;
    public HeightMapSettings[] heightMapSettings;
    public TextureData textureSettings;
    public BiomeSettings biomeSettings;

    // 观察者——玩家
    [Header("Player")]
    public Transform viewer;
    private Vector2 viewerPosition;
    private Vector2 viewerPositionOld;

    // 为碰撞体设定细节等级
    [Header("LOD Settings")]
    public int colliderLODIndex;
    public LODInfo[] detailLevels;

    [Header("Global Variables")]
    // 设定水面高度
    public static float WaterSurfaceHeight = 2f;
    // 可能成为城镇中心时该地图块的平原比例
    public static float PlainRatioForTownCenter = .9f;
    // 可能成为城镇时该地图块的平原比例
    public static float PlainRatioForTown = .75f;
    // 地图还可以生成城镇中心的数目
    public static float TownNumRemain = 7;

    // 距离地图中心的能生成城镇的最近坐标
    private float minCoordDstFromTownToCenter;
    // 可以生成城镇的最小坐标间隔
    private float minCoordDstFromTownToTown;
    private float meshWorldSize;
    // 设定每个方向最大可见地图块数量
    private int chunkVisibleInViewDst;
    [Header("Materials")]
    public Material terrainMaterial;
    public Material riverMaterial;

    // 地图块字典 存储坐标与地图块键值对
    private static Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new();
    // 存储可见的地图块
    private List<TerrainChunk> visibleTerrainChunks = new();

    public static Dictionary<Vector2, TerrainChunk> TerrainChunkDictionary => terrainChunkDictionary;
    public Dictionary<Vector2, TerrainChunk> townCenterDictionary = new();

    private void Start()
    {
        //textureSettings.ApplyToMaterial(mapMaterial);
        textureSettings.UpdateMeshHeights(terrainMaterial, heightMapSettings[0].MinHeight, heightMapSettings[0].MaxHeight);

        float maxViewDst = detailLevels[^1].visibleDstThreshold;
        meshWorldSize = meshSettings.MeshWorldSize;
        chunkVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);
        minCoordDstFromTownToCenter = 3 * chunkVisibleInViewDst;
        minCoordDstFromTownToTown = 5 * chunkVisibleInViewDst;

        UpdateVisibleChunks();
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if (viewerPosition != viewerPositionOld)
        {
            foreach (var chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        // Vector2.sqrMagnitude 返回该向量的平方长度,用于比较向量大小而无需进行开方运算。
        // Sqrt计算相当复杂，执行时间比普通算术运算要长。使用magnitude属性要快得多。
        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    /// <summary>
    /// 根据观察者所在位置，更新可见地图块
    /// </summary>
    private void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        // 将最中心的地图块坐标定义为（0，0），记录viewer所在的坐标位置
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for (int yOffset = -chunkVisibleInViewDst; yOffset <= chunkVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunkVisibleInViewDst; xOffset <= chunkVisibleInViewDst; xOffset++)
            {
                // 可见的地图块
                Vector2 viewedChunkCoord = new(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                // 当已经更新的地图块中不包含该地图块
                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    // 如果该地图块已经被添加到地图块字典中则直接更新
                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    // 如果该地图块没有添加过，则新创建一个地图块并添加进字典
                    else
                    {
                        TerrainChunk newChunk = new(viewedChunkCoord, heightMapSettings, meshSettings, biomeSettings, detailLevels, colliderLODIndex, transform, viewer, terrainMaterial, riverMaterial);
                        terrainChunkDictionary.Add(viewedChunkCoord, newChunk);

                        // 判断当前地图块能否生成城镇
                        if (viewedChunkCoord.x > minCoordDstFromTownToTown || viewedChunkCoord.y > minCoordDstFromTownToTown)
                        {
                            if (townCenterDictionary != null && FindNearestTown(viewedChunkCoord) > minCoordDstFromTownToCenter)
                            {
                                newChunk.isTown = true;
                            }
                        }


                        // 为事件添加函数 当地图块是否可见状态改变时
                        newChunk.OnVisibilityChanged += OnTerrainChunkVisbilityChanged;
                        newChunk.Load();

                        // 记录城镇位置
                        if (newChunk.isTown && TownNumRemain > 0)
                        {
                            townCenterDictionary.Add(viewedChunkCoord, newChunk);
                            TownNumRemain--;
                        }
                    }
                }
            }
        }
    }

    // 判断该坐标四周是否有城镇
    private bool IsNearTown(Vector2 currentCoord)
    {
        if (townCenterDictionary.ContainsKey(new Vector2(currentCoord.x, currentCoord.y + 1)) ||    // North
            townCenterDictionary.ContainsKey(new Vector2(currentCoord.x, currentCoord.y - 1)) ||    // Sourth
            townCenterDictionary.ContainsKey(new Vector2(currentCoord.x + 1, currentCoord.y)) ||    // West
            townCenterDictionary.ContainsKey(new Vector2(currentCoord.x - 1, currentCoord.y)))      // East
        {
            return true;
        }
            
        return false;
        
    }

    // 寻找与当前城镇距离最近的城镇
    private float FindNearestTown(Vector2 currentCoord)
    {
        float minValue = int.MaxValue;
        foreach (var town in townCenterDictionary)
        {
            if(Vector2.Distance(currentCoord, town.Key) < minValue)
            {
                minValue = Vector2.Distance(currentCoord, town.Key);
            }
        }

        return minValue;
    }

    private void OnTerrainChunkVisbilityChanged(TerrainChunk chunk, bool isVisible) 
    {
        if (isVisible)
        {
            visibleTerrainChunks.Add(chunk);
        }
        else
        {
            visibleTerrainChunks.Remove(chunk);
        }
    }


}

[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int lod;
    public float visibleDstThreshold;   // 可见距离阈值，表示该细节层次在多远距离内有效

    public float SqrVisibleDstThreshold
    {
        get
        {
            return visibleDstThreshold * visibleDstThreshold;
        }
    }
}
