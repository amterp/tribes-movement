using System;
using System.Collections;
using UnityEngine;

public static class Noise {
    public const int MAX_BORDER_OFFSET = TerrainConstants.MAX_LEVEL_OF_UNDETAIL * 2 * 2;
    public const int MAX_HALF_BORDER_OFFSET = MAX_BORDER_OFFSET / 2;

    public static NoiseMap GenerateNoiseMap(int seed, int mapNumPoints, float scale, int numOctaves, float persistance, float lacunarity, Vector2 offset) {
        float[,] noiseMap = new float[mapNumPoints + MAX_BORDER_OFFSET, mapNumPoints + MAX_BORDER_OFFSET];

        (float minHeight, float maxHeight) = CalculateHeightExtremes(numOctaves, persistance);

        System.Random rng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[numOctaves];
        for (int octave = 0; octave < numOctaves; octave++) {
            float offsetX = rng.Next(-100_000, 100_000) + offset.x;
            float offsetY = rng.Next(-100_000, 100_000) - offset.y;
            octaveOffsets[octave] = new Vector2(offsetX, offsetY);
        }

        scale = Mathf.Max(0.0001f, scale);

        float halfSize = mapNumPoints / 2f;

        for (int y = -MAX_HALF_BORDER_OFFSET; y < mapNumPoints + MAX_HALF_BORDER_OFFSET; y++) {
            for (int x = -MAX_HALF_BORDER_OFFSET; x < mapNumPoints + MAX_HALF_BORDER_OFFSET; x++) {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int octave = 0; octave < numOctaves; octave++) {

                    Vector2 octaveOffset = octaveOffsets[octave];
                    float sampleX = (x - halfSize + octaveOffset.x) / scale * frequency;
                    float sampleY = (y - halfSize + octaveOffset.y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                noiseMap[x + MAX_HALF_BORDER_OFFSET, y + MAX_HALF_BORDER_OFFSET] = Normalize(minHeight, maxHeight, noiseHeight);
            }
        }

        return new NoiseMap(noiseMap);
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

public struct NoiseMap {
    private float[,] _map;

    public NoiseMap(float[,] map) {
        _map = map;
    }

    public float Sample(int x, int y) {
        return _map[x + Noise.MAX_HALF_BORDER_OFFSET, y + Noise.MAX_HALF_BORDER_OFFSET];
    }

    public int GetSize() {
        return _map.GetLength(0) - Noise.MAX_BORDER_OFFSET;
    }
}
