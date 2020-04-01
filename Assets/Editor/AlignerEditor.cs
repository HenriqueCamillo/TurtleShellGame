using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Aligner))]
public class AlignerEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Aligner aligner = (Aligner)target;
        if (GUILayout.Button("Align"))
        {
            aligner.Align();
        }
    }
}

