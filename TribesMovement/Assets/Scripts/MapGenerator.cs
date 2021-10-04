using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MapDisplay))]
public class MapGenerator : MonoBehaviour {

    public bool AutoUpdate { get { return _isAutoUpdate; } private set { _isAutoUpdate = value; } }

    [SerializeField] private int _mapWidth = 50;
    [SerializeField] private int _mapHeight = 50;
    [SerializeField] private float _noiseScale = 4;
    [SerializeField] private bool _isAutoUpdate = true;

    private MapDisplay _mapDisplay;

    void Awake() {
        _mapDisplay = GetComponent<MapDisplay>();
    }

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(_mapWidth, _mapHeight, _noiseScale);
        _mapDisplay.DrawNoiseMap(noiseMap);
    }
}
