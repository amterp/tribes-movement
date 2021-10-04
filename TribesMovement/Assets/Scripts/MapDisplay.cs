using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

    [SerializeField] private Renderer _textureRenderer;

    public void DrawNoiseMap(float[,] noiseMap) {
        int numWidthPoints = noiseMap.GetLength(0);
        int numHeightPoints = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(numWidthPoints, numHeightPoints);

        Color[] colorMap = new Color[numWidthPoints * numHeightPoints];
        for (int y = 0; y < numHeightPoints; y++) {
            for (int x = 0; x < numWidthPoints; x++) {
                colorMap[y * numWidthPoints + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();

        _textureRenderer.sharedMaterial.mainTexture = texture;
        _textureRenderer.transform.localScale = new Vector3(numWidthPoints, 1, numHeightPoints);
    }
}
