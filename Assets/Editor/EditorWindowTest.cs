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

    static public GameObject source;
    private bool canSelectable;
    List<GameObject> instantiatedGo = new List<GameObject>();
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

        GUILayout.BeginArea(new Rect(0f, 100f, 500f, 100f));
        GUILayout.Label("TRANFORMS", EditorStyles.boldLabel);
        if (GUILayout.Button("Reset Transform of GameObjects"))
        {
            BResetTransform();
        };
        if (GUILayout.Button("Reset Rotation only of GameObjects"))
        {
            ResetRotation();
        };
        if (GUILayout.Button("DESTROY ALL INSTANTIATED OBJECT"))
        {
            ResetInstantiatedGo();
        };
        GUILayout.EndArea();
        
        GUILayout.BeginArea(new Rect(0f, 200f, 500f, 500f));
        GUILayout.Label("CREATE OBJECTS", EditorStyles.boldLabel);

        canSelectable = EditorGUILayout.Toggle("Peut créer des cubes ?", canSelectable);

        source = EditorGUILayout.ObjectField("Prefab", source, typeof(GameObject), true) as GameObject;


        if (GUILayout.Button("Create Cube"))
        {
            CreateCube(Input.mousePosition);
        };

        GUILayout.EndArea();
    }

    public void CreateLayout()
    {
        foreach (var selectedObject in Selection.gameObjects)
        {
            selectedObject.transform.rotation = Quaternion.Euler(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f));
        }
    }
    private void ResetRotation()
    {
        foreach (var selectedObject in Selection.gameObjects)
        {
            selectedObject.transform.rotation = Quaternion.Euler(0, 0, 0);
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
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }


    //Création de cubes
    private void CreateCube(Vector3 inputPosition)
    {
        //Le cube est sélectionné on peut le placer ou on veut sur la map
        GameObject cube = Instantiate(source);
        cube.GetComponent<Renderer>().sharedMaterial.color = Color.black;
        MeshFilter filter = cube.GetComponent<MeshFilter>();
        Mesh mesh = filter.mesh;

        
        cube.transform.position = inputPosition;
    }


    //Détruire tous les gameobjects instantiés
    private void ResetInstantiatedGo()
    {
        foreach(GameObject go in instantiatedGo)
        {
            DestroyImmediate(go, true);
        }
        instantiatedGo = new List<GameObject>();
    }
    //Fonction créer moi-même lorsque l'on est dans la scene
    private void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();
        if (canSelectable)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                //Le cube est sélectionné on peut le placer ou on veut sur la map
                Debug.Log("créate cube");
                GameObject obj = Instantiate(source);
                obj.transform.position = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
                instantiatedGo.Add(obj);

                DrawGizmoForMyScript(obj);
            }
        }
        Handles.EndGUI();
    }

    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    private static void DrawGizmoForMyScript(GameObject objInstantiated)
    {

    }
}
