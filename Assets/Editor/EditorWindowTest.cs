using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;
public class EditorWindowTest : EditorWindow
{
    [MenuItem("Window/EditorTest")]
    public static void OpenWindow()
    {
        EditorWindow.GetWindow<EditorWindowTest>("EditorTest");
    }
    /// <summary>
    /// Prefab à instantier
    /// </summary>
    static public GameObject source;

    /// <summary>
    /// Si on utilise le draw
    /// </summary>
    private bool canSelectable;

    /// <summary>
    /// Hauteur de la position en Y
    /// </summary>
    private float YPosition = 0f;
  
    /// <summary>
    /// Récupérer les types d'events de Unity
    /// </summary>
    public EventType eventType;

    [Flags] public enum OurEventTypes
    {
        MouseDown,
        MouseMove
    }

    public OurEventTypes ourEventType;

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
        
        GUILayout.BeginArea(new Rect(0f, 200f, 600f, 500f));
        GUILayout.Label("CREATE OBJECTS", EditorStyles.boldLabel);

        eventType = (EventType)EditorGUILayout.EnumFlagsField("Mode de draw", eventType);

        switch (eventType)
        {
            case EventType.MouseDown:
                InstantiatePrimitive(eventType);
                break;
            case EventType.MouseMove:
                InstantiatePrimitive(eventType);
                break;
        }
        canSelectable = EditorGUILayout.Toggle("Peut créer des cubes ?", canSelectable);

        source = EditorGUILayout.ObjectField("Prefab", source, typeof(GameObject), true) as GameObject;

        YPosition = EditorGUILayout.FloatField("Hauteur de création des cubes", YPosition);

        if (GUILayout.Button("Create Cube"))
        {
            CreateCube();
        };

        GUILayout.EndArea();
    }

    /// <summary>
    /// Random des angles des GameObjects sélectionnés
    /// </summary>
    public void CreateLayout()
    {
        foreach (var selectedObject in Selection.gameObjects)
        {
            selectedObject.transform.rotation = Quaternion.Euler(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f));
        }
    }
    /// <summary>
    /// Reset rotation des GameObjects sélectonnés
    /// </summary>
    private void ResetRotation()
    {
        foreach (var selectedObject in Selection.gameObjects)
        {
            selectedObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    /// <summary>
    /// Reset le Transform des GameObjects sélectonnées
    /// </summary>
    private void BResetTransform()
    {
        foreach (var selectedObject in Selection.gameObjects)
        {
            selectedObject.transform.rotation = Quaternion.Euler(0,0,0);
            selectedObject.transform.position = new Vector3(0,0,0);
        }
    }

    /// <summary>
    /// Enables/disable
    /// </summary>
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }


    //Création de cubes
    private void CreateCube()
    {
        //Le cube est sélectionné on peut le placer ou on veut sur la map
        GameObject cube = Instantiate(source);
        cube.GetComponent<Renderer>().sharedMaterial.color = Color.black;
        MeshFilter filter = cube.GetComponent<MeshFilter>();
        Mesh mesh = filter.mesh;

        cube.transform.position = new Vector3(0f,0f,0f);
    }

   /// <summary>
   /// Détruire tous les GameObjects qui ont été crées par l'outil
   /// </summary>
    private void ResetInstantiatedGo()
    {
        foreach(GameObject go in instantiatedGo)
        {
            DestroyImmediate(go, true);
        }
        instantiatedGo = new List<GameObject>();
    }

    /// <summary>
    /// Lorsque le joueur est sur la sene
    /// </summary>
    /// <param name="sceneView"></param>
    private void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();
        if (canSelectable)
        {
            InstantiatePrimitive(eventType);

        }

        Handles.EndGUI();

    }

    /// <summary>
    /// Instantier le prefab en fonction du mode de draw
    /// Envoie d'un raycast et position du prefab par ce dernier
    /// </summary>
    /// <param name="evt"></param>
    private void InstantiatePrimitive(EventType evt)
    {
        EventType currEventType = evt;
        Debug.Log(currEventType);

        //Le cube est sélectionné on peut le placer ou on veut sur la map
        if (Event.current.type == currEventType)
        {
            Ray worlray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitIfos;
            if (Physics.Raycast(worlray, out hitIfos, 10000f))
            {

                if (hitIfos.collider.name == "Floor")
                {
                    GameObject obj = Instantiate(source);

                    Vector3 gridPos = new Vector3(Mathf.Round(hitIfos.point.x), Mathf.Round(hitIfos.point.y), Mathf.Round(hitIfos.point.z));
                    obj.transform.position = gridPos;

                    //obj.transform.position = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).GetPoint(distanceToDraw);
                    instantiatedGo.Add(obj);
                }
            }
        }       
    }
}
