using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator {
    public static Texture2D From(Terrain terrain) {
        Texture2D texture = new Texture2D(terrain.GetWidth(), terrain.GetHeight());
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(terrain.GetColors());
        texture.Apply();
        return texture;
    }
}
