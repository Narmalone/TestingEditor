using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using Unity.VisualScripting;

public class LevelEditor : EditorWindow
{
    #region Création de la window
    [MenuItem("Tools/Level Editor")]
    public static void OpenWindow()
    {
        var Window = EditorWindow.GetWindow<LevelEditor>();
        Window.titleContent = new GUIContent("Level Editor");
    }
    #endregion


    #region Basic Variables

    /// <summary>
    /// Pour docs UXML
    /// </summary>
    TemplateContainer treeAssetObject;
    TemplateContainer treeAssetPrefabs;

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

    private Vector2 scrollPos;
    string t = "Dossiers Prefabs";

    private int index;


    List<Button> directoryButtons = new List<Button>();

    ObjectField obj;
    #endregion

    #region OnEnable/OnDisable
    private void OnEnable()
    {
        GenerateObjectCategory();
        GeneratePrefabsCategory();
        DisplayCategory(treeAssetObject, treeAssetPrefabs);
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }


    #endregion

    #region Unity Gui functions
    private void OnSceneGUI(SceneView sceneView)
    {
        if (canSelectable)
        {
            if (source == null)
            {

                Debug.LogWarning("Mettez un prefab avant de continuer !");
                return;
            }
            //InstantiatePrefab(eventType);
            //if (previewObject != null) PreviewSource(previewObject);
        }

        if (Event.current.keyCode == KeyCode.A)
        {
            RotateFromScrollWheel(-7.5f, previewObject);
            RotateFromScrollWheel(-7.5f, objTaille);
        }
        if (Event.current.keyCode == KeyCode.E)
        {
            RotateFromScrollWheel(7.5f, previewObject);
            RotateFromScrollWheel(7.5f, objTaille);
        }

        //CheckPreviewMesh(previewObject);

    }
    #endregion

    #region GenerateUI / Categories Functions
    public void GenerateButtons(int count, VisualElement parent, List<Button> listToAddBtns, params string[] btnsName)
    {
        for (int i = 0; i < count; i++)
        {
            Button btn = new Button();
            listToAddBtns.Add(btn);

            if (btnsName[i] != null)
            btn.name = btnsName[i];

            parent.Add(btn);
        }
    }
    public void GenerateObjectCategory()
    {
        rootVisualElement.name = "EditorContainer";
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/LevelEditorWindow.uxml");
        treeAssetObject = original.CloneTree();
        treeAssetObject.name = "OriginalTreeAsset";
        rootVisualElement.Add(treeAssetObject);
        List<Button> ButtonsCategories = new List<Button>();
        List<Button> ButtonsFunctionsCategories = new List<Button>();

        VisualTreeAsset ve_categoriesContainer = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UxmlEditor/VE_ParentCategory.uxml");

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/LevelEditorStylesSheet.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        //Boucle for pour créer les Catégories
        for (int i = 0; i < 5; i++)
        {
            //Ajouter les parents Visual Element qui continnent les Visual elements
            VisualElement parentsCat = ve_categoriesContainer.CloneTree();

            //Renommer les Container générés
            parentsCat.name = "CategoryContainer_" + i.ToString();

            VisualElement LabelCategory = parentsCat.Q<VisualElement>("VE_LabelContainer");
            LabelCategory.name = "LabelCategory" + i.ToString();

            VisualElement BodyCategory = parentsCat.Q<VisualElement>("VE_CategoriesContainer");
            BodyCategory.name = "BodyCategory" + i.ToString();


            //Les ajouter au rootVisualElement
            treeAssetObject.Add(parentsCat);

            //Créer les Label du nom des catégories
            Label labCategory = new Label();
            labCategory.style.unityFontStyleAndWeight = FontStyle.BoldAndItalic;
            labCategory.style.fontSize = 24;
            labCategory.name = "Lab_" + i.ToString();

            if (i == 0)
            {
                labCategory.text = "CATEGORIES";
                labCategory.style.paddingTop = 10;
                BodyCategory.style.flexDirection = FlexDirection.Row;
                GenerateButtons(2, BodyCategory, ButtonsCategories, "B_OBJECTS", "B_PREFABS");

                //Query les boutons générés que l'on veut et set leurs données
                ButtonsCategories[0].text = "OBJECTS";
                ButtonsCategories[1].text = "PREFABS";

                ButtonsCategories[0].clickable.clicked += DisplayObjectsCategory;
                ButtonsCategories[1].clickable.clicked += DisplayPrefabsCategory;

                //Set coding Styles
                foreach (Button btn in ButtonsCategories)
                {
                    btn.style.width = 250;
                    btn.style.height = 50;
                    btn.style.fontSize = 20;
                }
            }
            else if (i == 1)
            {
                labCategory.text = "LEVEL FUNCTIONS";
                labCategory.style.paddingTop = 30;
                BodyCategory.style.flexDirection = FlexDirection.Column;
                GenerateButtons(2, BodyCategory, ButtonsFunctionsCategories, "B_DESTROY_ALL_GAMEOBJECTS_SELECTED", "B_DESTROY_LAST_GAMEOBJECT_PLACED");

                ButtonsFunctionsCategories[0].text = "DESTROY ALL GAMEOBJECTS SELECTED";
                ButtonsFunctionsCategories[1].text = "DESTROY LAST GAMEOBJECT PLACED";
            }
            else if (i == 2)
            {
                labCategory.text = "TRANSFORMS";
                labCategory.style.paddingTop = 30;
                BodyCategory.style.flexDirection = FlexDirection.Column;
                GenerateButtons(2, BodyCategory, ButtonsFunctionsCategories, "B_RESET_TRANSFORMS", "B_RESET_ROTATIONS");

                ButtonsFunctionsCategories[2].text = "Reset Transfroms of Selected GameObjects";
                ButtonsFunctionsCategories[2].clickable.clicked += ResetTransforms;

                ButtonsFunctionsCategories[3].text = "Reset Rotations of SelectedGameObjects";
                ButtonsFunctionsCategories[3].clickable.clicked += ResetRotations;
            }
            else if(i == 3)
            {
                labCategory.text = "CREATE OBJECTS";
                labCategory.style.paddingTop = 30;
                BodyCategory.style.flexDirection = FlexDirection.Row;

                //Ajouter les labels au VisualElement Parent qui contient les labels
                // ----------------------- OBJECT FIELD ----------------------- \\

                //Créer constructeur
                obj = new ObjectField("Prefab");

                //Ajouter l'object field au rootVisualElement
                BodyCategory.Add(obj);

                //Set le nom de l'object field au cas ou
                obj.name = "PrefabField";

                //Autoriser le fait que l'on puisse prendre des objets de la scene -> non
                obj.allowSceneObjects = false;

                //Atribbuer le type d'objet que l'on souhaite instancier -> GameObject
                obj.objectType = typeof(GameObject);

                //définir max Width de tout le visual Element
                obj.style.width = 300;

                //Querry le premier Label
                Label labObj = obj.Q<Label>();
                labObj.name = "LabelBeforeField";

                //Querry l'image qu'il y'a à l'intérieur de l'object field container
                Image imgObj = obj.Q<Image>();
                imgObj.name = "PrefabTexture";

                //Get source AssetDataBasegetPreview and set la texture de l'image obj

                //Quand il y'a un changement registercallback de la valeur à la nouvelle valeure
                obj.RegisterCallback<ChangeEvent<Object>>((evt) =>
                {
                    obj.value = evt.newValue;
                });
            }

            //Définitions styles des bouttons Categories Functions crées pour qu'ils soient tous les même
            foreach (Button btn in ButtonsFunctionsCategories)
            {
                btn.style.width = 350;
                btn.style.height = 40;
                btn.style.fontSize = 15;
            }
            LabelCategory.Add(labCategory);
        }

        //Vu qu'on est dans la catégorie Object on met le texte du bouton en cyan
        ButtonsCategories[0].style.color = Color.cyan;
        treeAssetObject.style.display = DisplayStyle.None;
    }
    public void GeneratePrefabsCategory()
    {
        rootVisualElement.name = "EditorContainer";
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/LevelEditorPrefabWindow.uxml");
        treeAssetPrefabs = original.CloneTree();
        treeAssetPrefabs.name = "OriginalTreeAsset";
        rootVisualElement.Add(treeAssetPrefabs);

        List<Button> ButtonsCategories = new List<Button>();
        List<Button> ButtonsFunctionsCategories = new List<Button>();
        VisualTreeAsset ve_categoriesContainer = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UxmlEditor/VE_ParentCategory.uxml");

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/LevelEditorStylesSheet.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        //Boucle for pour créer les Catégories
        for (int i = 0; i < 2; i++)
        {
            //Ajouter les parents Visual Element qui continnent les Visual elements
            VisualElement parentsCat = ve_categoriesContainer.CloneTree();

            //Renommer les Container générés
            parentsCat.name = "CategoryContainer_" + i.ToString();

            VisualElement LabelCategory = parentsCat.Q<VisualElement>("VE_LabelContainer");
            LabelCategory.name = "LabelCategory" + i.ToString();

            VisualElement BodyCategory = parentsCat.Q<VisualElement>("VE_CategoriesContainer");
            BodyCategory.name = "BodyCategory" + i.ToString();


            //Les ajouter au rootVisualElement
            treeAssetPrefabs.Add(parentsCat);

            //Créer les Label du nom des catégories
            Label labCategory = new Label();
            labCategory.style.unityFontStyleAndWeight = FontStyle.BoldAndItalic;
            labCategory.style.fontSize = 24;
            labCategory.name = "Lab_" + i.ToString();

            if (i == 0)
            {
                labCategory.text = "CATEGORIES";
                labCategory.style.paddingTop = 10;
                BodyCategory.style.flexDirection = FlexDirection.Row;
                GenerateButtons(2, BodyCategory, ButtonsCategories, "B_OBJECTS", "B_PREFABS");

                //Query les boutons générés que l'on veut et set leurs données
                ButtonsCategories[0].text = "OBJECTS";
                ButtonsCategories[1].text = "PREFABS";

                ButtonsCategories[0].clickable.clicked += DisplayObjectsCategory;
                ButtonsCategories[1].clickable.clicked += DisplayPrefabsCategory;

                //Set coding Styles
                foreach (Button btn in ButtonsCategories)
                {
                    btn.style.marginTop = 10;
                    btn.style.width = 250;
                    btn.style.height = 50;
                    btn.style.fontSize = 20;
                }

                //Ajouter les labels au VisualElement Parent qui contient les labels
                LabelCategory.Add(labCategory);
            }
            else if(i == 1)
            {
                labCategory.text = "PREFABS FOLDER";
                labCategory.style.marginTop = 30;
                labCategory.style.marginBottom = 20;

                VisualElement scrollviewAndPrefabs = new VisualElement();
                scrollviewAndPrefabs.name = "ScrollViewAndPrefabs";
                scrollviewAndPrefabs.style.width = 1000f;
                scrollviewAndPrefabs.style.flexWrap = Wrap.NoWrap;
                scrollviewAndPrefabs.style.alignItems = Align.Stretch;
                scrollviewAndPrefabs.style.justifyContent = Justify.FlexStart;
                scrollviewAndPrefabs.style.flexDirection = FlexDirection.Row;
                scrollviewAndPrefabs.style.height = 1000f;

                //Faire une scoll view
                VisualElement scrollViewContainer = new VisualElement();
                scrollViewContainer.name = "ScrollViewContainer";

                //Définir size et autre de la scrollView ou il y'a les dossiers
                ScrollView scrollDirectories = new ScrollView(ScrollViewMode.Vertical);
                scrollDirectories.style.width = 250;
                scrollDirectories.style.height = 700f;
                scrollDirectories.style.flexDirection = FlexDirection.Column;
                scrollDirectories.style.flexWrap = Wrap.NoWrap;



                VisualElement scrollViewContainer2 = new VisualElement();
                scrollViewContainer2.name = "ScrollViewContainer2";

                //Faire une scoll view
                ScrollView scrollPrefabs = new ScrollView(ScrollViewMode.Vertical);
                scrollPrefabs.style.width = 500;
                scrollPrefabs.style.height = 700f;
                scrollPrefabs.style.flexDirection = FlexDirection.Column;
                scrollPrefabs.style.flexWrap = Wrap.NoWrap;

                //Ajouter les ScrollViews aux containers
                scrollViewContainer.Add(scrollDirectories);
                scrollViewContainer2.Add(scrollPrefabs);

                //Ajouter les containers au root qui permet de les afficher en lignes
                scrollviewAndPrefabs.Add(scrollViewContainer);
                scrollviewAndPrefabs.Add(scrollViewContainer2);

                //Ajouter le root au rootvisual
                BodyCategory.Add(scrollviewAndPrefabs);

                //Récupérer les dossiers dans le dossier Prefab
                string[] directories = Directory.GetDirectories("Assets/Prefabs");

                foreach (string dir in directories)
                {
                    //Créer bouttons
                    Button btn = new Button();

                    //Si bouton cliqué on assigne source gameobject au prefab cliqué
                    btn.clickable.clicked += () =>
                    {
                        directorySelected = dir;
                        RefreshPrefabs(directorySelected, scrollPrefabs);
                    };

                    //Styles des boutons crées
                    btn.text = dir;
                    btn.style.marginRight = 30;
                    btn.style.height = 30;

                    //Ajouter le bouton à scrollView Directories
                    scrollDirectories.Add(btn);
                }

            }

            LabelCategory.Add(labCategory);
            //Vu que on est dans catégorie préfab on peut mettre son texte en cyan
            ButtonsCategories[1].style.color = Color.cyan;
            treeAssetPrefabs.style.display = DisplayStyle.None;
        }


     
    }
    private void RefreshPrefabs(string dir, VisualElement scrollViewToSetPrefabsButtons)
    {
        //ok voir pourquoi ça s'instantie 2 fois
        if (Directory.Exists(dir))
        {
            if(directoryButtons.Count > 0)
            {
                foreach (Button button in directoryButtons)
                {
                    VisualElement ToRemove = scrollViewToSetPrefabsButtons.Q<Button>("btnFiles");
                    ObjectField ToRemoveField = scrollViewToSetPrefabsButtons.Q<ObjectField>("PrefabAsset");

                    scrollViewToSetPrefabsButtons.Remove(ToRemove);
                    scrollViewToSetPrefabsButtons.Remove(ToRemoveField);
                }
                directoryButtons = new List<Button>();
            }

            string[] files = Directory.GetFiles(dir, "*.prefab");

            //faire comme pour les visual element btnFiles et tout retirer à chaques fois
            foreach (string f in files)
            {
                //Récupérer chaques prefabs depuis leur path
                UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath(f, typeof(GameObject));

                //Object field pour chacuns des fichiers
                //Créer constructeur
                ObjectField obj = new ObjectField("Prefab Asset: ");

                //Ajouter l'object field au rootVisualElement
                scrollViewToSetPrefabsButtons.Add(obj);

                //Set le nom de l'object field au cas ou
                obj.name = "PrefabAsset";

                //Autoriser le fait que l'on puisse prendre des objets de la scene -> non
                obj.allowSceneObjects = false;

                //Atribbuer le type d'objet que l'on souhaite instancier -> GameObject
                obj.objectType = typeof(GameObject);
                obj.value = prefab;

                //définir max Width de tout le visual Element
                obj.style.width = 300;

                //Styles
                Button btn = new Button();
                btn.style.backgroundImage = AssetPreview.GetAssetPreview(prefab);
                btn.style.marginBottom = 30;
                btn.name = "btnFiles";
                btn.style.unityFontStyleAndWeight = FontStyle.BoldAndItalic;
                btn.text = "CLICK";
                btn.clickable.clicked += () =>
                {
                    SetNewPrefabSelected(prefab);
                };

                //Styles des boutons crées
                btn.style.marginTop = 5;
                btn.style.height = 100;
                btn.style.width = 100;

                directoryButtons.Add(btn);

                //Set les prefabs buttons ici
                scrollViewToSetPrefabsButtons.Add(btn);
            }
        }
    }
    private void SetNewPrefabSelected(Object prefab)
    {
        source = prefab.GameObject();
        obj.value = source;
        Debug.Log(source.name);
    }

    private void RefreshPreviewObject(object p)
    {
        throw new System.NotImplementedException();
    }

    public void DisplayCategory(VisualElement veToShow, VisualElement veToHide)
    {
        veToShow.style.display = DisplayStyle.Flex;
        veToHide.style.display = DisplayStyle.None;
    }
    public void DisplayObjectsCategory()
    {
        DisplayCategory(treeAssetObject, treeAssetPrefabs);
        Debug.Log("Display la catégorie OBJECTS");
    }

    public void DisplayPrefabsCategory()
    {
        DisplayCategory(treeAssetPrefabs, treeAssetObject);
        Debug.Log("display prefab catégorie");
    }
    #endregion

    #region Level Functions
    //Fonction détruire tous les objets placés
    //Fonction détruire le dernier objet placé
    //Fonction détruire tous les objets sélectionnés ? <- problématique avec notre liste ?
    #endregion
    #region Prefab Functions
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

    #region Transform Functions
    /// <summary>
    /// Reset rotation des GameObjects sélectonnés
    /// </summary>
    private void ResetRotations()
    {
        Debug.Log("Reset Rotations");
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
    private void ResetTransforms()
    {
        Debug.Log("Reset Transforms");
        foreach (var selectedObject in Selection.gameObjects)
        {
            selectedObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            selectedObject.transform.position = new Vector3(0, 0, 0);
        }
    }
    #endregion
}
