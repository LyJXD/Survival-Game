using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TerrainGenerator : MonoBehaviour
{
    // �۲����ƶ���������ֵ����Ż���µ�ͼ��
    private const float viewerMoveThresholdForChunkUpdate = 25f;
    private const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    // ���ɵ������� settings
    [Header("Terrain Settings")]
    public MeshSettings meshSettings;
    public HeightMapSettings[] heightMapSettings;
    public TextureData textureSettings;
    public BiomeSettings biomeSettings;

    // �۲��ߡ������
    [Header("Player")]
    public Transform viewer;
    private Vector2 viewerPosition;
    private Vector2 viewerPositionOld;

    // Ϊ��ײ���趨ϸ�ڵȼ�
    [Header("LOD Settings")]
    public int colliderLODIndex;
    public LODInfo[] detailLevels;

    [Header("Global Variables")]
    // �趨ˮ��߶�
    public static float WaterSurfaceHeight = 2f;
    // ���ܳ�Ϊ��������ʱ�õ�ͼ���ƽԭ����
    public static float PlainRatioForTownCenter = .9f;
    // ���ܳ�Ϊ����ʱ�õ�ͼ���ƽԭ����
    public static float PlainRatioForTown = .75f;
    // ��ͼ���������ɳ������ĵ���Ŀ
    public static float TownNumRemain = 7;

    // �����ͼ���ĵ������ɳ�����������
    private float minCoordDstFromTownToCenter;
    // �������ɳ������С������
    private float minCoordDstFromTownToTown;
    private float meshWorldSize;
    // �趨ÿ���������ɼ���ͼ������
    private int chunkVisibleInViewDst;
    [Header("Materials")]
    public Material terrainMaterial;
    public Material riverMaterial;

    // ��ͼ���ֵ� �洢�������ͼ���ֵ��
    private static Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new();
    // �洢�ɼ��ĵ�ͼ��
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

        // Vector2.sqrMagnitude ���ظ�������ƽ������,���ڱȽ�������С��������п������㡣
        // Sqrt�����൱���ӣ�ִ��ʱ�����ͨ��������Ҫ����ʹ��magnitude����Ҫ��öࡣ
        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    /// <summary>
    /// ���ݹ۲�������λ�ã����¿ɼ���ͼ��
    /// </summary>
    private void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        // �������ĵĵ�ͼ�����궨��Ϊ��0��0������¼viewer���ڵ�����λ��
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for (int yOffset = -chunkVisibleInViewDst; yOffset <= chunkVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunkVisibleInViewDst; xOffset <= chunkVisibleInViewDst; xOffset++)
            {
                // �ɼ��ĵ�ͼ��
                Vector2 viewedChunkCoord = new(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                // ���Ѿ����µĵ�ͼ���в������õ�ͼ��
                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    // ����õ�ͼ���Ѿ�����ӵ���ͼ���ֵ�����ֱ�Ӹ���
                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    // ����õ�ͼ��û����ӹ������´���һ����ͼ�鲢��ӽ��ֵ�
                    else
                    {
                        TerrainChunk newChunk = new(viewedChunkCoord, heightMapSettings, meshSettings, biomeSettings, detailLevels, colliderLODIndex, transform, viewer, terrainMaterial, riverMaterial);
                        terrainChunkDictionary.Add(viewedChunkCoord, newChunk);

                        // �жϵ�ǰ��ͼ���ܷ����ɳ���
                        if (viewedChunkCoord.x > minCoordDstFromTownToTown || viewedChunkCoord.y > minCoordDstFromTownToTown)
                        {
                            if (townCenterDictionary != null && FindNearestTown(viewedChunkCoord) > minCoordDstFromTownToCenter)
                            {
                                newChunk.isTown = true;
                            }
                        }


                        // Ϊ�¼���Ӻ��� ����ͼ���Ƿ�ɼ�״̬�ı�ʱ
                        newChunk.OnVisibilityChanged += OnTerrainChunkVisbilityChanged;
                        newChunk.Load();

                        // ��¼����λ��
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

    // �жϸ����������Ƿ��г���
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

    // Ѱ���뵱ǰ�����������ĳ���
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
    public float visibleDstThreshold;   // �ɼ�������ֵ����ʾ��ϸ�ڲ���ڶ�Զ��������Ч

    public float SqrVisibleDstThreshold
    {
        get
        {
            return visibleDstThreshold * visibleDstThreshold;
        }
    }
}
