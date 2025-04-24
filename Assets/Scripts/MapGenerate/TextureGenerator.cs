using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    /// <summary>
    /// ��Ϊ��ͬ�߶ȷ�����ɫ��������ͼ��ȡ����
    /// </summary>
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;      // ʹ����ģ��
        texture.wrapMode = TextureWrapMode.Clamp;   // �޸������ظ�����
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    /// <summary>
    /// �Ӻڰ������߶�ͼ��ȡ����
    /// </summary>
    public static Texture2D TextureFromHeightMap(HeightMap heightMap)
    {
        // GetLength(0)ȡ��ά�����������GetLength(1)ȡ��ά���������
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
