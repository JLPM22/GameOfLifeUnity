using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

[CustomEditor(typeof(GameOfLife))]
public class GameOfLifeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameOfLife gameOfLife = (GameOfLife)target;

        EditorGUILayout.Space();

        // Initial State
        switch (gameOfLife.InitMode)
        {
            case GameOfLife.Init.Random:
                break;
            case GameOfLife.Init.ScriptableObject:
                gameOfLife.InitialState = EditorGUILayout.ObjectField("Initial State", gameOfLife.InitialState, typeof(Texture2D), false) as Texture2D;
                break;
        }

        EditorGUILayout.Space();

        // Buttons
        GUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(!gameOfLife.Paused);
        if (GUILayout.Button("Resume"))
        {
            gameOfLife.Resume();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(25);
        EditorGUI.BeginDisabledGroup(gameOfLife.Paused);
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

        // Save Current State
        if (GUILayout.Button("Save current state as ScriptableObject"))
        {
            Texture2D asset = gameOfLife.GetCurrentState();
            SaveTextureAsPNG(asset, Path.Combine(Application.dataPath, "Savedstates/NewState.png"));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }

        // Save Changes
        if (GUI.changed && !EditorApplication.isPlaying)
        {
            EditorUtility.SetDirty(gameOfLife);
            EditorSceneManager.MarkSceneDirty(gameOfLife.gameObject.scene);
        }
    }

    private static void SaveTextureAsPNG(Texture2D texture, string fullPath)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);
    }
}
