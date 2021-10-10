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
    [SerializeField] private int _editorLevelOfUndetail = 4;
    [Range(2, 2000)]
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
    [Range(0, 1000)]
    [SerializeField] private float _heightMultiplier = 20;
    [SerializeField] private AnimationCurve _heightCurve;
    [SerializeField] private TerrainType[] _terrainTypes;

    private MapDisplay _mapDisplay;
    private TaskPool<TerrainData> _terrainDataTaskPool;
    private TaskPool<MeshData> _terrainMeshTaskPool;
    private ObjectPool<AnimationCurve> _heightCurvePool;

    void Awake() {
        Initialize();
    }

    void Update() {
        if (!Application.isPlaying) return;
        _terrainDataTaskPool.CallbackOnResults();
        _terrainMeshTaskPool.CallbackOnResults();
    }

    public void GenerateAndDisplayTerrainForEditor() {
        Initialize();
        TerrainData terrain = GenerateTerrainData(Vector2.zero);
        MeshData? meshData = _isGenerateMesh ? GenerateTerrainMeshData(terrain, _editorLevelOfUndetail) : null;
        _mapDisplay.Display(terrain, meshData);
    }

    public TerrainData GenerateTerrainData(Vector2 center) {
        float[,] noiseMap = Noise.GenerateNoiseMap(_seed, MAP_CHUNK_NUM_VERTICES, MAP_CHUNK_NUM_VERTICES, _noiseScale, _numOctaves, _persistance, _lacunarity, center + _offset);
        return CreateTerrain(noiseMap, _terrainTypes, _isUseColor, _isGenerateMesh);
    }

    public void RequestTerrainData(Vector2 center, Action<TerrainData> callback) {
        _terrainDataTaskPool.Submit(() => GenerateTerrainData(center), callback);
    }

    public MeshData GenerateTerrainMeshData(TerrainData terrain, int levelOfUndetail) {
        Func<float, float> noiseToHeightMapper = noiseValue => _heightCurvePool.Lease(heightCurve => heightCurve.Evaluate(noiseValue) * _heightMultiplier);
        return MeshGenerator.GenerateMeshTerrain(terrain, levelOfUndetail, noiseToHeightMapper);
    }

    public void RequestTerrainMeshData(TerrainData terrain, int levelOfUndetail, Action<MeshData> callback) {
        _terrainMeshTaskPool.Submit(() => GenerateTerrainMeshData(terrain, levelOfUndetail), callback);
    }

    private void Initialize() {
        Debug.Log("MapGenerator initializing...");
        _mapDisplay = GetComponent<MapDisplay>();
        _terrainDataTaskPool = TaskPool<TerrainData>.FixedSize(_numThreads);
        _terrainMeshTaskPool = TaskPool<MeshData>.FixedSize(_numThreads);
        _heightCurvePool = ObjectPool<AnimationCurve>.Create(() => new AnimationCurve(_heightCurve.keys));
    }

    private static TerrainData CreateTerrain(float[,] noiseMap, TerrainType[] terrainTypes, bool isUseColor, bool isGenerateMesh) {
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

        return new TerrainData(terrainPoints);
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
