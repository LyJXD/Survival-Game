using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Generate/BiomeMap Settings")]
public class BiomeSettings : UpdatableData
{
    public BiomeType[] biomeTypes;

    // ���ݸ߶ȡ�ʪ�ȡ��¶�ƥ����̬����
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

    // ��Դ���ɸ���
    public float treeSpawnChance;
    public float stoneSpawnChance;

    // �߶ȷ�Χ
    public float minHeight;
    public float maxHeight;

    // ʪ�ȷ�Χ
    [Range(0, 100)]
    public float minHumidity;
    [Range(0, 100)]
    public float maxHumidity;

    // �¶ȷ�Χ
    [Range(0,100)]
    public float minTemperature;
    [Range(0, 100)]
    public float maxTemperature;
}
