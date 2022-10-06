using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class EditorWindowTest : EditorWindow
{
    [MenuItem("Window/EditorTest")]
    public static void OpenWindow()
    {
        EditorWindow.GetWindow<EditorWindowTest>("EditorTest");
    }
 
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0f, 10f, 200f, 100f));
        GUILayout.Label("CATEGORIES", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Selected Objects"))
        {
            CreateLayout();
        };
        GUILayout.Button("Level Creator");
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(0f, 100f, 200f, 100f));
        GUILayout.Label("TRANFORMS", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset Transform of GameObjects"))
        {
            BResetTransform();
        };
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        
        
        GUILayout.BeginArea(new Rect(0f, 200f, 200f, 100f));
        GUILayout.Label("CREATE OBJECTS", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Cube"))
        {
            CreateCube(Input.mousePosition);
        };
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public void CreateLayout()
    {
        foreach (var selectedObject in Selection.gameObjects)
        {
            selectedObject.transform.rotation = Quaternion.Euler(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f));
        }
    }

    private void BResetTransform()
    {
        foreach (var selectedObject in Selection.gameObjects)
        {
            selectedObject.transform.rotation = Quaternion.Euler(0,0,0);
            selectedObject.transform.position = new Vector3(0,0,0);
        }
    }

    private void CreateCube(Vector3 inputPosition)
    {
        //Le cube est sélectionné on peut le placer ou on veut sur la map
        GameObject cube = new GameObject();
        cube.transform.position = inputPosition;
    }
}
