using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Noise
{
    /// <summary>
    /// 归一化模式，局部模式和全局模式
    /// </summary>
    public enum NormalizeMode
    {
        Local,
        Global
    };

    /// <summary>
    /// Generate Perlin-Noise Map.
    /// </summary>
    /// <param name="octaves">分形噪声由不同频率的多个样本组成，这些被称为倍频。</param>
    /// <param name="persistence">定义了各层噪声的振幅衰减</param>
    /// <param name="lacunarity">间隙度参数，决定了频率的间隔</param>
    /// <param name="offset">自定义偏移值用于滚动浏览噪声</param>
    /// <returns></returns>
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCenter)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        float amplitude = 1;    // 振幅
        float frequency = 1;    // 频率

        // pseudorandom number generator 伪随机数生成
        System.Random prng = new System.Random(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];
        float maxPossibleHeight = 0;
        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCenter.x;
            float offsetY = prng.Next(-100000, 100000) - settings.offset.y - sampleCenter.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistence;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f; 
        float halfHeight = mapHeight / 2f;

        for(int y= 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++) 
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;  // 噪声高度值，将每一度的振幅相加来获得

                for (int i= 0; i < settings.octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;    // * 2 - 1 使其范围在 -1到 1
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistence;   // 振幅在每个循环后减小
                    frequency *= settings.lacunarity;    // 频率在每个循环后增大
                }

                if(noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x,y] = noiseHeight;

                if (settings.normalizeMode == NormalizeMode.Global)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }


        if (settings.normalizeMode == NormalizeMode.Local)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    // 归一化
                    // Mathf.InverseLerp计算在两个值之间的插值。
                    // 这个函数接受三个参数：a（起点值），b（终点值），以及 value（当前值）。
                    // 它返回一个浮点数，表示当前值在起点和终点之间的相对位置。
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
            }
        }

        return noiseMap;
    }
}

[System.Serializable]
public class NoiseSettings
{
    public Noise.NormalizeMode normalizeMode;

    public float scale = 50;

    public int octaves = 6;
    [Range(0, 1)]
    public float persistence = .6f;
    public float lacunarity = 2;

    public int seed;
    public Vector2 offset;

    public void ValidateValues()
    {
        seed = Random.Range(0, short.MaxValue);
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistence = Mathf.Clamp01(persistence);
    }
}