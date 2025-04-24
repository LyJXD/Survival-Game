using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Generate/Mesh Settings")]
public class MeshSettings : UpdatableData
{
    public const int numSupportedLODs = 5;
    public const int numSupportedFlatShadedChunkSizes = 3;
    public const int numSupportedChunkSizes = 9;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

    public float meshScale = 1f;

    public bool useFlatShading;

    [Range(0, numSupportedFlatShadedChunkSizes - 1)]
    public int flatShadedChunkSizeIndex;
    [Range(0, numSupportedChunkSizes - 1)]
    public int ChunkSizeIndex;

    /// <summary>
    /// 在细节级别等于0时渲染的网格每行的顶点数
    /// 包括了两个不包含在最终网格中，但用于计算边缘法线的额外顶点
    /// </summary>
    public int NumVerticesPerLine
    {
        get
        {
            return supportedChunkSizes[(useFlatShading) ? flatShadedChunkSizeIndex : ChunkSizeIndex] + 5;
        }
    }

    public float MeshWorldSize
    {
        get
        {
            return (NumVerticesPerLine - 3) * meshScale;
        }
    }
}
