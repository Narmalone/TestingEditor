using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
using System.IO;
using Unity.VisualScripting;

public class EditorWindowTest : EditorWindow
{
    #region Création de la window
    [MenuItem("Window/EditorTest")]
    public static void OpenWindow()
    {
        EditorWindow.GetWindow<EditorWindowTest>("EditorTest");
    }
    #endregion

    #region Basic Variables
    /// <summary>
    /// Prefab à instantier et son preview
    /// </summary>
    static public GameObject source;
    public GameObject previewObject;

    /// <summary>
    /// Si on utilise le draw
    /// </summary>
    private bool canSelectable;

    /// <summary>
    /// Récupérer les types d'events de Unity
    /// </summary>
    public EventType eventType;

    /// <summary>
    /// Les objets que l'on créer s'incrémentent dans la liste puis quand on détruits la map ça comprend tous les objets dedans
    /// </summary>
    List<GameObject> instantiatedGo = new List<GameObject>();

    /// <summary>
    /// Changer le Material pour la preview
    /// </summary>
    public Material materialPreview;
    public Color objColor;

    private bool ActivePreview = false;

    private List<GameObject> SaveObjects = new List<GameObject> ();

    private GUIStyle labelStyle;

    /// <summary>
    /// Quand quelqu'un clique sur une des catégorie, stocker l'infos dans un index
    /// </summary>
    private int CategoryIndex = 0;

    /// <summary>
    /// Scrollview quand index == 1
    /// </summary>
    private Vector2 scrollViewPos;

    private string directorySelected;
    #endregion

    #region Inspector's GUI
    private void OnGUI()
    {
        //Catégories, servent à changer de fenêtres dans l'inspecteur
        GUILayout.BeginArea(new Rect(0f, 10f, 600, 100f));
        GUILayout.Label("CATEGORIES", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Selected Objects", GUILayout.Width(200f)))
        {
            CategoryIndex = 0;
        };
        if(GUILayout.Button("Tous les Prefabs", GUILayout.Width(200f)))
        {
            CategoryIndex = 1;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (CategoryIndex == 0)
        {
           
            GUILayout.BeginArea(new Rect(0f, 60f, 300f, 100f));
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.yellow;
            //Détruire la map (Foutre le bouton en rouge)
            if (GUILayout.Button("DESTROY ALL GAMEOBJECTS PLACED", style))
            {
                ResetInstantiatedGo();
            };
            GUILayout.EndArea();
            //Tous les boutons liés aux transforms (rotate l'objet par exemple ou reset son transform)
            GUILayout.BeginArea(new Rect(0f, 100f, 500f, 300f));
            GUILayout.Label("TRANFORMS", EditorStyles.boldLabel);

            //Reset transform des objets sélectionnés
            if (GUILayout.Button("Reset Transform of GameObjects"))
            {
                BResetTransform();
            };
            //Reset que la rotation des objets sélectionnés
            if (GUILayout.Button("Reset Rotation only of GameObjects"))
            {
                ResetRotation();
            };

            if (GUILayout.Button("DISCARD UNSAVED CHANGES"))
            {
                DiscardChanges();
            };

            if (GUILayout.Button("SAVE CHANGES"))
            {
                SaveChanges();
            };
            GUILayout.EndArea();


            //Tout ce qui est lié à la création des objets donc le mode par lequel on dessine, création de la preview ect...
            GUILayout.BeginArea(new Rect(0f, 250f, 600f, 500f));
            GUILayout.Label("CREATE OBJECTS", EditorStyles.boldLabel);

            eventType = (EventType)EditorGUILayout.EnumFlagsField("Mode de draw", eventType);

            switch (eventType)
            {
                case EventType.MouseDown:
                    InstantiatePrefab(eventType);
                    break;
                case EventType.MouseMove:
                    InstantiatePrefab(eventType);
                    break;
            }

            canSelectable = EditorGUILayout.Toggle("Peut créer des cubes ?", canSelectable);

            source = EditorGUILayout.ObjectField("Prefab", source, typeof(GameObject), true) as GameObject;
            materialPreview = EditorGUILayout.ObjectField("MaterialPreview", materialPreview, typeof(Material), true) as Material;

            if (GUILayout.Button("Activate Preview"))
            {
                GeneratePreview(source);
            };

            if (source != null)
            {
                Rect rt = GUILayoutUtility.GetRect(100, 100);
                EditorGUI.DrawPreviewTexture(new Rect(rt.x, rt.y, 100, 100), AssetPreview.GetAssetPreview(source));
            }
            GUILayout.EndArea();
        }
        else
        {
            //Chercher tous les sous-dossiers dans le dossier Prefabs
            string[] directories = Directory.GetDirectories("Assets/Prefabs");

            //mettre boutons à l'horizontal en header
            GUILayout.BeginArea(new Rect(0f, 50f, 1500f, 200f));
            GUILayout.BeginHorizontal(GUILayout.Width(1500f));

            //Chaques dossiers dans le tableau ont un bouton
            foreach (string dir in directories)
            {
                if(GUILayout.Button(dir, GUILayout.Width(200f), GUILayout.Height(50f)))
                {
                    //Attribuer le dossier sélectionné
                    directorySelected = dir;
                }
            }
            GUILayout.EndArea();
            GUILayout.EndHorizontal();

            GUILayout.BeginArea(new Rect(0f, 100f, 500f, 800f));

            //Si la string dossier sélectionné == null on return
            if (directorySelected == null) return;
            if(Directory.Exists(directorySelected)) DisplayPrefabSelect(directorySelected);

            //Lancer la fonction afin ici afin qu'elle s'update en fonction du dossier sélecionné

            GUILayout.EndArea();
        
        }

    }
    #endregion
         
    /// <summary>
    /// Envoyer le path du dossier, récupérer tous ses prefabs et les afficher
    /// </summary>
    /// <param name="dir"></param>
    public void DisplayPrefabSelect(string dir)
    {
        scrollViewPos = GUILayout.BeginScrollView(scrollViewPos, GUILayout.Width(500f), GUILayout.Height(800f));

        //Si le dossier est détruit on return la création des dossiers
        
        string[] files = Directory.GetFiles(dir, "*.prefab");
        foreach (string f in files)
        {
            //Récupérer chaques prefabs depuis leur path
            UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath(f, typeof(GameObject));

            //Chacuns des prefabs ont une petite place qui leur est attribué
            Rect rt = GUILayoutUtility.GetRect(50f, 50f);

            //On tous les prefabs là - mettre une submail plus tard
            prefab = EditorGUILayout.ObjectField("Prefab asset: " + prefab.name, prefab, typeof(GameObject), true) as GameObject;

            //Lancer les fonctions quand le joueur clique sur le nouvel asset
            if (GUILayout.Button(AssetPreview.GetAssetPreview(prefab)))
            {
                //Changer la source à instancier -> Prefab
                SetNewPrefabSelected(prefab);

                //Si le joueur ne peut pas créer de cube blc de la preview
                if (!canSelectable) return;

                //On refresh complètement la preview du joueur et on converti le UnityEngine.Object en Gameobject par une fonction de VisualScripting
                RefreshPreviewObject(prefab.GameObject());
            }
        }
        GUILayout.EndScrollView();
    }
    /// <summary>
    /// Envoyer la nouvelle source obj et transformer en gameobject
    /// </summary>
    /// <param name="obj"></param>
    public void SetNewPrefabSelected(UnityEngine.Object obj)
    {
        source = obj.GameObject();
        Debug.Log(source.name);
    }

    #region On Scene GUI
    /// <summary>
    /// Lorsque le joueur est sur la sene
    /// </summary>
    /// <param name="sceneView"></param>
    private void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();

        if (canSelectable)
        {
            if (source == null) {

                Debug.LogWarning("Mettez un prefab avant de continuer !");
                return;
            }
            InstantiatePrefab(eventType);
            if (previewObject != null) PreviewSource(previewObject);
        }

        Handles.EndGUI();

    }
    #endregion

    #region Transform Functions
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
            selectedObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            selectedObject.transform.position = new Vector3(0, 0, 0);
        }
    }
    #endregion

    #region OnEnable/OnDisable
    /// <summary>
    /// Enables/disable
    /// </summary>
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    #endregion

    #region ResetAllInstantiatedObject
    /// <summary>
    /// Détruire tous les GameObjects qui ont été crées par l'outil
    /// </summary>
    private void ResetInstantiatedGo()
    {
        foreach (GameObject go in instantiatedGo)
        {
            Debug.Log(instantiatedGo.Count);
            DestroyImmediate(go, true);
        }
        instantiatedGo = new List<GameObject>();
    }
    #endregion

    #region Previews Functions

    /// <summary>
    /// Source équivaut au prefab, fonction appelée uniquement quand on clique sur le bouton Generate Preview
    /// </summary>
    /// <param name="source"></param>
    public void GeneratePreview(GameObject source)
    {
        if (!ActivePreview)
        {
            if (canSelectable)
            {
                if (source == null) return;
                previewObject = Instantiate(source);
                previewObject.name = "Preview";
                ActivePreview = true;
                if (previewObject.GetComponent<Renderer>().sharedMaterial == null)
                {
                    return;
                }
                previewObject.GetComponent<Renderer>().sharedMaterial = materialPreview;
                Debug.Log(ActivePreview);
            }
        }
        else
        {
            DestroyImmediate(previewObject);
            ActivePreview = false;
        }
    }

    /// <summary>
    /// Fonctions qui permet de refresh la preview de l'object actuel (Ne marche pas encore !!)
    /// Faire en sorte que la preview se refresh et pas juste la source (Attention à la destruction de GO/Assets)
    /// </summary>
    /// <param name="newSource"></param>
    public void RefreshPreviewObject(GameObject newSource)
    {
        if (source == null) return;
        source = newSource;

        ActivePreview = false;

        DestroyImmediate(previewObject);

        previewObject = Instantiate(source);
        previewObject.name = "Preview";
        previewObject.GetComponent<Renderer>().sharedMaterial = materialPreview;
        ActivePreview = true;
    }
    /// <summary>
    /// Prévisualisation de la préview en fonction des blocs avec lesquels on collide
    /// </summary>
    /// <param name="source"></param>
    public void PreviewSource(GameObject source)
    {
        //récupérer position de la souris dans l'espace 3D
        Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;

        //Checker si on est sur le sol si oui Afficher la preview
        if (Physics.Raycast(worldRay, out hit, Mathf.Infinity))
        {
            //Si le gameobject touché s'appelle Floor
            if (hit.collider.name == "Floor")
            {
                //Si oui on set la position de l'objet par rapport au raycast hit
                Vector3 posToTexture = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
                source.transform.position = posToTexture;
            }
            //Si le gameobject ne s'appelle pas preview
            else if (hit.collider.name != "Preview")
            {
                //différence entre la position du hit du raycast et le centre de l'objet qu'on touche
                float difX = hit.transform.position.x - hit.point.x;
                float difY = hit.transform.position.y - hit.point.y;
                float difZ = hit.transform.position.z - hit.point.z;

                //future position du cube qu'on va placer
                Vector3 newPosition;

                ///va récupérer la face sur la quelle on clique et augmenter la valeur de la position pour poser le block collé a celle ci
                //si on clique sur l'axe x du cube
                if (difX >= 0.5f)
                {
                    Debug.Log(difX);
                    newPosition = new Vector3(Mathf.Round(hit.point.x - 0.5f), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
                    previewObject.transform.position = newPosition;

                }
                else if (difX <= -0.5f)
                {
                    newPosition = new Vector3(Mathf.Round(hit.point.x + 0.5f), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
                    previewObject.transform.position = newPosition;

                }
                //si on clique sur l'axe Z du cube
                if (difZ >= 0.5f)
                {
                    newPosition = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z - 0.5f));
                    previewObject.transform.position = newPosition;
                }
                else if (difZ <= -0.5f)
                {
                    newPosition = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z + 0.5f));
                    previewObject.transform.position = newPosition;
                }

                //si on clique l'axe Y du cube
                if (difY >= 0.5f)
                {
                    newPosition = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y - 0.5f), Mathf.Round(hit.point.z));
                    previewObject.transform.position = newPosition;
                }
                else if (difY <= -0.5f)
                {
                    newPosition = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y + 0.5f), Mathf.Round(hit.point.z));
                    previewObject.transform.position = newPosition;
                }

            }
        }

    }
    #endregion

    #region PrefabsFunction
    /// <summary>
    /// Instantier le prefab en fonction du mode de draw
    /// Envoie d'un raycast et position du prefab par ce dernier
    /// </summary>
    /// <param name="evt"></param>
    private void InstantiatePrefab(EventType evt)
    {
        EventType currEventType = evt;

        //Le cube est sélectionné on peut le placer ou on veut sur la map
        if (Event.current.type == currEventType)
        {
            Ray worlray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitIfos;
            if (Physics.Raycast(worlray, out hitIfos, Mathf.Infinity))
            {

                if (hitIfos.collider.name == "Floor" || hitIfos.collider.name == "Preview")
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
    #endregion


    //Récupérer la liste et à chaques modifs modifier tableau
    //Save quand on bool le créer cube
    public override void DiscardChanges()
    {

        Debug.Log($"{this} discarded changes!!!");
        base.DiscardChanges();
    }

    public override void SaveChanges()
    {
        // Your custom save procedures here

        Debug.Log($"{this} saved successfully!!!");
        base.SaveChanges();
    }
}
