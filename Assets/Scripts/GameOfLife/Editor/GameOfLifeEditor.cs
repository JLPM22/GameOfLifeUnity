using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(GameOfLife))]
public class GameOfLifeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameOfLife gameOfLife = (GameOfLife)target;

        EditorGUILayout.Space();

        // Buttons
        GUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(gameOfLife.Paused);
        if (GUILayout.Button("Resume"))
        {
            gameOfLife.Resume();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(25);
        EditorGUI.BeginDisabledGroup(!gameOfLife.Paused);
        if (GUILayout.Button("Pause"))
        {
            gameOfLife.Pause();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(25);
        if (GUILayout.Button("Restart"))
        {
            gameOfLife.Restart();
        }
        GUILayout.EndHorizontal();

        // Save Changes
        if (GUI.changed && !EditorApplication.isPlaying)
        {
            EditorUtility.SetDirty(gameOfLife);
            EditorSceneManager.MarkSceneDirty(gameOfLife.gameObject.scene);
        }
    }
}
