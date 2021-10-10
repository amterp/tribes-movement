using System;
using System.Collections;
using UnityEngine;

public static class Noise {

    public static float[,] GenerateNoiseMap(int seed, int mapWidth, int mapHeight, float scale, int numOctaves, float persistance, float lacunarity, Vector2 offset) {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        (float minHeight, float maxHeight) = CalculateHeightExtremes(numOctaves, persistance);

        System.Random rng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[numOctaves];
        for (int octave = 0; octave < numOctaves; octave++) {
            float offsetX = rng.Next(-100_000, 100_000) + offset.x;
            float offsetY = rng.Next(-100_000, 100_000) - offset.y;
            octaveOffsets[octave] = new Vector2(offsetX, offsetY);
        }

        scale = Mathf.Max(0.0001f, scale);

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int octave = 0; octave < numOctaves; octave++) {

                    Vector2 octaveOffset = octaveOffsets[octave];
                    float sampleX = (x - halfWidth + octaveOffset.x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffset.y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                noiseMap[x, y] = Normalize(minHeight, maxHeight, noiseHeight);
            }
        }

        return noiseMap;
    }

    private static (float minHeight, float maxHeight) CalculateHeightExtremes(int numOctaves, float persistance) {
        float theoExtreme = 0;
        float amplitude = 1;
        for (int octave = 0; octave < numOctaves; octave++) {
            theoExtreme += amplitude;
            amplitude *= persistance;
        }
        return (0, theoExtreme);
    }

    private static float Normalize(float minHeight, float maxHeight, float value) {
        return Mathf.InverseLerp(minHeight, maxHeight, value);
    }
}
