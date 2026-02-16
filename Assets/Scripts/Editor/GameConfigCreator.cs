using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor utility to create the GameConfig ScriptableObject asset if missing.
/// Menu: Tools → Create GameConfig Asset
/// </summary>
public static class GameConfigCreator
{
    [MenuItem("Tools/Create GameConfig Asset")]
    public static void CreateAsset()
    {
        const string path = "Assets/Resources/Config/GameConfig.asset";

        if (AssetDatabase.LoadAssetAtPath<GameConfig>(path) != null)
        {
            Debug.Log("GameConfig already exists at " + path);
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameConfig>(path);
            return;
        }

        // Ensure folder exists
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Config"))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.CreateFolder("Assets/Resources", "Config");
        }

        GameConfig config = ScriptableObject.CreateInstance<GameConfig>();
        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("GameConfig asset created at " + path);
        Selection.activeObject = config;
        EditorUtility.FocusProjectWindow();
    }
}
