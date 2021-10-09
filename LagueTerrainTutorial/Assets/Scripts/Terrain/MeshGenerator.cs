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
        mesh.RecalculateNormals();
        return mesh;
    }
}
