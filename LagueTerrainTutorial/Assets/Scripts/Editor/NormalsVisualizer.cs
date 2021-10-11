using UnityEditor;
using UnityEngine;

// Script from gist: https://gist.github.com/mandarinx/ed733369fbb2eea6c7fa9e3da65a0e17
[CustomEditor(typeof(MeshFilter))]
public class NormalsVisualizer : Editor {

    private Mesh _mesh;

    void OnEnable() {
        MeshFilter meshFilter = target as MeshFilter;
        if (meshFilter != null) {
            _mesh = meshFilter.sharedMesh;
        }
    }

    void OnSceneGUI() {
        if (_mesh == null) {
            return;
        }

        Handles.matrix = (target as MeshFilter).transform.localToWorldMatrix;
        Handles.color = Color.yellow;
        Vector3[] vertices = _mesh.vertices;
        Vector3[] normals = _mesh.normals;
        int numVertices = _mesh.vertexCount;

        for (int vertexIndex = 0; vertexIndex < numVertices; vertexIndex++) {
            Handles.DrawLine(vertices[vertexIndex], vertices[vertexIndex] + normals[vertexIndex] * 1.5f);
        }
    }
}