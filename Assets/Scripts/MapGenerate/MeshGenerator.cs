using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshGenerator
{
    public static (MeshData, MeshData) GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings, int levelOfDetail)
    {
        // ������������������㣬���ٱ����Ķ�������
        int skipIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int numVerticesPerLine = meshSettings.NumVerticesPerLine;

        // Ϊȷ��������ȫ����,��¼��ͼ���ϽǶ���λ��
        Vector2 topLeft = new Vector2(-1, 1) * meshSettings.MeshWorldSize / 2f;

        MeshData terrainMeshData = new(numVerticesPerLine, skipIncrement, meshSettings.useFlatShading);
        MeshData riverMeshData = new(numVerticesPerLine, skipIncrement, meshSettings.useFlatShading);

        // Ϊ��ʹ����齻�紦���߼�����ȷ��������Ҫ�����񶥵�������һȦ�߽綥�㣬���ڼ��㽻�紦���񶥵�ķ���
        int[,] vertexIndicesMap = new int[numVerticesPerLine, numVerticesPerLine];
        int meshVertexIndex = 0;
        int outOfMeshVertexIndex = -1;

        // ��¼�߽��ⶥ���δ�������ı߽綥��
        for (int y = 0; y < numVerticesPerLine; y++)
        {
            for (int x = 0; x < numVerticesPerLine; x++)
            {
                bool isOutOfMeshVertex = y == 0 || y == numVerticesPerLine - 1 || x == 0 || x == numVerticesPerLine - 1;
                bool isSkippedVertex = x > 2 && x < numVerticesPerLine - 3 && y > 2 && y < numVerticesPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);
                if (isOutOfMeshVertex)
                {
                    vertexIndicesMap[x, y] = outOfMeshVertexIndex;
                    outOfMeshVertexIndex--;
                }
                else if (!isSkippedVertex) 
                {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        // �������񶥵�
        for (int y = 0; y < numVerticesPerLine; y++)
        {
            for (int x = 0; x < numVerticesPerLine; x++)
            {
                bool isSkippedVertex = x > 2 && x < numVerticesPerLine - 3 && y > 2 && y < numVerticesPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (!isSkippedVertex)
                {
                    bool isOutOfMeshVertex = y == 0 || y == numVerticesPerLine - 1 || x == 0 || x == numVerticesPerLine - 1;
                    bool isMeshEdgeVertex = (y == 1 || y == numVerticesPerLine - 2 || x == 1 || x == numVerticesPerLine - 2) && !isOutOfMeshVertex;
                    bool isMainVertex = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 && !isOutOfMeshVertex && !isMeshEdgeVertex;
                    bool isEdgeConnectionVertex = (y == 2 || y == numVerticesPerLine - 3 || x == 2 || x == numVerticesPerLine - 3) && !isOutOfMeshVertex && !isMeshEdgeVertex && !isMainVertex;

                    int vertexIndex = vertexIndicesMap[x, y];
                    Vector2 percent = new Vector2(x - 1, y - 1) / (numVerticesPerLine - 3);
                    Vector2 vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y) * meshSettings.MeshWorldSize;
                    float height = heightMap[x, y];

                    // ȷ����Ե���������������Ӵ��߶�һ��
                    if (isEdgeConnectionVertex)
                    {
                        bool isVertical = x == 2 || x == numVerticesPerLine - 3;
                        int dstToMainVertexA = ((isVertical) ? y - 2 : x - 2) % skipIncrement;
                        int dstToMainVertexB = skipIncrement - dstToMainVertexA;
                        float dstPercentFromAToB = dstToMainVertexA / (float)skipIncrement;

                        float heightMainVertexA = heightMap[(isVertical) ? x : x - dstToMainVertexA, (isVertical) ? y - dstToMainVertexA : y];
                        float heightMainVertexB = heightMap[(isVertical) ? x : x + dstToMainVertexB, (isVertical) ? y + dstToMainVertexB : y];

                        height = heightMainVertexA * (1 - dstPercentFromAToB) + heightMainVertexB * dstPercentFromAToB;
                    }
                    // ��������Ӷ���
                    terrainMeshData.AddVertex(new Vector3(vertexPosition2D.x, height, vertexPosition2D.y), percent, vertexIndex);
                    // �ڵ��ݴ����ɹ̶�ˮ��߶ȵĺ������񶥵�
                    riverMeshData.AddVertex(new Vector3(vertexPosition2D.x, TerrainGenerator.WaterSurfaceHeight, vertexPosition2D.y), percent, vertexIndex);
                    
                    bool createTriangle = x < numVerticesPerLine - 1 && y < numVerticesPerLine - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2));

                    // A(x, y)      B(x + i, y)
                    // C(x, y + i)  D(x + i, y + i)
                    // �����ϽǶ���Ϊ��׼���������
                    // ˳ʱ��˳���¼���㲢���������� ADC & DAB
                    if (createTriangle)
                    {
                        int currentIncrement = (isMainVertex && x != numVerticesPerLine - 3 && y != numVerticesPerLine - 3) ? skipIncrement : 1;

                        int a = vertexIndicesMap[x, y];
                        int b = vertexIndicesMap[x + currentIncrement, y];
                        int c = vertexIndicesMap[x, y + currentIncrement];
                        int d = vertexIndicesMap[x + currentIncrement, y + currentIncrement];
                        terrainMeshData.AddTriangle(a, d, c);
                        terrainMeshData.AddTriangle(d, a, b);
                        riverMeshData.AddTriangle(a, d, c);
                        riverMeshData.AddTriangle(d, a, b);
                    }
                }
            }
        }

        terrainMeshData.FinalizeMesh();
        riverMeshData.FinalizeMesh();

        return (terrainMeshData, riverMeshData);
    }

}

/// <summary>
/// ��������
/// </summary>
/// <remarks>
/// <para>
/// 0��1��2<br/>
/// 3��4��5<br/>
/// 7��8��9<br/>
/// ��3*3��С������Ϊ����ÿ�ĸ����㹹��һ�������Σ�һ����������������������ɡ�<br/>
/// 3 * 3 = <see langword="meshWidth"/> * <see langword="meshHeight"/> = 9 <see cref="vertices"/>��<br/>
/// 2 * 2 * 6 = (<see langword="meshWidth"/> - 1) * (<see langword="meshHeight"/> - 1) * 6 <see cref="triangles"/>��<br/>
/// </para> 
/// </remarks>
public class MeshData
{
    private Vector3[] vertices;
    private int[] triangles;     // ÿ���������һ��������
    private Vector2[] uvs;       // UV���ǽ�ͼ����ÿһ���㾫ȷ��Ӧ��ģ������ı��棬�ڵ����֮��ļ�϶λ�����������ͼ��⻬��ֵ����
    private Vector3[] bakedNormals;

    private Vector3[] outOfMeshVertices;
    int[] outOfMeshTriangles;

    int triangleIndex;
    int outOfMeshTriangleIndex;

    bool useFlatShading;

    public MeshData(int numVerticesPerLine, int skipIncrement, bool useFlatShading)
    {
        this.useFlatShading = useFlatShading;

        int numMeshEdgeVertices = (numVerticesPerLine - 2) * 4 - 4;
        int numEdgeConnectionVertices = (skipIncrement - 1) * (numVerticesPerLine - 5) / skipIncrement * 4;
        int numMainVerticesPerLine = (numVerticesPerLine - 5) / skipIncrement + 1;
        int numMainVertices = numMainVerticesPerLine * numMainVerticesPerLine;

        vertices = new Vector3[numMeshEdgeVertices + numEdgeConnectionVertices  + numMainVertices];
        uvs = new Vector2[vertices.Length];

        int numMeshEdgeTriangles = (numVerticesPerLine - 4) * 8;
        int numMainTriangles = (numMainVerticesPerLine - 1) * (numMainVerticesPerLine - 1) * 2;
        triangles = new int[(numMeshEdgeTriangles + numMainTriangles) * 3];

        outOfMeshVertices = new Vector3[numVerticesPerLine * 4 - 4];
        outOfMeshTriangles = new int[(numVerticesPerLine - 2) * 24];
    }
    
    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        // �߽綥��
        if(vertexIndex < 0)
        {
            outOfMeshVertices[-vertexIndex - 1] = vertexPosition;
        }
        else
        {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = new Vector2(0, vertexPosition.y);
        }
    }

    public void AddTriangle(int a, int b, int c)
    {
        // �������б߽綥��
        if(a < 0|| b < 0 || c < 0)
        {
            outOfMeshTriangles[outOfMeshTriangleIndex] = a;
            outOfMeshTriangles[outOfMeshTriangleIndex + 1] = b;
            outOfMeshTriangles[outOfMeshTriangleIndex + 2] = c;
            outOfMeshTriangleIndex += 3;
        }
        else
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
    }

    /// <summary>
    /// ���㷨��
    /// </summary>
    private Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        // �������������ǵķ���
        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        // ����߽����ǵķ��ߣ�ȷ����������Ե������ʱ��֤���ߵ�һ����
        int borderTriangleCount = outOfMeshTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = outOfMeshTriangles[normalTriangleIndex];
            int vertexIndexB = outOfMeshTriangles[normalTriangleIndex + 1];
            int vertexIndexC = outOfMeshTriangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            if(vertexIndexA >= 0)
            {
                vertexNormals[vertexIndexA] += triangleNormal;
            }
            if (vertexIndexB >= 0)
            {
                vertexNormals[vertexIndexB] += triangleNormal;
            }
            if (vertexIndexC >= 0)
            {
                vertexNormals[vertexIndexC] += triangleNormal;
            }
        }

        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }

    /// <summary>
    /// ��������ȡ���淨��
    /// </summary>
    Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = (indexA < 0) ? outOfMeshVertices[-indexA - 1] : vertices[indexA];
        Vector3 pointB = (indexB < 0) ? outOfMeshVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = (indexC < 0) ? outOfMeshVertices[-indexC - 1] : vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public void FinalizeMesh()
    {
        // ʹ��ƽ����ɫʱ����ȷ������һ���ԣ���ʹ�����õļ��㷨�߷���
        if (useFlatShading)
        {
            FlatShading();
        }
        else
        {
            BakeNormals();
        }
    }
    
    /// <summary>
    /// ���෨��
    /// </summary>
    private void BakeNormals() 
    {
        bakedNormals = CalculateNormals();
    }

    /// <summary>
    /// ƽ����ɫ
    /// </summary>
    private void FlatShading()
    {
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUvs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            // ���䶥���uv����ֵ
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];
            // ���·������������ֵ���ⶥ�㸴��
            // ʹ0��1��2��������һ��3��4��5�������Ƕ�
            triangles[i] = i;
        }

        vertices = flatShadedVertices;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new()
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };
        if (useFlatShading){
            mesh.RecalculateNormals();
        }
        else
        {
            mesh.normals = bakedNormals;
        }
        return mesh;
    }
}
