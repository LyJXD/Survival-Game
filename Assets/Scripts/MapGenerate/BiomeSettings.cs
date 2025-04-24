using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Generate/BiomeMap Settings")]
public class BiomeSettings : UpdatableData
{
    public BiomeType[] biomeTypes;

    // 根据高度、湿度、温度匹配生态区域
    public BiomeType MatchBiome(float height, float humidity, float temperature)
    {
        foreach (BiomeType biome in biomeTypes)
        {
            if (height >= biome.minHeight && height <= biome.maxHeight &&
                humidity >= biome.minHumidity && humidity <= biome.maxHumidity &&
                temperature >= biome.minTemperature && temperature <= biome.maxTemperature)
            {
                return biome;
            }
        }

        return biomeTypes[0]; // fallback
    }
}

[System.Serializable]
public struct BiomeType
{
    public string biomeType;

    // 资源生成概率
    public float treeSpawnChance;
    public float stoneSpawnChance;

    // 高度范围
    public float minHeight;
    public float maxHeight;

    // 湿度范围
    [Range(0, 100)]
    public float minHumidity;
    [Range(0, 100)]
    public float maxHumidity;

    // 温度范围
    [Range(0,100)]
    public float minTemperature;
    [Range(0, 100)]
    public float maxTemperature;
}
