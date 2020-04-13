﻿#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


[ExecuteInEditMode]
public class Aligner : MonoBehaviour 
{
    [SerializeField] float cellSize;
    [SerializeField] int gridSize;
    [SerializeField] Transform origin;

    public void Align()
    {   
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform child = this.transform.GetChild(i).transform;
            
            float x = Mathf.Floor(child.transform.position.x) + cellSize/2;
            float y = Mathf.Floor(child.transform.position.y) + cellSize/2;
            float z = Mathf.Floor(child.transform.position.z) + cellSize/2;

            child.transform.position = new Vector3(x,y,z);

            if (x < origin.position.x || y < origin.position.y || x > origin.position.x + gridSize * cellSize || y > origin.position.y + gridSize * cellSize)
                Debug.LogError(child.name + " is outside the grid");
        }
    }

    public void SetFlags()
    {
        EditorSceneManager.MarkSceneDirty(this.gameObject.scene);

        for (int i = 0; i < this.transform.childCount; i++)
        {
            Snappable snappableChild = this.transform.GetChild(i).GetComponent<Snappable>();

            if (snappableChild == null)
                Debug.LogError(snappableChild.name + " is not snappable");
            else
            {
                EditorUtility.SetDirty(snappableChild);
                snappableChild.positionedInInspector = true;
                snappableChild.isPlaced = false;
            }
        }
    }
}

#endif