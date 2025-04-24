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
    /// ��ϸ�ڼ������0ʱ��Ⱦ������ÿ�еĶ�����
    /// ���������������������������У������ڼ����Ե���ߵĶ��ⶥ��
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
