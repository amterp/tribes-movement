using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TerrainPoint {
    public Vector2 Position;
    public float NoiseValue;
    public Color Color;

    public TerrainPoint(float x, float y, float noiseValue, Color color) : this(new Vector2(x, y), noiseValue, color) {
    }

    public TerrainPoint(Vector2 position, float noiseValue, Color color) {
        Position = position;
        NoiseValue = noiseValue;
        Color = color;
    }
}

[Serializable]
public struct TerrainType {
    public string Name;
    public float HeightEnd;
    public Color Color;
}
