using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;
using System.IO;
using Unity.VisualScripting;
using System.Linq;

public class EditorWindowTest : EditorWindow
{
    #region Création de la window
    [MenuItem("Window/EditorWindowTest")]
    public static void OpenWindow()
    {
        EditorWindow.GetWindow<EditorWindowTest>("EditorWindowTest");
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
    /// Va permettre d'activer ou désactiver la "grille"
    /// </summary>
    private bool Snapping;

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
    public Material materialSave;
    public Color objColor;

    private bool ActivePreview = false;

    private List<GameObject> SaveObjects = new List<GameObject>();

    private GUIStyle labelStyle;

    /// <summary>
    /// Quand quelqu'un clique sur une des catégorie, stocker l'infos dans un index
    /// </summary>
    private int CategoryIndex = 0;

    /// <summary>
    /// Scrollview quand index == 1
    /// </summary>
    private Vector2 scrollViewPos;

    /// <summary>
    /// Variable qui sert à récupérer le bon dossier quand on clique sur le bouton
    /// </summary>
    private string directorySelected;

    /// <summary>
    /// Si un prefab seul est sélectionné on peut y activer la preview
    /// </summary>
    private bool isPrevievable;


    /// <summary>
    /// Get la taille du preview object
    /// </summary>
    private float previewHalfHeight;
    private float previewHalfWidth;
    private float previewHalfLenght;


    //la distance ou va etre instancié l'obet qui calcule la taille de la preview
    private float DistObjTaille = 2000f;

    //objet qui calcule la taille de la prevew
    private GameObject objTaille;

    //sauvegarde de la rotation quand on place un prefab
    private Vector3 sauvegardeRot;


    //premier objet qu'on touche
    RaycastHit FirstHit;
    //distance de base de la distance qu'on check
    float distFromCam = 10000f;

    /// <summary>
    /// dernier objet posé sur la scene
    /// </summary>
    private GameObject lastObject;



    /// <summary>
    /// Sauvegarde la rotation quand on reset le prevew object
    /// </summary>

    #endregion

    #region Inspector's GUI
    private void OnGUI()
    {
        //Déclarations des variables et propriétés permettant de controller les texts, bouttons ect...
        var stylestate = new GUIStyleState();

        GUILayoutOption buttonWidth = GUILayout.Width(200f);
        GUILayoutOption buttonHeight = GUILayout.Height(30f);

        //Varaible pour toucher à tout ce qu'il y'a dans les boutons
        var CategoriesButtons = new GUIStyle(GUI.skin.button);
        CategoriesButtons.fontSize = 16;
        CategoriesButtons.alignment = TextAnchor.MiddleCenter;
        CategoriesButtons.onHover = stylestate;
        CategoriesButtons.normal.textColor = Color.yellow;

        var FunctionsButtons = new GUIStyle(GUI.skin.button);
        FunctionsButtons.fontSize = 16;
        FunctionsButtons.alignment = TextAnchor.MiddleCenter;
        FunctionsButtons.onHover = stylestate;
        FunctionsButtons.normal.textColor = Color.white;
        GUILayoutOption functionButtonWidth = GUILayout.Width(400f);

        var fontStyles = new GUIStyle(GUI.skin.label);
        fontStyles.fontSize = 16;
        fontStyles.fontStyle = FontStyle.BoldAndItalic;
        fontStyles.normal.textColor = Color.white;
        //faire marcher le onhover
        fontStyles.onFocused.textColor = Color.blue;

        var functionsTextStyles = new GUIStyle(GUI.skin.label);
        functionsTextStyles.fontSize = 8;
        functionsTextStyles.fontStyle = FontStyle.Italic;
        functionsTextStyles.normal.textColor = Color.white;
        //faire marcher le onhover


        //Catégories, servent à changer de fenêtres dans l'inspecteur
        GUILayout.BeginArea(new Rect(0f, 10f, 200f, 100f));

        GUILayout.Label("CATEGORIES", fontStyles);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("OBJECTS", CategoriesButtons, buttonWidth, buttonHeight))
        {
            CategoryIndex = 0;
        };

        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(250f, 33f, 200f, 100f));

        if (GUILayout.Button("PREFABS", CategoriesButtons, buttonWidth, buttonHeight))
        {
            CategoryIndex = 1;
        }
        GUILayout.EndHorizontal();

        GUILayout.EndArea();


        if (CategoryIndex == 0)
        {

            GUILayout.BeginArea(new Rect(0f, 100f, 400f, 100f));

            GUILayout.Label("LEVEL FUNCTIONS", fontStyles);

            //Détruire la map (Foutre le bouton en rouge)
            if (GUILayout.Button("DESTROY ALL GAMEOBJECTS PLACED", CategoriesButtons))
            {
                ResetInstantiatedGo();
            };

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0f, 160f, 400f, 100f));

            if (GUILayout.Button("DESTROY LAST OBJECT PLACED", CategoriesButtons))
            {
                ResetLastInstance();
            };

            GUILayout.EndArea();

            //Tous les boutons liés aux transforms (rotate l'objet par exemple ou reset son transform)
            GUILayout.BeginArea(new Rect(0f, 200f, 500f, 300f));

            GUILayout.Label("CHANGES", fontStyles);


            if (GUILayout.Button("SAVE CHANGES", FunctionsButtons, functionButtonWidth, buttonHeight))
            {
                SaveChanges();
            };
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0f, 260f, 500f, 300f));

            if (GUILayout.Button("DISCARD UNSAVED CHANGES", FunctionsButtons, functionButtonWidth, buttonHeight))
            {
                DiscardChanges();
            };

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0f, 320f, 500f, 300f));
            GUILayout.Label("TRANFORMS", fontStyles);

            //Reset transform des objets sélectionnés
            if (GUILayout.Button("Reset Transform of Selected GameObjects", FunctionsButtons, functionButtonWidth, buttonHeight))
            {
                BResetTransform();
            };
            //Reset que la rotation des objets sélectionnés
            if (GUILayout.Button("Reset Rotation of Selected GameObjects", FunctionsButtons, functionButtonWidth, buttonHeight))
            {
                ResetRotation();
            };

            GUILayout.EndArea();

            #region Create Objects
            //Tout ce qui est lié à la création des objets donc le mode par lequel on dessine, création de la preview ect...
            GUILayout.BeginArea(new Rect(0f, 430f, 600f, 300f));
            GUILayout.Label("CREATE OBJECTS", fontStyles);

            eventType = (EventType)EditorGUILayout.EnumFlagsField("DRAWING MODE", eventType);

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

            Snapping = EditorGUILayout.Toggle("Activer le Snap", Snapping);

            GUILayout.EndArea();

            //------------------------ PREFAB SECTION ------------------------\\
            GUILayout.BeginArea(new Rect(0, 700, 600f, 200f));


            GUILayout.Label("PREVIEW", fontStyles);
            source = EditorGUILayout.ObjectField("Prefab", source, typeof(GameObject), true) as GameObject;

            if (isPrevievable = EditorGUILayout.Toggle("Material preview can be displayed", isPrevievable))
            {
                materialPreview = EditorGUILayout.ObjectField("MaterialPreview", materialPreview, typeof(Material), true) as Material;
                if (previewObject != null)
                {
                    if (materialPreview != null)
                    {
                        previewObject.GetComponent<Renderer>().sharedMaterial = materialPreview;
                    }
                }

            }
            else
            {
                if (previewObject != null)
                    previewObject.GetComponent<Renderer>().sharedMaterial = materialSave;
                if (materialPreview != null)
                {

                }
            }

            if (GUILayout.Button("Activate Preview", FunctionsButtons, functionButtonWidth, buttonHeight))
            {
                GeneratePreview(source);
            };
            if (GUILayout.Button("Reset Preview", FunctionsButtons, functionButtonWidth, buttonHeight))
            {
                ResetePreviewVisual();
            };

            if (source != null)
            {
                Rect rt = GUILayoutUtility.GetRect(100, 100);
                EditorGUI.DrawPreviewTexture(new Rect(rt.x, rt.y, 100, 100), AssetPreview.GetAssetPreview(source));
            }

            GUILayout.EndArea();
            #endregion
        }
        else
        {
            //Chercher tous les sous-dossiers dans le dossier Prefabs
            string[] directories = Directory.GetDirectories("Assets/Prefabs");

            //mettre boutons à l'horizontal en header
            GUILayout.BeginArea(new Rect(0f, 70f, 1500f, 200f));
            GUILayout.BeginHorizontal(GUILayout.Width(1500f));

            //Chaques dossiers dans le tableau ont un bouton
            foreach (string dir in directories)
            {
                if (GUILayout.Button(dir, GUILayout.Width(200f), GUILayout.Height(50f)))
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
            if (Directory.Exists(directorySelected)) DisplayPrefabSelect(directorySelected);

            //Lancer la fonction afin ici afin qu'elle s'update en fonction du dossier sélecionné

            GUILayout.EndArea();

        }

    }
    #endregion

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
            if (source == null)
            {

                Debug.LogWarning("Mettez un prefab avant de continuer !");
                return;
            }
            InstantiatePrefab(eventType);
            if (previewObject != null) PreviewSource(previewObject);
        }

        if (Event.current.keyCode == KeyCode.A)
        {
            Debug.Log("a pressed");
            RotateFromScrollWheel(-7.5f, previewObject);
            RotateFromScrollWheel(-7.5f, objTaille);
        }
        if (Event.current.keyCode == KeyCode.E)
        {
            Debug.Log("E pressed");
            RotateFromScrollWheel(7.5f, previewObject);
            RotateFromScrollWheel(7.5f, objTaille);
        }
        CheckPreviewMesh(previewObject);


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
        if (previewObject != null)
        {
            previewObject.transform.rotation = Quaternion.identity;
        }
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

    #region ResetInstantiatedObject
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


    /// <summary>
    /// reset juste la derniere instance crée
    /// 
    /// mais ça marche pas bien
    /// </summary>
    private void ResetLastInstance()
    {
        Debug.Log("destry last");
        GameObject go = instantiatedGo.Last();
        instantiatedGo.Remove(instantiatedGo.Last());
        DestroyImmediate(go, true);
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

                //set up de l'objet pour check la taille
                if (objTaille != null) DestroyImmediate(objTaille);
                objTaille = Instantiate(previewObject);
                objTaille.transform.position = new Vector3(DistObjTaille, DistObjTaille, DistObjTaille);
                instantiatedGo.Add(objTaille);
                materialSave = previewObject.GetComponent<Renderer>().sharedMaterial;
                previewObject.GetComponent<Renderer>().sharedMaterial = materialPreview;
                if (previewObject.GetComponent<Renderer>().sharedMaterial == null)
                {
                    return;
                }
            }
        }
        else
        {
            DestroyImmediate(previewObject);
            ActivePreview = false;
        }
    }



    /// <summary>
    /// permet de reset la taille de l'objet quand on change de prefab
    /// </summary>
    private void ResetObjectStats()
    {
        //set up de l'objet pour check la taille
        if (objTaille != null) DestroyImmediate(objTaille);
        objTaille = Instantiate(previewObject);
        objTaille.transform.position = new Vector3(DistObjTaille, DistObjTaille, DistObjTaille);
        instantiatedGo.Add(objTaille);
    }

    /// <summary>
    /// reset preview quand on place un block
    /// </summary>
    /// <param name="source"></param>
    public void ResetePreview(GameObject source)
    {
        if (canSelectable)
        {
            if (source == null) return;
            previewObject = Instantiate(source);
            previewObject.name = "Preview";
            previewObject.transform.rotation = Quaternion.Euler(sauvegardeRot);
            ActivePreview = true;

            ResetObjectStats();

            if (previewObject.GetComponent<Renderer>())
            {
                previewObject.GetComponent<Renderer>().sharedMaterial = materialPreview;

            }
        }
    }


    /// <summary>
    /// reset sans supprimer le nouveau prefab
    /// </summary>
    private void ResetePreviewVisual()
    {

        if (canSelectable)
        {

            DestroyImmediate(previewObject);

            if (source == null) return;
            previewObject = Instantiate(source);
            previewObject.name = "Preview";
            previewObject.transform.rotation = Quaternion.Euler(sauvegardeRot);
            ActivePreview = true;

            if (objTaille != null) DestroyImmediate(objTaille);
            objTaille = Instantiate(previewObject);
            objTaille.transform.position = new Vector3(DistObjTaille, DistObjTaille, DistObjTaille);


            //ResetObjectStats();

            if (previewObject.GetComponent<Renderer>() != null)
            {
                previewObject.GetComponent<Renderer>().sharedMaterial = materialPreview;

            }


            CheckPreviewMesh(previewObject);


        }
    }

    /// <summary>
    /// Va servir a detecter la hauteur/ largeur d'un objet
    /// </summary>
    /// <param name="obj">objet de la preview qu'on veut récuperer</param>
    private void CheckPreviewMesh(GameObject obj)
    {

        //créer un raycast en dessous du block
        //Ray worldRayUp = new Ray(obj.transform.position - Vector3.up * DistRay, obj.transform.position);
        Ray worldRayUp = new Ray(new Vector3(DistObjTaille, DistObjTaille - 50f, DistObjTaille), Vector3.up);

        //créer un raycast en dessous du block
        Ray worldRayRight = new Ray(new Vector3(DistObjTaille - 50f, DistObjTaille, DistObjTaille), Vector3.right);

        //créer un raycast en dessous du block
        Ray worldRayForward = new Ray(new Vector3(DistObjTaille, DistObjTaille, DistObjTaille - 50f), Vector3.forward);

        RaycastHit hit;

        if (Physics.Raycast(worldRayUp, out hit, Mathf.Infinity))
        {

            if (hit.collider.transform.parent != null)
                previewHalfHeight = Vector3.Dot(hit.point - hit.transform.parent.position, hit.normal);
            else
            {
                previewHalfHeight = Vector3.Dot(hit.point - hit.transform.position, hit.normal);
            }
        }

        if (Physics.Raycast(worldRayRight, out hit, Mathf.Infinity))
        {
            if (hit.collider.transform.parent != null)
                previewHalfWidth = Vector3.Dot(hit.point - hit.transform.parent.position, hit.normal);
            else
            {
                previewHalfWidth = Vector3.Dot(hit.point - hit.transform.position, hit.normal);

            }
        }
        if (Physics.Raycast(worldRayForward, out hit, Mathf.Infinity))
        {
            if (hit.collider.transform.parent != null)
                previewHalfLenght = Vector3.Dot(hit.point - hit.transform.parent.position, hit.normal);
            else
            {
                previewHalfLenght = Vector3.Dot(hit.point - hit.transform.position, hit.normal);

            }
        }
        /**/
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
        previewObject.transform.rotation = Quaternion.Euler(sauvegardeRot);
        materialSave = previewObject.GetComponent<Renderer>().sharedMaterial;
        previewObject.GetComponent<Renderer>().sharedMaterial = materialPreview;
        ActivePreview = true;
    }


    /// <summary>
    /// Prévisualisation de la préview en fonction des blocs avec lesquels on collide
    /// </summary>
    /// <param name="source"></param>
    public void PreviewSource(GameObject source)
    {

        /// <summary>
        /// si snapping est activé, va arrondir les valeurs pour que les objets se placent sur une grille
        /// </summary>
        if (Snapping)
        {
            //récupérer position de la souris dans l'espace 3D
            //Ray worldRay = ;
            RaycastHit[] hits;
            hits = Physics.RaycastAll(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), 100.0F);

            //si la listeest vide, ne vas pas plus loins
            if (hits.Length <= 0) return;

            //on initialise une premiere fois avec le dernier element hit
            FirstHit = hits[0];
            distFromCam = hits[0].distance;


            /// <summary>
            /// on va check l'objet le plus proche de la camera et le garder dans "firstHit"
            /// </summary>
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.name != "Preview")
                {

                    //détecte si un des parents est preview
                    if (hits[i].collider.transform.parent != null)
                    {

                        if (hits[i].collider.transform.parent.name == "Preview")
                        {
                            Debug.Log("un parent de vu wola");
                            break;


                        }
                        if (hits[i].collider.transform.root.name == "Preview")
                        {
                            Debug.Log("un parent de vu wola");
                            break;
                        }
                    }

                    //si l'objet touché est le sol on ne vérifie pas de parents
                    if (hits[i].collider.name == "Floor")
                    {
                        if (distFromCam > hits[i].distance)
                        {
                            distFromCam = hits[i].distance;
                            FirstHit = hits[i];
                        }
                    }
                    else
                    {

                        if (hits[i].collider.transform.parent == null)
                        {
                            if (distFromCam > hits[i].distance)
                            {
                                distFromCam = hits[i].distance;
                                FirstHit = hits[i];
                                CalculatePreviewSnapped(FirstHit.point, FirstHit.transform.position, FirstHit.normal);

                            }
                        }
                        //si l'obje a des parents
                        if (hits[i].collider.transform.parent != null)
                        {
                            if (distFromCam > hits[i].distance)
                            {
                                distFromCam = hits[i].distance;
                                FirstHit = hits[i];
                            }
                            CalculatePreviewSnapped(FirstHit.point, FirstHit.transform.root.position, FirstHit.normal);

                        }
                    }

                }
            }

            if (FirstHit.transform.name == "Floor")
            {
                Vector3 posToTexture = new Vector3(Mathf.Round(FirstHit.point.x), FirstHit.point.y + previewHalfHeight, Mathf.Round(FirstHit.point.z));
                source.transform.position = posToTexture;
            }

        }



        /// <summary>
        /// si snapping n'est pas activé
        /// </summary>
        else
        {
            //récupérer position de la souris dans l'espace 3D
            //Ray worldRay = ;
            RaycastHit[] hits;
            hits = Physics.RaycastAll(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), 100.0F);

            //si la listeest vide, ne vas pas plus loins
            if (hits.Length <= 0) return;

            //on initialise une premiere fois avec le dernier element hit
            FirstHit = hits[0];
            distFromCam = hits[0].distance;


            /// <summary>
            /// on va check l'objet le plus proche de la camera et le garder dans "firstHit"
            /// </summary>
            for (int i = 0; i < hits.Length; i++)
            {


                if (hits[i].collider.name != "Preview")
                {



                    if (hits[i].collider.transform.root.name != "Preview")
                    {


                        Debug.Log(hits[i].collider.transform.root.transform.name);


                        if (distFromCam > hits[i].distance)
                        {
                            distFromCam = hits[i].distance;
                            FirstHit = hits[i];
                            Debug.Log(hits[i].collider.transform.root.transform.name);
                            CalculatePreview(FirstHit.transform.root.position);

                        }
                    }

                }
            }

            //si n'a pas de parent, prends directement l'objet
            if (FirstHit.collider.transform.parent == null)
            {

                CalculatePreview(FirstHit.transform.position);

            }
            else
            {
                if (FirstHit.collider.transform.root.transform.name == "Preview") return;

                CalculatePreview(FirstHit.collider.transform.root.transform.position);


            }

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hitPoint">la position du contact du raycast avec l'objet</param>
    /// <param name="objectPos">la position de l'objet touché ou du parent</param>
    /// <param name="hitNormal">normal du hitpoint</param>
    private void CalculatePreviewSnapped(Vector3 hitPoint, Vector3 objectPos, Vector3 hitNormal)
    {


        Vector3 newPosition;

        //sert a récuperer le milieu de la face touchée , et de faire la disance 
        float dist = Vector3.Dot(hitPoint - objectPos, hitNormal);

        //la nouvelle position = la pos de l'objet + la normale de la face multiplié par la distance entre le point de la normale et le centre de l'objet, + la largeur / hauter / longeur du prefab             hit.normal.y * dist + previewHalfHeight, hit.normal.z * dist + previewHalfWidth
        newPosition = objectPos + hitNormal * dist + new Vector3(hitNormal.x * previewHalfWidth, hitNormal.y * previewHalfHeight, hitNormal.z * previewHalfLenght);
        previewObject.transform.position = newPosition;


    }

    /// <summary>
    /// Sert a calculer la position de la preview quand on ne snap pas a partir de la position du parent du hit.collider
    /// </summary>
    /// <param name="ray">Posiion de l'objet qu'on touche ou du parent</param>
    private void CalculatePreview(Vector3 ray)
    {
        //si il n'y a qu'un objet et qu'il s'app preview, return
        if (FirstHit.collider.name == "Preview") return;
        Vector3 newPosition;

        //sert a récuperer le milieu de la face touchée , et de faire la disance 
        float dist = Vector3.Dot(FirstHit.point - FirstHit.transform.position, FirstHit.normal);

        //la nouvelle position = la pos de l'objet + la normale de la face multiplié par la distance entre le point de la normale et le centre de l'objet, + la largeur / hauter / longeur du prefab             hit.normal.y * dist + previewHalfHeight, hit.normal.z * dist + previewHalfWidth
        newPosition = FirstHit.point + new Vector3(FirstHit.normal.x * previewHalfWidth, FirstHit.normal.y * previewHalfHeight, FirstHit.normal.z * previewHalfLenght);
        previewObject.transform.position = newPosition;
    }



    /// <summary>
    /// change la rotation en fonction du Y de la scroll wheel
    /// </summary>
    /// <param name="val">Valeur Y de la scroll wheel qui sert a savoir si on scroll en haut ou en bas</param>
    private void RotateFromScrollWheel(float val, GameObject obj)
    {
        if (Event.current.type == EventType.MouseMove) return;
        if (val < 0)
        {
            obj.transform.rotation = Quaternion.Euler(obj.transform.rotation.eulerAngles.x, obj.transform.rotation.eulerAngles.y + val, obj.transform.rotation.eulerAngles.z);
            Debug.Log("l'object bouge en +");
        }
        if (val > 0)
        {
            Debug.Log("l'object bouge en +");
            obj.transform.rotation = Quaternion.Euler(obj.transform.rotation.eulerAngles.x, obj.transform.rotation.eulerAngles.y + val, obj.transform.rotation.eulerAngles.z);
        }

        sauvegardeRot = obj.transform.rotation.eulerAngles;
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
        //Debug.Log(currEventType);

        //Le cube est sélectionné on peut le placer ou on veut sur la map
        if (Event.current.type == currEventType)
        {
            //return si le bouton clické n'est pas left click
            Event e = Event.current;
            if (e.button != 0 && e.isMouse) return;

            Ray worlray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitIfos;

            //crée le prefab a la place de la preview
            if (Physics.Raycast(worlray, out hitIfos, Mathf.Infinity))
            {
                GameObject go = previewObject;
                if (previewObject != null)
                {
                    sauvegardeRot = go.transform.rotation.eulerAngles;
                }
                if (go == null) return;
                go.name = "Wall";
                instantiatedGo.Add(go);
                if (go.GetComponent<Renderer>())
                {
                    go.GetComponent<Renderer>().sharedMaterial = materialSave;

                }
                go.transform.rotation = Quaternion.Euler(sauvegardeRot);

                lastObject = go;
                ResetePreview(source);

            }
        }


    }

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
    #endregion

    #region Changes Functions
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

    #endregion
}
