using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorGrid : Editor
{
    [SerializeField] private bool drawLines = true;
    [SerializeField] private int lenght = 10;
    [SerializeField] private int width = 5;
    [SerializeField] private int lineLength = 300;
    [SerializeField] private int LineStep = 1;
    Vector3 basicPosition = new Vector3(0,0,0);

    List<GameObject> listeGameObjects = new List<GameObject>();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Reset Grid"))
        {
            foreach(GameObject go in listeGameObjects)
            {
                DestroyImmediate(go);
            }
        }
    }

    private void OnSceneGUI()
    {
        if (!drawLines) return;
        CreateGrid();
    }

    public void CreateGrid()
    {
        Debug.Log("create grid");
        Debug.DrawLine(new Vector3(0f, 0f, 0f), Vector3.forward * 20);
        for (int i = 0; i < lenght; i++)
        { 
            Debug.DrawLine(new Vector3(LineStep * i, 0f, 0f), new Vector3(LineStep * i, 0f, lineLength));

            for (int j = 0; j < width; j++)
            {

               Debug.DrawLine(new Vector3(0, 0f, LineStep * j), new Vector3(lineLength, 0f, j * LineStep));
            }
        }
    }
}
