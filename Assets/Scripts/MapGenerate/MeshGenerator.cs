using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshGenerator
{
    public static (MeshData, MeshData) GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings, int levelOfDetail)
    {
        // 网格简化增量，跳过顶点，减少遍历的顶点数量
        int skipIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int numVerticesPerLine = meshSettings.NumVerticesPerLine;

        // 为确保网格完全居中,记录地图左上角顶点位置
        Vector2 topLeft = new Vector2(-1, 1) * meshSettings.MeshWorldSize / 2f;

        MeshData terrainMeshData = new(numVerticesPerLine, skipIncrement, meshSettings.useFlatShading);
        MeshData riverMeshData = new(numVerticesPerLine, skipIncrement, meshSettings.useFlatShading);

        // 为了使网格块交界处法线计算正确，我们需要在网格顶点外增加一圈边界顶点，用于计算交界处网格顶点的法线
        int[,] vertexIndicesMap = new int[numVerticesPerLine, numVerticesPerLine];
        int meshVertexIndex = 0;
        int outOfMeshVertexIndex = -1;

        // 记录边界外顶点和未被跳过的边界顶点
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

        // 生成网格顶点
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

                    // 确保边缘顶点与主顶点连接处高度一致
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
                    // 给网格添加顶点
                    terrainMeshData.AddVertex(new Vector3(vertexPosition2D.x, height, vertexPosition2D.y), percent, vertexIndex);
                    // 在低洼处生成固定水面高度的河流网格顶点
                    riverMeshData.AddVertex(new Vector3(vertexPosition2D.x, TerrainGenerator.WaterSurfaceHeight, vertexPosition2D.y), percent, vertexIndex);
                    
                    bool createTriangle = x < numVerticesPerLine - 1 && y < numVerticesPerLine - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2));

                    // A(x, y)      B(x + i, y)
                    // C(x, y + i)  D(x + i, y + i)
                    // 以左上角顶点为基准添加三角形
                    // 顺时针顺序记录顶点并构建三角形 ADC & DAB
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
/// 网格数据
/// </summary>
/// <remarks>
/// <para>
/// 0—1—2<br/>
/// 3—4—5<br/>
/// 7—8—9<br/>
/// 以3*3大小的网格为例，每四个顶点构成一个正方形，一个正方形由两个三角形组成。<br/>
/// 3 * 3 = <see langword="meshWidth"/> * <see langword="meshHeight"/> = 9 <see cref="vertices"/>，<br/>
/// 2 * 2 * 6 = (<see langword="meshWidth"/> - 1) * (<see langword="meshHeight"/> - 1) * 6 <see cref="triangles"/>。<br/>
/// </para> 
/// </remarks>
public class MeshData
{
    private Vector3[] vertices;
    private int[] triangles;     // 每三个顶点记一个三角形
    private Vector2[] uvs;       // UV就是将图像上每一个点精确对应到模型物体的表面，在点与点之间的间隙位置由软件进行图像光滑插值处理。
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
        // 边界顶点
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
        // 顶点中有边界顶点
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
    /// 计算法线
    /// </summary>
    private Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        // 计算网格内三角的法线
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

        // 计算边界三角的法线，确保在网格块边缘共享顶点时保证法线的一致性
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
    /// 从索引获取表面法线
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
        // 使用平面着色时无需确保法线一致性，可使用内置的计算法线方法
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
    /// 烘培法线
    /// </summary>
    private void BakeNormals() 
    {
        bakedNormals = CalculateNormals();
    }

    /// <summary>
    /// 平面着色
    /// </summary>
    private void FlatShading()
    {
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUvs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            // 分配顶点和uv坐标值
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];
            // 重新分配三角数组的值避免顶点复用
            // 使0、1、2代表三角一，3、4、5代表三角二
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
