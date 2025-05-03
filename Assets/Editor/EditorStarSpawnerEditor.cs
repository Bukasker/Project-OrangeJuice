using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EditorStarSpawner))]
public class EditorStarSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorStarSpawner spawner = (EditorStarSpawner)target;
        if (GUILayout.Button("Generate Stars"))
        {
            spawner.GenerateStars();
        }
    }
}
