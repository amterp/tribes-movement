using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapGenerator))]
public class InfiniteTerrain : MonoBehaviour {
    public const float MAX_VIEW_DISTANCE = 450;
    private static readonly int CHUNKS_VISIBLE_IN_VIEW_DISTANCE = Mathf.RoundToInt(MAX_VIEW_DISTANCE / MapGenerator.MAP_CHUNK_SIZE);

    [SerializeField] private Transform _viewer;
    [SerializeField] private Material _mapChunkMaterial;

    private MapGenerator _mapGenerator;
    private Dictionary<Vector2, TerrainChunk> _chunksByCoordinate;
    private List<TerrainChunk> _chunksVisibleLastFrame;

    void Awake() {
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

        for (int yOffsetFromPlayer = -CHUNKS_VISIBLE_IN_VIEW_DISTANCE; yOffsetFromPlayer <= CHUNKS_VISIBLE_IN_VIEW_DISTANCE; yOffsetFromPlayer++) {
            for (int xOffsetFromPlayer = -CHUNKS_VISIBLE_IN_VIEW_DISTANCE; xOffsetFromPlayer <= CHUNKS_VISIBLE_IN_VIEW_DISTANCE; xOffsetFromPlayer++) {
                Vector2 chunkCoordinateToEnsureVisible = new Vector2(currentChunkCoordinateX + xOffsetFromPlayer, currentChunkCoordinateY + yOffsetFromPlayer);

                TerrainChunk chunk;

                if (!_chunksByCoordinate.ContainsKey(chunkCoordinateToEnsureVisible)) {
                    chunk = TerrainChunk.From(chunkCoordinateToEnsureVisible, MapGenerator.MAP_CHUNK_SIZE, transform, _mapChunkMaterial);
                    _mapGenerator.RequestTerrainData(chunk.Initialize);
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
    private const float INHERENT_PLANE_SIZE = 10f;

    private GameObject _meshObject;
    private Vector3 _worldPosition;
    private Bounds _bounds;
    private MeshRenderer _meshRenderer; // todo why store this?
    private MeshFilter _meshFilter;

    public static TerrainChunk From(Vector2 coordinate, int size, Transform parent, Material chunkMaterial) {
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
            meshFilter);
        chunk.SetVisible(false);

        return chunk;
    }

    private TerrainChunk(GameObject meshObject,
            Vector3 worldPosition,
            Bounds bounds,
            MeshRenderer meshRenderer,
            MeshFilter meshFilter) {
        _meshObject = meshObject;
        _worldPosition = worldPosition;
        _bounds = bounds;
        _meshRenderer = meshRenderer;
        _meshFilter = meshFilter;
    }

    public void Initialize((Terrain, MeshData?) terrainAndMeshData) {
        _meshFilter.mesh = terrainAndMeshData.Item2.CreateMesh();
    }

    public void UpdateVisibility(Vector3 viewerPosition) {
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(viewerPosition));
        SetVisible(viewerDistanceFromNearestEdge <= InfiniteTerrain.MAX_VIEW_DISTANCE);
    }

    public bool IsVisible() {
        return _meshObject.activeSelf;
    }

    public void SetVisible(bool isVisible) {
        _meshObject.SetActive(true);
    }
}
