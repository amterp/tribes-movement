using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MapDisplay))]
public class MapGenerator : MonoBehaviour {

    public bool AutoUpdate { get { return _isAutoUpdate; } private set { _isAutoUpdate = value; } }

    [SerializeField] private int _seed = 0;
    [Range(1, 1000)]
    [SerializeField] private int _mapWidth = 50;
    [Range(1, 1000)]
    [SerializeField] private int _mapHeight = 50;
    [Range(2, 200)]
    [SerializeField] private float _noiseScale = 4;
    [Range(0, 20)]
    [SerializeField] private int _numOctaves = 4;
    [Range(-2, 2)]
    [SerializeField] private float _persistance = 0.5f;
    [Range(0, 5)]
    [SerializeField] private float _lacunarity = 2f;
    [SerializeField] private Vector2 _offset = new Vector2(0, 0);
    [SerializeField] private bool _isAutoUpdate = true;

    private MapDisplay _mapDisplay;

    void Awake() {
        _mapDisplay = GetComponent<MapDisplay>();
    }

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(_seed, _mapWidth, _mapHeight, _noiseScale, _numOctaves, _persistance, _lacunarity, _offset);
        _mapDisplay.DrawNoiseMap(noiseMap);
    }
}
