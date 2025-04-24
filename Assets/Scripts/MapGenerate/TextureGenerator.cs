using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    /// <summary>
    /// 从为不同高度分配颜色的噪声地图获取纹理
    /// </summary>
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;      // 使纹理不模糊
        texture.wrapMode = TextureWrapMode.Clamp;   // 修复纹理重复问题
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    /// <summary>
    /// 从黑白噪声高度图获取纹理
    /// </summary>
    public static Texture2D TextureFromHeightMap(HeightMap heightMap)
    {
        // GetLength(0)取二维数组的行数，GetLength(1)取二维数组的列数
        int width = heightMap.Values.GetLength(0);
        int height = heightMap.Values.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heightMap.MinValue, heightMap.MaxValue, heightMap.Values[x, y]));
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }
}
