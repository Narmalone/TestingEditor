using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

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

    private void OnEnable()
    {
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/LevelEditorWindow.uxml");
        TemplateContainer treeAsset = original.CloneTree();
        rootVisualElement.Add(treeAsset);

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/LevelEditorStylesSheet.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
    }

}
