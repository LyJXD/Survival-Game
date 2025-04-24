using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunkData : MonoBehaviour
{
    public Vector3 size;

    public int heightmapResolution;

    public Texture heightmapTexture;

    public Texture terrainTexture;

    public void SetUp(Vector3 size, int heightmapResolution, Texture heightmapTexture, Texture terrainTexture)
    {
        this.size = size;
        this.heightmapResolution = heightmapResolution;
        this.heightmapTexture = heightmapTexture;
        this.terrainTexture = terrainTexture;
    }
}
