using Acetix.Grass;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
    // �۲������ͼ��������ֵ�Ż������ײ��
    const float colliderGenerationDistanceThreshold = 5;
    const float resourceSpawnDistanceThreshold = 25;

    public Transform transform;
    public Vector2 coord;
    public Vector2 sampleCenter;
    private Bounds bounds;
    private Transform viewer;
    private float maxViewDst;

    // ��ͼ���������
    private GameObject meshObject;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    // �����������
    private GameObject riverMeshObject;
    private MeshRenderer riverMeshRenderer;
    private MeshFilter riverMeshFilter;

    // LOD��Ϣ
    private LODInfo[] detailLevels;
    private LODMesh[] lodMeshes;
    private int colliderLODIndex;
    private int previousLODIndex = -1;

    public event Action<TerrainChunk, bool> OnVisibilityChanged;
    private bool heightMapReceived, humidityMapReceived, temperatureMapReceived;
    private bool hasSetCollider;

    // ���ɵ������� settings
    private HeightMapSettings[] heightMapSettings;
    public MeshSettings MeshSettings { get; private set; }
    private BiomeSettings biomeSettings;
    public HeightMap heightMapData, humidityMapData, temperatureMapData;
    public BiomeType[,] BiomeMap { get; private set; }

    // �жϸõ�ͼ���Ƿ�Ϊ��������
    private bool isFirstCreateChunk = true;
    public bool isTown = false;

    public TerrainChunkData terrainData; 
    private Texture terrainTexture;

    public TerrainChunk(
        Vector2 coord, 
        HeightMapSettings[] heightMapSettings, 
        MeshSettings meshSettings, 
        BiomeSettings biomeSettings,
        LODInfo[] detailLevels, 
        int colliderLODIndex, 
        Transform parent, 
        Transform viewer, 
        Material terrainMaterial, 
        Material riverMaterial)
    {
        this.coord = coord;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.MeshSettings = meshSettings;
        this.biomeSettings = biomeSettings;
        this.viewer = viewer;
        terrainTexture = terrainMaterial.GetTexture("_Grass_Texture");
        terrainData = new();

        sampleCenter = coord * meshSettings.MeshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.MeshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.MeshWorldSize);

        // ���ɵ�ͼ������������������������趨����
        meshObject = new GameObject("Terrian Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = terrainMaterial;
        // �趨��������λ���븸�����趨���ɼ�
        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;

        transform = meshObject.transform;

        //meshObject.AddComponent<GPUGrassTest>().enabled = false;

        // ���ɺ�������������������������趨����
        riverMeshObject = new GameObject("River");
        riverMeshRenderer = riverMeshObject.AddComponent<MeshRenderer>();
        riverMeshFilter = riverMeshObject.AddComponent<MeshFilter>();
        riverMeshRenderer.material = riverMaterial;
        // �趨��������λ���븸�����趨���ɼ�
        riverMeshObject.transform.position = new Vector3(position.x, 0, position.y);
        riverMeshObject.transform.parent = meshObject.transform;

        SetVisible(false);

        meshObject.layer = LayerMask.NameToLayer("Ground");

        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            lodMeshes[i].UpdateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex)
            {
                lodMeshes[i].UpdateCallback += UpdateCollisionMesh;
            }
        }


        // ^ ����Ϊ��������� ������������������
        maxViewDst = detailLevels[^1].visibleDstThreshold;
    }

    private Vector2 ViewerPosition
    {
        get
        {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }

    // �������� �����̳߳��е��߳̽��е�ͼ���ݵĴ���
    public void Load()
    {
        ThreadDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(MeshSettings.NumVerticesPerLine, MeshSettings.NumVerticesPerLine, heightMapSettings[0], sampleCenter), OnHeightMapReceived);
        ThreadDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(MeshSettings.NumVerticesPerLine, MeshSettings.NumVerticesPerLine, heightMapSettings[1], sampleCenter), OnHumidityMapReceived);
        ThreadDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(MeshSettings.NumVerticesPerLine, MeshSettings.NumVerticesPerLine, heightMapSettings[2], sampleCenter), OnTemperatureMapReceived);
    }

    private void OnHeightMapReceived(object heightMapObject)
    {
        heightMapData = (HeightMap)heightMapObject;
        heightMapReceived = true;
        terrainData.SetUp(
            new Vector3(MeshSettings.MeshWorldSize, heightMapData.MaxValue, MeshSettings.MeshWorldSize),
            MeshSettings.NumVerticesPerLine,
            GenerateHeightmapTexture(heightMapData.Values),
            terrainTexture);

        UpdateTerrainChunk();
    }
    private void OnHumidityMapReceived(object heightMapObject)
    {
        humidityMapData = (HeightMap)heightMapObject;
        humidityMapReceived = true;

        UpdateTerrainChunk();
    }
    private void OnTemperatureMapReceived(object heightMapObject)
    {
        temperatureMapData = (HeightMap)heightMapObject;
        temperatureMapReceived = true; 
        
        UpdateTerrainChunk();
    }

    private void GenerateBiomeMap()
    {
        var plainCount = 0;
        BiomeMap = new BiomeType[MeshSettings.NumVerticesPerLine, MeshSettings.NumVerticesPerLine];
        for (int y = 0; y < MeshSettings.NumVerticesPerLine; y++)
        {
            for (int x = 0; x < MeshSettings.NumVerticesPerLine; x++)
            {
                BiomeType matchedBiome = biomeSettings.MatchBiome(heightMapData.Values[x, y], humidityMapData.Values[x, y], temperatureMapData.Values[x, y]);
                BiomeMap[x, y] = matchedBiome;
                if(matchedBiome.biomeType == "ƽԭ")
                {
                    plainCount++;
                }
            }
        }

        var plainRatio = plainCount / (MeshSettings.NumVerticesPerLine * MeshSettings.NumVerticesPerLine);
        isTown = plainRatio > TerrainGenerator.PlainRatioForTownCenter;
    }

    public Texture2D GenerateHeightmapTexture(float[,] heightMapData)
    {
        int width = heightMapData.GetLength(0);
        int height = heightMapData.GetLength(1);

        Texture2D tex = new Texture2D(width, height, TextureFormat.RFloat, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        Color[] colors = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float v = Mathf.Clamp01(heightMapData[x, y]);
                colors[y * width + x] = new Color(v, v, v, 1f); // �Ҷ�ͼ
            }
        }

        tex.SetPixels(colors);
        tex.Apply();

        return tex;
    }

    public void UpdateTerrainChunk()
    {
        if (heightMapReceived && humidityMapReceived && temperatureMapReceived)
        {        
            // Bounds.SqrDistance ���ظõ���ð�Χ��֮�����Сƽ������
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(ViewerPosition));

            bool wasVisible = IsVisible();
            bool visible = viewerDstFromNearestEdge <= maxViewDst;

            if (visible)
            {
                float sqrDstFromViewerToEdge = bounds.SqrDistance(ViewerPosition);

                // ������̬����ͼ
                GenerateBiomeMap();
                if (isFirstCreateChunk) //&& sqrDstFromViewerToEdge < resourceSpawnDistanceThreshold * resourceSpawnDistanceThreshold)
                {
                    // �õ�ͼ��Ϊ�������� �����ڴ˴����ɸõ�ͼ��Ļ�����Դ - ��ľ��
                    PoolManager.Instance.treeFactory.Spawn(this);
                    //meshObject.AddComponent<GrassInstancerController>()._terrain = this;
                    //PoolManager.Instance.grassFactory.Spawn(this);
                    if (isTown)
                    {
                        PoolManager.Instance.buildingFactory.Spawn(this);
                    }
                    isFirstCreateChunk = false;
                }

                int lodIndex = 0;

                // ƥ��ϸ�ڵȼ�
                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                // ����ϸ�ڵȼ�
                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.terrainMesh;
                        // meshObject.GetComponent<GPUGrassTest>().enabled = true;
                        // meshObject.GetComponent<GPUGrassTest>()._terrianMesh = meshFilter.mesh;
                        if (!isTown)
                        {
                            riverMeshFilter.mesh = lodMesh.riverMesh;
                        }
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(heightMapData, MeshSettings);
                    }
                }
            }

            // �Ƿ�ɼ� �����仯
            if (wasVisible != visible)
            {
                SetVisible(visible);

                OnVisibilityChanged?.Invoke(this, visible);
            }

        }
    }

    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            float sqrDstFromViewerToEdge = bounds.SqrDistance(ViewerPosition);

            if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].SqrVisibleDstThreshold)
            {
                if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
                {
                    lodMeshes[colliderLODIndex].RequestMesh(heightMapData, MeshSettings);
                }
            }

            if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
            {
                if (lodMeshes[colliderLODIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[colliderLODIndex].terrainMesh;
                    hasSetCollider = true;
                }
            }
        }
    }

    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return meshObject.activeSelf;
    }


    private class LODMesh
    {
        public Mesh terrainMesh;
        public Mesh riverMesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        private int lod;    // The level of detail about this mesh.
        public event Action UpdateCallback;

        public LODMesh(int lod)
        {
            this.lod = lod;
        }

        private void OnMeshDataReceived(object meshDataObject)
        {
            (MeshData mesh1, MeshData mesh2) = ((MeshData,MeshData))meshDataObject;
            terrainMesh = mesh1.CreatMesh();
            riverMesh = mesh2.CreatMesh();

            hasMesh = true;

            UpdateCallback();
        }

        public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
        {
            hasRequestedMesh = true;
            ThreadDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.Values, meshSettings, lod), OnMeshDataReceived);
        }
    }
}
