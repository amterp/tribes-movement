using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MapDisplay))]
public class MapGenerator : MonoBehaviour {

    public const int MAP_CHUNK_NUM_VERTICES = 241;
    public const int MAP_CHUNK_SIZE = MAP_CHUNK_NUM_VERTICES - 1;

    public bool AutoUpdate { get { return _isAutoUpdate; } private set { _isAutoUpdate = value; } }

    [SerializeField] private int _seed = 0;
    [Range(1, 24)]
    [SerializeField] private int _numThreads = 12;
    [Range(0, 6)]
    [SerializeField] private int _levelOfUndetail = 4;
    [Range(2, 200)]
    [SerializeField] private float _noiseScale = 4;
    [Range(1, 8)]
    [SerializeField] private int _numOctaves = 4;
    [Range(-2, 2)]
    [SerializeField] private float _persistance = 0.5f;
    [Range(0, 5)]
    [SerializeField] private float _lacunarity = 2f;
    [SerializeField] private Vector2 _offset = new Vector2(0, 0);
    [SerializeField] private bool _isAutoUpdate = true;
    [SerializeField] private bool _isUseColor = true;
    [SerializeField] private bool _isGenerateMesh = true;
    [Range(0, 100)]
    [SerializeField] private float _heightMultiplier = 20;
    [SerializeField] private AnimationCurve _heightCurve;
    [SerializeField] private TerrainType[] _terrainTypes;

    private MapDisplay _mapDisplay;
    private TaskPool<(Terrain, MeshData?)> _taskPool;

    void Awake() {
        _mapDisplay = GetComponent<MapDisplay>();
        _taskPool = TaskPool<(Terrain, MeshData?)>.FixedSize(_numThreads);
    }

    void Update() {
        if (!Application.isPlaying) return;
        _taskPool.CallbackOnResults();
    }

    public void GenerateAndDisplayTerrain() {
        (Terrain terrain, MeshData meshData) = GenerateTerrainData();
        _mapDisplay.Display(terrain, meshData);
    }

    public void RequestTerrainData(Action<(Terrain, MeshData?)> callback) {
        _taskPool.Submit(GenerateTerrainData, callback);
    }

    public (Terrain, MeshData?) GenerateTerrainData() {
        float[,] noiseMap = Noise.GenerateNoiseMap(_seed, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE, _noiseScale, _numOctaves, _persistance, _lacunarity, _offset);
        Terrain terrain = CreateTerrain(noiseMap, _terrainTypes, _isUseColor, _isGenerateMesh);

        Func<float, float> noiseToHeightMapper = (noiseValue) => _heightCurve.Evaluate(noiseValue) * _heightMultiplier;
        MeshData? meshData = _isGenerateMesh ? MeshGenerator.GenerateMeshTerrain(terrain, _levelOfUndetail, noiseToHeightMapper) : null;

        return (terrain, meshData);
    }

    private static Terrain CreateTerrain(float[,] noiseMap, TerrainType[] terrainTypes, bool isUseColor, bool isGenerateMesh) {
        int numWidthPoints = noiseMap.GetLength(0);
        int numHeightPoints = noiseMap.GetLength(1);

        TerrainPoint[,] terrainPoints = new TerrainPoint[numWidthPoints, numHeightPoints];

        for (int y = 0; y < numHeightPoints; y++) {
            for (int x = 0; x < numWidthPoints; x++) {
                float noiseValue = noiseMap[x, y];
                Color color = ResolveColor(noiseValue, terrainTypes, isUseColor);
                terrainPoints[x, y] = new TerrainPoint(x, y, noiseValue, color);
            }
        }

        return new Terrain(terrainPoints);
    }

    private static Color ResolveColor(float noiseHeight, TerrainType[] terrainTypes, bool isUseColor) {
        if (!isUseColor) {
            return Color.Lerp(Color.black, Color.white, noiseHeight);
        } else {
            foreach (TerrainType terrainType in terrainTypes) {
                if (noiseHeight <= terrainType.HeightEnd) {
                    return terrainType.Color;
                }
            }
            return Color.magenta;
        }
    }
}
