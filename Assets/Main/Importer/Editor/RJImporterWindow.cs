using System.IO;
using UnityEditor;
using UnityEngine;

public class RJImporterWindow : EditorWindow
{
    public string assetPath;

    [MenuItem("Window/RJ/Importer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(RJImporterWindow));
    }

    public class TextureLoader
    {
        public string assetPath;
        public string baseName;
        public Material material;

        public void LoadTexture(string field, string ending, TextureImporterType type)
        {
            string texturePath = "Assets/" + assetPath + "/" + baseName + ending;
            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            importer.textureType = TextureImporterType.NormalMap;
            Texture2D texture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
            if (texture != null)
            {
                material.SetTexture(field, texture);
            }
            else
            {
                Debug.Log("Missing texture for " + texturePath);
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Label("RJ Importer", EditorStyles.boldLabel);
        assetPath = EditorGUILayout.TextField("Path", assetPath);
        if (GUILayout.Button("Import Materials"))
        {
            // path = EditorUtility.OpenFolderPanel("Texture Directory", path, "");
            Debug.Log("Opening file " + assetPath);
            foreach(string filePath in Directory.GetFiles(Path.Combine(Application.dataPath, assetPath)))
            {
                TextureLoader textureLoader = new TextureLoader();
                textureLoader.assetPath = assetPath;

                string fileName = Path.GetFileName(filePath);
                string ending = "_col.jpg";
                if (fileName.EndsWith("_col.jpg"))
                {
                    textureLoader.baseName = fileName.Substring(0, fileName.Length - ending.Length);
                    textureLoader.material = new Material(Shader.Find("Standard"));
                    textureLoader.LoadTexture("_MainTex", "_col.jpg", TextureImporterType.Default);
                    textureLoader.LoadTexture("_BumpMap", "_nrm.jpg", TextureImporterType.NormalMap);
                    textureLoader.LoadTexture("_ParallaxMap", "_disp.jpg", TextureImporterType.NormalMap);
                    textureLoader.LoadTexture("_MetallicGlossMap", "_disp.jpg", TextureImporterType.NormalMap);
                    AssetDatabase.CreateAsset(textureLoader.material, "Assets/" + textureLoader.baseName + ".mat");
                    Debug.Log("Created Asset " + "Assets/" + textureLoader.baseName + ".mat");
                }
            }
        }
    }
}
