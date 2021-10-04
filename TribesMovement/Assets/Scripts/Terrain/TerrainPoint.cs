using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TerrainPoint {
    public Vector3 Position;
    public Color Color;

    public TerrainPoint(float x, float y, float z, Color color) : this(new Vector3(x, y, z), color) {
    }

    public TerrainPoint(Vector3 position, Color color) {
        Position = position;
        Color = color;
    }
}

[Serializable]
public struct TerrainType {
    public string Name;
    public float HeightEnd;
    public Color Color;
}
