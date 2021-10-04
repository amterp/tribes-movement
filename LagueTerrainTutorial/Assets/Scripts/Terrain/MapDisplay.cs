using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapDisplay : MonoBehaviour {

    [SerializeField] private GameObject _nonMeshTarget;
    [SerializeField] private GameObject _meshTarget;

    private Renderer _nonMeshRenderer;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    void Awake() {
        Initialize();
    }

    public void Display(Terrain terrain, MeshData? meshData) {
        Initialize();
        Texture2D texture = TextureGenerator.From(terrain);
        if (meshData == null) {
            _nonMeshRenderer.sharedMaterial.mainTexture = texture;
            _nonMeshRenderer.transform.localScale = new Vector3(terrain.GetWidth(), 1, terrain.GetHeight());
        } else {
            _meshFilter.sharedMesh = meshData.CreateMesh();
            _meshRenderer.sharedMaterial.mainTexture = texture;
        }
    }

    private void Initialize() {
        if (_nonMeshRenderer != null) return;
        _nonMeshRenderer = _nonMeshTarget.GetComponent<Renderer>();
        _meshFilter = _meshTarget.GetComponent<MeshFilter>();
        _meshRenderer = _meshTarget.GetComponent<MeshRenderer>();
        Debug.Log("Initialized.");
    }
}
