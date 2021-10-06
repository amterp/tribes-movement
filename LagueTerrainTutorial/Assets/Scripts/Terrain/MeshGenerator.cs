using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {

    public static MeshData GenerateMeshTerrain(Terrain terrain, int levelOfUndetail, Func<float, float> noiseMapper) {
        int width = terrain.GetWidth();
        int height = terrain.GetHeight();

        int simplificationIncrement = Math.Max(1, levelOfUndetail * 2);
        int verticesPerLine = (width - 1) / simplificationIncrement + 1;

        MeshData meshData = MeshData.From(width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y += simplificationIncrement) {
            for (int x = 0; x < width; x += simplificationIncrement) {

                meshData.Vertices[vertexIndex] = new Vector3(x, noiseMapper(terrain.GetHeightAtPoint(x, y)), y);
                meshData.Uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (IsNotVertexOnRightOrBottom(width, height, x, y)) {
                    int downRightVertexIndex = vertexIndex + verticesPerLine + 1;
                    int downVertexIndex = vertexIndex + verticesPerLine;
                    int rightVertexIndex = vertexIndex + 1;

                    // for some reason I have to do this anti-clockwise in order to show properly, 
                    // but I expect culling should be done on anticlockwise triangles. Why is this backwards?
                    meshData.AddTriangle(vertexIndex, downVertexIndex, downRightVertexIndex);
                    meshData.AddTriangle(downRightVertexIndex, rightVertexIndex, vertexIndex);
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
    public int[] Triangles;

    private int _currentTriangleIndex = 0;

    public static MeshData From(int meshWidth, int meshHeight) {
        return new MeshData(new Vector3[meshWidth * meshHeight],
            new Vector2[meshWidth * meshHeight],
            new int[(meshWidth - 1) * (meshHeight - 1) * 6]);
    }

    private MeshData(Vector3[] vertices, Vector2[] uvs, int[] triangles) {
        Vertices = vertices;
        Uvs = uvs;
        Triangles = triangles;
    }

    public void AddTriangle(int a, int b, int c) {
        Triangles[_currentTriangleIndex++] = a;
        Triangles[_currentTriangleIndex++] = b;
        Triangles[_currentTriangleIndex++] = c;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        mesh.uv = Uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
