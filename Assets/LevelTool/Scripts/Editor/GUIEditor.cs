#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

[InitializeOnLoad]
public static class GUIEditor
{
    static GUIEditor()
    {
        // Register a callback to draw the toolbar
        ToolbarExtender.RightToolbarGUI.Add(DrawCustomButtons);
    }

    private static void DrawCustomButtons()
    {
        if (GUILayout.Button("GamePlay UI", GUILayout.Width(100)))
        {
            OpenScene("Assets/_GameResources/Scenes/GameSceneUI.unity");
        }
        if (GUILayout.Button("GamePlay", GUILayout.Width(100)))
        {
            OpenScene("Assets/_GameResources/Scenes/GameScene.unity");
        }
        if (GUILayout.Button("Level Editor", GUILayout.Width(100)))
        {
            OpenScene("Assets/_GameResources/Scenes/LevelEditor.unity");
        }
        if (GUILayout.Button("Level Test", GUILayout.Width(100)))
        {
            OpenScene("Assets/_GameResources/Scenes/LevelTest.unity");
        }
    }

    private static void OpenScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}
#endif

