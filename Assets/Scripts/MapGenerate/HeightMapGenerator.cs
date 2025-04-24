using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCenter)
    {
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCenter);

        AnimationCurve heightCurve_threadSafe = new(settings.heightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                values[i, j] *= heightCurve_threadSafe.Evaluate(values[i, j]) * settings.heightMultiplier;

                if (values[i, j] < minValue)
                {
                    minValue = values[i, j];
                }
                if (values[i, j] > maxValue)
                {
                    maxValue = values[i, j];
                }
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }
}

public readonly struct HeightMap
{
    public float[,] Values { get; }
    public float MinValue { get; }
    public float MaxValue { get; }

    public HeightMap(float[,] values, float minValue, float maxValue)
    {
        this.Values = values;
        this.MinValue = minValue;
        this.MaxValue = maxValue;
    }
}