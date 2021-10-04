using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        MapGenerator mapGenerator = (MapGenerator)target;

        if (DrawInspectorAndIsUpdated() && mapGenerator.AutoUpdate) {
            mapGenerator.GenerateMap();
        }

        if (GUILayout.Button("Generate")) {
            mapGenerator.GenerateMap();
        }
    }

    private bool DrawInspectorAndIsUpdated() {
        return DrawDefaultInspector();
    }
}
