using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TilemapManagerScript))]
public class TilemapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = (TilemapManagerScript)target;

        if (GUILayout.Button("Save Map"))
        {
            script.SaveMap();
        }

        if (GUILayout.Button("Load Map"))
        {
            script.LoadMap();
        }

        if (GUILayout.Button("Clear Map"))
        {
            script.ClearMap();
        }
    }
}
