using UnityEditor;
using UnityEngine;

public class VoxelEditorWindow : EditorWindow
{
    [SerializeField]
    private VoxelSurface voxelSurface;

    [SerializeField]
    private int size;

    [SerializeField]
    private bool cut;

    [MenuItem("Window/RJ/VoxelEditorWindow")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(VoxelEditorWindow));
    }

    void OnGUI()
    {
        GUILayout.Label("Terrain Editor Settings", EditorStyles.boldLabel);
    }
}
