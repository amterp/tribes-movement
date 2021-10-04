using System;
using System.Linq;
using UnityEngine;

public struct Terrain {
    public TerrainPoint[,] Points;

    public Terrain(TerrainPoint[,] terrainPoints) {
        Points = terrainPoints;
    }

    public Color[] GetColors() {
        int width = GetWidth();
        int height = GetHeight();

        Color[] colors = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                colors[y * width + x] = Points[x, y].Color;
            }
        }

        return colors;
    }

    public int GetWidth() {
        return Points.GetLength(0);
    }

    public int GetHeight() {
        return Points.GetLength(1);
    }
}
