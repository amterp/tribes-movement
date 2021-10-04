using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

    [SerializeField] private Renderer _textureRenderer;

    public void Display(Terrain terrain) {
        Texture2D texture = TextureGenerator.From(terrain);
        _textureRenderer.sharedMaterial.mainTexture = texture;
        _textureRenderer.transform.localScale = new Vector3(terrain.GetWidth(), 1, terrain.GetHeight());
    }
}
