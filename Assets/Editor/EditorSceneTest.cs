using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorSceneTest : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("GenerateCube"))
        {

        }
    }
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if(Event.current.type == EventType.MouseDown)
        {
            Ray worlray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitIfos;
            if (Physics.Raycast(worlray, out hitIfos, 10000f))
            {
                Debug.Log("raycast en cours");
            }
        }      
    }
}
