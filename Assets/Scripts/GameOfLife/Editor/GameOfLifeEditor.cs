using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameOfLife))]
public class GameOfLifeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


    }
}
