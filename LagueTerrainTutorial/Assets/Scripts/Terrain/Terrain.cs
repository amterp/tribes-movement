using System;
using System.Linq;
using UnityEngine;

public struct Terrain {
    private readonly TerrainPoint[,] _points;

    public Terrain(TerrainPoint[,] terrainPoints) {
        _points = terrainPoints;
    }

    public Color[] GetColors() {
        int width = GetWidth();
        int height = GetHeight();

        Color[] colors = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                colors[y * width + x] = _points[x, y].Color;
            }
        }

        return colors;
    }

    public float GetHeightAtPoint(int x, int y) {
        Debug.Assert(0 <= x && x < GetWidth(), "x is out of bounds [0, width)");
        Debug.Assert(0 <= y && y < GetHeight(), "y is out of bounds [0, height)");
        return _points[x, y].NoiseValue;
    }

    public int GetWidth() {
        return _points.GetLength(0);
    }

    public int GetHeight() {
        return _points.GetLength(1);
    }
}
