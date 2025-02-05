using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CheckPointManager))]
public class CheckPointManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CheckPointManager myCheckpoint = (CheckPointManager)target;


        if (GUILayout.Button("Create a New CheckPoint"))
        {
            myCheckpoint.CreateCheckPoint();
        }
    }

}
