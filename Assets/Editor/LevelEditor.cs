using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using static Unity.VisualScripting.Member;

public class LevelEditor : EditorWindow
{
    #region Création de la window
    [MenuItem("Tools/Level Editor")]
    public static void OpenWindow()
    {
        var Window = EditorWindow.GetWindow<LevelEditor>();
        Window.titleContent = new GUIContent("Level Editor");
        Window.minSize = new Vector2(800, 600);
    }
    #endregion


    #region Basic Variables
    private int indexCategory = 0;

    TemplateContainer treeAssetObject;
    TemplateContainer treeAssetPrefabs;

    static public GameObject source;
    #endregion

    private void OnEnable()
    {
        GenerateObjectCategory();
        GeneratePrefabsCategory();
        DisplayCategory(treeAssetObject, treeAssetPrefabs);

        Debug.Log("On Enable");
    }
    public void GenerateButtons(int count, VisualElement parent, List<Button> listToAddBtns, params string[] btnsName)
    {
        for (int i = 0; i < count; i++)
        {
            Button btn = new Button();
            listToAddBtns.Add(btn);

            if (btnsName[i] != null) ;
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
                GenerateButtons(2, BodyCategory, ButtonsFunctionsCategories, "B_DESTROY_ALL_GAMEOBJECTS_PLACED", "B_DESTROY_LAST_GAMEOBJECT_PLACED");

                ButtonsFunctionsCategories[0].text = "DESTROY ALL GAMEOBJECTS PLACED";
                ButtonsFunctionsCategories[1].text = "DESTROY LAST GAMEOBJECT PLACED";
            }
            else if (i == 2)
            {
                labCategory.text = "TRANSFORMS";
                labCategory.style.paddingTop = 30;
                BodyCategory.style.flexDirection = FlexDirection.Column;
                GenerateButtons(2, BodyCategory, ButtonsFunctionsCategories, "B_RESET_TRANSFORMS", "B_RESET_ROTATIONS");

                ButtonsFunctionsCategories[2].text = "Reset Transfroms of Selected GameObjects";
                ButtonsFunctionsCategories[3].text = "Reset Rotations of SelectedGameObjects";
            }
            else if(i == 3)
            {
                labCategory.text = "CREATE OBJECTS";
                labCategory.style.paddingTop = 30;
                BodyCategory.style.flexDirection = FlexDirection.Row;

                ObjectField obj = new ObjectField("Prefab");
                obj.value = source;
                BodyCategory.Add(obj);

            }


            foreach (Button btn in ButtonsFunctionsCategories)
            {
                btn.style.width = 350;
                btn.style.height = 40;
                btn.style.fontSize = 15;
            }
            //Ajouter les labels au VisualElement Parent qui contient les labels
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
                    btn.style.width = 250;
                    btn.style.height = 50;
                    btn.style.fontSize = 20;
                }

                //Ajouter les labels au VisualElement Parent qui contient les labels
                LabelCategory.Add(labCategory);
            }

            //Vu que on est dans catégorie préfab on peut mettre son texte en cyan
            ButtonsCategories[1].style.color = Color.cyan;
            treeAssetPrefabs.style.display = DisplayStyle.None;
        }
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
}
