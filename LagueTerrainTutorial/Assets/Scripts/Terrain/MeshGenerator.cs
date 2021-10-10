using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {

    public static MeshData GenerateMeshTerrain(TerrainData terrain, int levelOfUndetail, Func<float, float> noiseMapper) {
        int width = terrain.GetWidth();
        int height = terrain.GetHeight();
        int halfWidth = width / 2;
        int halfHeight = height / 2;

        int simplificationIncrement = Math.Max(1, levelOfUndetail * 2);
        int verticesPerLine = (width - 1) / simplificationIncrement + 1;

        MeshData meshData = MeshData.From(width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y += simplificationIncrement) {
            for (int x = 0; x < width; x += simplificationIncrement) {

                float centeredX = x - halfWidth;
                float centeredY = -y + halfHeight;
                meshData.Vertices[vertexIndex] = new Vector3(centeredX, noiseMapper(terrain.GetHeightAtPoint(x, y)), centeredY);
                meshData.Uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (IsNotVertexOnRightOrBottom(width, height, x, y)) {
                    int downRightVertexIndex = vertexIndex + verticesPerLine + 1;
                    int downVertexIndex = vertexIndex + verticesPerLine;
                    int rightVertexIndex = vertexIndex + 1;

                    meshData.AddTriangle(vertexIndex, downRightVertexIndex, downVertexIndex);
                    meshData.AddTriangle(downRightVertexIndex, vertexIndex, rightVertexIndex);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }

    private static bool IsNotVertexOnRightOrBottom(int width, int height, int x, int y) {
        return x < width - 1 && y < height - 1;
    }
}

public class MeshData {
    public Vector3[] Vertices;
    public Vector2[] Uvs;
    public int[] OrderedTriangleVertexConnections;

    private int _currentTriangleIndex = 0;

    public static MeshData From(int meshWidth, int meshHeight) {
        return new MeshData(new Vector3[meshWidth * meshHeight],
            new Vector2[meshWidth * meshHeight],
            new int[(meshWidth - 1) * (meshHeight - 1) * 6]);
    }

    private MeshData(Vector3[] vertices, Vector2[] uvs, int[] triangles) {
        Vertices = vertices;
        Uvs = uvs;
        OrderedTriangleVertexConnections = triangles;
    }

    public void AddTriangle(int a, int b, int c) {
        OrderedTriangleVertexConnections[_currentTriangleIndex++] = a;
        OrderedTriangleVertexConnections[_currentTriangleIndex++] = b;
        OrderedTriangleVertexConnections[_currentTriangleIndex++] = c;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertices;
        mesh.triangles = OrderedTriangleVertexConnections;
        mesh.uv = Uvs;
        mesh.normals = CalculateNormals();
        return mesh;
    }

    private Vector3[] CalculateNormals() {
        Vector3[] normals = new Vector3[Vertices.Length];
        int numTriangles = OrderedTriangleVertexConnections.Length / 3;
        for (int triangleStartIndex = 0; triangleStartIndex < OrderedTriangleVertexConnections.Length; triangleStartIndex += 3) {
            int vertexIndexA = OrderedTriangleVertexConnections[triangleStartIndex];
            int vertexIndexB = OrderedTriangleVertexConnections[triangleStartIndex + 1];
            int vertexIndexC = OrderedTriangleVertexConnections[triangleStartIndex + 2];
            Vector3 normal = CalculateVertexNormal(vertexIndexA, vertexIndexB, vertexIndexC);
            normals[vertexIndexA] += normal;
            normals[vertexIndexB] += normal;
            normals[vertexIndexC] += normal;
        }

        for (int normalIndex = 0; normalIndex < normals.Length; normalIndex++) {
            normals[normalIndex] /= 3;
        }

        return normals;
    }

    private Vector3 CalculateVertexNormal(int vertexIndexA, int vertexIndexB, int vertexIndexC) {
        Vector3 a = Vertices[vertexIndexA];
        Vector3 b = Vertices[vertexIndexB];
        Vector3 c = Vertices[vertexIndexC];

        Vector3 ab = b - a;
        Vector3 ac = c - a;
        return Vector3.Cross(ab, ac).normalized;
    }
}
