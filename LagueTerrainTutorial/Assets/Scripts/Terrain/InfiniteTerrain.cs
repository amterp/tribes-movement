using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapGenerator))]
public class InfiniteTerrain : MonoBehaviour {

    [SerializeField] private Transform _viewer;
    [SerializeField] private Material _mapChunkMaterial;
    [SerializeField] private LodInfo[] _lods;

    private float maxViewDistance;
    private int maxChunksViewDistance;
    private MapGenerator _mapGenerator;
    private Dictionary<Vector2, TerrainChunk> _chunksByCoordinate;
    private List<TerrainChunk> _chunksVisibleLastFrame;

    void Awake() {
        maxViewDistance = _lods.Last().VisibleDistanceEnd;
        maxChunksViewDistance = Mathf.RoundToInt(maxViewDistance / MapGenerator.MAP_CHUNK_SIZE);
        _mapGenerator = GetComponent<MapGenerator>();
        _chunksByCoordinate = new Dictionary<Vector2, TerrainChunk>();
        _chunksVisibleLastFrame = new List<TerrainChunk>();
    }

    void Update() {
        UpdateVisibleChunks();
    }

    public void UpdateVisibleChunks() {
        _chunksVisibleLastFrame.ForEach(chunk => chunk.SetVisible(false));
        _chunksVisibleLastFrame.Clear();

        int currentChunkCoordinateX = Mathf.RoundToInt(_viewer.transform.position.x / MapGenerator.MAP_CHUNK_SIZE);
        int currentChunkCoordinateY = Mathf.RoundToInt(_viewer.transform.position.z / MapGenerator.MAP_CHUNK_SIZE);

        for (int yOffsetFromPlayer = -maxChunksViewDistance; yOffsetFromPlayer <= maxChunksViewDistance; yOffsetFromPlayer++) {
            for (int xOffsetFromPlayer = -maxChunksViewDistance; xOffsetFromPlayer <= maxChunksViewDistance; xOffsetFromPlayer++) {
                Vector2 chunkCoordinateToEnsureVisible = new Vector2(currentChunkCoordinateX + xOffsetFromPlayer, currentChunkCoordinateY + yOffsetFromPlayer);

                TerrainChunk chunk;

                if (!_chunksByCoordinate.ContainsKey(chunkCoordinateToEnsureVisible)) {
                    chunk = TerrainChunk.From(chunkCoordinateToEnsureVisible, MapGenerator.MAP_CHUNK_SIZE, transform, _mapChunkMaterial, _lods, maxViewDistance, _mapGenerator.RequestTerrainMeshData);
                    _mapGenerator.RequestTerrainData(chunk.WorldPosition.DropY(), chunk.InitializeTerrainData);
                    _chunksByCoordinate.Add(chunkCoordinateToEnsureVisible, chunk);
                } else {
                    chunk = _chunksByCoordinate[chunkCoordinateToEnsureVisible];
                }

                chunk.UpdateVisibility(_viewer.transform.position);
                if (chunk.IsVisible())
                    _chunksVisibleLastFrame.Add(chunk);
            }
        }
    }
}

public class TerrainChunk {

    public Vector3 WorldPosition { get { return _worldPosition; } private set { } }

    private GameObject _meshObject;
    private Vector3 _worldPosition;
    private Bounds _bounds;
    private TerrainData? _terrainData;
    private MeshRenderer _meshRenderer; // todo why store this?
    private MeshFilter _meshFilter;
    private LodInfo[] _lods;
    private float _maxViewDistance;
    private Action<TerrainData, int, Action<MeshData>> _meshDataLoader;
    private LoadingCache<int, MeshData> _meshDataByLodLoadingCache;
    private LoadingCache<int, Mesh> _meshByLodLoadingCache;
    private int _previousLevelOfUndetail = -1;

    public static TerrainChunk From(Vector2 coordinate,
            int size,
            Transform parent,
            Material chunkMaterial,
            LodInfo[] lods,
            float maxViewDistance,
            Action<TerrainData, int, Action<MeshData>> meshDataLoader) {
        Vector3 worldPosition = new Vector3(coordinate.x * size, 0, coordinate.y * size);
        Bounds bounds = new Bounds(worldPosition, Vector3.one * size);

        GameObject meshObject = new GameObject("Terrain Chunk");
        meshObject.transform.position = worldPosition;
        meshObject.transform.SetParent(parent);

        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshRenderer.material = chunkMaterial;
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();

        TerrainChunk chunk = new TerrainChunk(meshObject,
            worldPosition,
            bounds,
            meshRenderer,
            meshFilter,
            lods,
            maxViewDistance,
            meshDataLoader);
        chunk.SetVisible(false);

        return chunk;
    }

    private TerrainChunk(GameObject meshObject,
            Vector3 worldPosition,
            Bounds bounds,
            MeshRenderer meshRenderer,
            MeshFilter meshFilter,
            LodInfo[] lods,
            float maxViewDistance,
            Action<TerrainData, int, Action<MeshData>> meshDataLoader) {
        _meshObject = meshObject;
        _worldPosition = worldPosition;
        _bounds = bounds;
        _meshRenderer = meshRenderer;
        _meshFilter = meshFilter;
        _lods = lods;
        _maxViewDistance = maxViewDistance;
        _meshDataLoader = meshDataLoader;
        _meshByLodLoadingCache = LoadingCache<int, Mesh>.Create((levelOfUndetail, callback) => LoadMesh(levelOfUndetail, callback));
    }

    public void InitializeTerrainData(TerrainData terrainData) {
        _terrainData = terrainData;
        _meshDataByLodLoadingCache = LoadingCache<int, MeshData>.Create((levelOfUndetail, callback) => _meshDataLoader(terrainData, levelOfUndetail, callback));
    }

    public void UpdateVisibility(Vector3 viewerPosition) {
        if (_terrainData == null) return;

        float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(viewerPosition));
        if (viewerDistanceFromNearestEdge > _maxViewDistance) {
            SetVisible(false);
            return;
        }

        foreach (LodInfo lod in _lods) {
            if (viewerDistanceFromNearestEdge <= lod.VisibleDistanceEnd) {
                LoadLodMesh(lod.LevelOfUndetail);
                SetVisible(true);
                return;
            }
        }
        Debug.LogWarning("No LODs picked in foreach loop, should not be possible.");
    }

    public bool IsVisible() {
        return _meshObject.activeSelf;
    }

    public void SetVisible(bool isVisible) {
        _meshObject.SetActive(isVisible);
    }

    private void LoadLodMesh(int levelOfUndetail) {
        if (levelOfUndetail == _previousLevelOfUndetail) return;
        _meshDataByLodLoadingCache.Load(levelOfUndetail, meshData => OnMeshDataLoaded(levelOfUndetail, meshData));
        _previousLevelOfUndetail = levelOfUndetail;
    }

    private void OnMeshDataLoaded(int levelOfUndetail, MeshData meshData) {
        _meshRenderer.material.mainTexture = TextureGenerator.From((TerrainData)_terrainData);
        _meshByLodLoadingCache.Load(levelOfUndetail, OnMeshLoaded);
    }

    private void OnMeshLoaded(Mesh mesh) {
        _meshFilter.mesh = mesh;
    }

    private void LoadMesh(int levelOfUndetail, Action<Mesh> callback) {
        MeshData? meshData = _meshDataByLodLoadingCache.Get(levelOfUndetail);

        if (meshData == null) {
            Debug.LogWarning("Expecting non null mesh data here!");
            return;
        }

        callback(meshData.CreateMesh());
    }
}
