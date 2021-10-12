using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator {

    public static MeshData GenerateMeshTerrain(TerrainData terrain, int levelOfUndetail, Func<float, float> noiseMapper) {
        int numVertices = terrain.GetWidth();
        float halfSize = (numVertices - 1) / 2f;

        int simplificationIncrement = Math.Max(1, levelOfUndetail * 2);
        int verticesPerLine = (numVertices - 1) / simplificationIncrement + 1;

        MeshData meshData = MeshData.From(numVertices, simplificationIncrement, terrain.GetSourceNoiseMap(), noiseMapper);
        int vertexIndex = 0;

        for (int y = 0; y < numVertices; y += simplificationIncrement) {
            for (int x = 0; x < numVertices; x += simplificationIncrement) {

                float centeredX = x - halfSize;
                float centeredY = -y + halfSize;
                meshData.Vertices[vertexIndex] = new Vector3(centeredX, noiseMapper(terrain.GetHeightAtPoint(x, y)), centeredY);
                meshData.Uvs[vertexIndex] = new Vector2(x / (float)(numVertices), y / (float)(numVertices));

                if (IsNotVertexOnRightOrBottom(numVertices, x, y)) {
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

    private static bool IsNotVertexOnRightOrBottom(int numVertices, int x, int y) {
        return x < numVertices - 1 && y < numVertices - 1;
    }
}

public class MeshData {
    public Vector3[] Vertices;
    public Vector2[] Uvs;

    private int[] _orderedTriangleVertexConnections;
    private Vector3[] _normals;
    private int _meshSize;
    private int _meshSideNumVertices;
    private int _simplificationIncrement;
    private Func<int, int, float> _heightProvider;
    private int _currentTriangleIndex = 0;

    public static MeshData From(int numVertices, int simplificationIncrement, NoiseMap sourceNoiseMap, Func<float, float> noiseMapper) {
        Func<int, int, float> heightProvider = (x, y) => noiseMapper(sourceNoiseMap.Sample(x, y));

        return new MeshData(new Vector3[numVertices * numVertices],
            new Vector2[numVertices * numVertices],
            new int[(numVertices - 1) * (numVertices - 1) * 6],
            numVertices - 1,
            simplificationIncrement,
            heightProvider);
    }

    private MeshData(Vector3[] vertices, Vector2[] uvs, int[] triangles, int meshSize, int simplificationIncrement, Func<int, int, float> heightProvider) {
        Vertices = vertices;
        Uvs = uvs;
        _orderedTriangleVertexConnections = triangles;
        _meshSize = meshSize;
        _meshSideNumVertices = meshSize + 1;
        _simplificationIncrement = simplificationIncrement;
        _heightProvider = heightProvider;
    }

    public void AddTriangle(int a, int b, int c) {
        _orderedTriangleVertexConnections[_currentTriangleIndex++] = a;
        _orderedTriangleVertexConnections[_currentTriangleIndex++] = b;
        _orderedTriangleVertexConnections[_currentTriangleIndex++] = c;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertices;
        mesh.triangles = _orderedTriangleVertexConnections;
        mesh.uv = Uvs;
        CalculateNormals();
        mesh.normals = _normals;
        return mesh;
    }

    private void CalculateNormals() {
        _normals = new Vector3[Vertices.Length];
        for (int triangleStartIndex = 0; triangleStartIndex < _orderedTriangleVertexConnections.Length; triangleStartIndex += 3) {
            int vertexIndexA = _orderedTriangleVertexConnections[triangleStartIndex];
            int vertexIndexB = _orderedTriangleVertexConnections[triangleStartIndex + 1];
            int vertexIndexC = _orderedTriangleVertexConnections[triangleStartIndex + 2];
            Vector3 normal = CalculateVertexNormal(vertexIndexA, vertexIndexB, vertexIndexC);
            _normals[vertexIndexA] += normal;
            _normals[vertexIndexB] += normal;
            _normals[vertexIndexC] += normal;
        }

        for (int borderIndexY = -_simplificationIncrement; borderIndexY <= _meshSize; borderIndexY += _simplificationIncrement) {
            for (int borderIndexX = -_simplificationIncrement; borderIndexX <= _meshSize; borderIndexX += _simplificationIncrement) {
                if (!OnBorderingTriangle(borderIndexX, borderIndexY)) continue;

                int halvedX = borderIndexX - _meshSize / 2;
                int flippedHalvedY = -borderIndexY + _meshSize / 2;
                Vector3 a = new Vector3(halvedX, _heightProvider(borderIndexX, borderIndexY), flippedHalvedY);

                int bPointX = borderIndexX + _simplificationIncrement;
                int bPointY = borderIndexY + _simplificationIncrement;
                Vector3 b = new Vector3(halvedX + _simplificationIncrement, _heightProvider(bPointX, bPointY), flippedHalvedY - _simplificationIncrement);

                int cPointX = borderIndexX;
                int cPointY = borderIndexY + _simplificationIncrement;
                Vector3 c = new Vector3(halvedX, _heightProvider(cPointX, cPointY), flippedHalvedY - _simplificationIncrement);

                Vector3 bottomTriangleNormal = CalculateVertexNormal(a, b, c);
                TryAddingBottomTriangleNormalToVertices(borderIndexX, borderIndexY, bottomTriangleNormal);

                int dPointX = borderIndexX + _simplificationIncrement;
                int dPointY = borderIndexY;
                Vector3 d = new Vector3(halvedX + _simplificationIncrement, _heightProvider(dPointX, dPointY), flippedHalvedY);

                Vector3 topTriangleNormal = CalculateVertexNormal(b, a, d);
                TryAddingTopTriangleNormalToVertices(borderIndexX, borderIndexY, topTriangleNormal);
            }
        }

        for (int normalIndex = 0; normalIndex < _normals.Length; normalIndex++) {
            _normals[normalIndex].Normalize();
        }
    }

    private void TryAddingBottomTriangleNormalToVertices(int borderIndexX, int borderIndexY, Vector3 bottomTriangleNormal) {
        Vector2Int vertexPosToAddTo = new Vector2Int(borderIndexX, borderIndexY);
        TryAddingNormalToVertexPos(bottomTriangleNormal, vertexPosToAddTo);
        vertexPosToAddTo += new Vector2Int(0, _simplificationIncrement);
        TryAddingNormalToVertexPos(bottomTriangleNormal, vertexPosToAddTo);
        vertexPosToAddTo += new Vector2Int(_simplificationIncrement, 0);
        TryAddingNormalToVertexPos(bottomTriangleNormal, vertexPosToAddTo);
    }

    private void TryAddingTopTriangleNormalToVertices(int borderIndexX, int borderIndexY, Vector3 topTriangleNormal) {
        Vector2Int vertexPosToAddTo = new Vector2Int(borderIndexX, borderIndexY);
        TryAddingNormalToVertexPos(topTriangleNormal, vertexPosToAddTo);
        vertexPosToAddTo += new Vector2Int(_simplificationIncrement, 0);
        TryAddingNormalToVertexPos(topTriangleNormal, vertexPosToAddTo);
        vertexPosToAddTo += new Vector2Int(0, _simplificationIncrement);
        TryAddingNormalToVertexPos(topTriangleNormal, vertexPosToAddTo);
    }

    private void TryAddingNormalToVertexPos(Vector3 triangleNormal, Vector2Int vertexPosToAddTo) {
        if (vertexPosToAddTo.x < 0 || vertexPosToAddTo.x > _meshSize
            || vertexPosToAddTo.y < 0 || vertexPosToAddTo.y > _meshSize) {
            return;
        }

        int index = vertexPosToAddTo.x / _simplificationIncrement + vertexPosToAddTo.y / _simplificationIncrement * (_meshSize / _simplificationIncrement + 1);
        _normals[index] += triangleNormal;
    }

    private bool OnBorderingTriangle(int borderIndexX, int borderIndexY) {
        return ((borderIndexX == -_simplificationIncrement || borderIndexX == _meshSize) && borderIndexY <= _meshSize)
            || ((borderIndexY == -_simplificationIncrement || borderIndexY == _meshSize) && borderIndexX <= _meshSize);
    }

    private Vector3 CalculateVertexNormal(int vertexIndexA, int vertexIndexB, int vertexIndexC) {
        Vector3 a = Vertices[vertexIndexA];
        Vector3 b = Vertices[vertexIndexB];
        Vector3 c = Vertices[vertexIndexC];

        return CalculateVertexNormal(a, b, c);
    }

    private Vector3 CalculateVertexNormal(Vector3 a, Vector3 b, Vector3 c) {
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        return Vector3.Cross(ab, ac).normalized;
    }
}
