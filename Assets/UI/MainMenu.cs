using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

public class MainMenu : EditorWindow
{
    [MenuItem("Window/UI Toolkit/MainMenu")]
    public static void ShowExample()
    {
        MainMenu wnd = GetWindow<MainMenu>();
        wnd.titleContent = new GUIContent("MainMenu");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scenes/MainMenu.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scenes/MainMenu.uss");
        VisualElement labelWithStyle = new Label("Hello World! With Style");
        labelWithStyle.styleSheets.Add(styleSheet);
        root.Add(labelWithStyle);


        Button button = root.Q<Button>("SinglePlayer");

        OnSinglePlayerDelegate singlePlayerDelegate = OnSinglePlayer;
        button.clickable.clickedWithEventInfo += singlePlayerDelegate;
    }
    public delegate void OnSinglePlayerDelegate(Action<EventBase> e);
    public static void OnSinglePlayer(Action<EventBase> e)
    {
        Debug.Log("SinglePlayer");
    }
}