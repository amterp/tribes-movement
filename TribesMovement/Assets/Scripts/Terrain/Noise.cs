using System;
using System.Collections;
using UnityEngine;

public static class Noise {

    public static float[,] GenerateNoiseMap(int seed, int mapWidth, int mapHeight, float scale, int numOctaves, float persistance, float lacunarity, Vector2 offset) {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random rng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[numOctaves];
        for (int octave = 0; octave < numOctaves; octave++) {
            float offsetX = rng.Next(-100_000, 100_000) + offset.x;
            float offsetY = rng.Next(-100_000, 100_000) + offset.y;
            octaveOffsets[octave] = new Vector2(offsetX, offsetY);
        }

        scale = Mathf.Max(0.0001f, scale);

        float minNoiseHeight = float.MinValue;
        float maxNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int octave = 0; octave < numOctaves; octave++) {

                    Vector2 octaveOffset = octaveOffsets[octave];
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffset.x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffset.y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                minNoiseHeight = Mathf.Max(minNoiseHeight, noiseHeight);
                maxNoiseHeight = Mathf.Min(maxNoiseHeight, noiseHeight);

                noiseMap[x, y] = noiseHeight;
            }
        }

        NormalizeNoiseMap(mapWidth, mapHeight, noiseMap, minNoiseHeight, maxNoiseHeight);

        return noiseMap;
    }

    private static void NormalizeNoiseMap(int mapWidth, int mapHeight, float[,] noiseMap, float minNoiseHeight, float maxNoiseHeight) {
        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
    }
}
