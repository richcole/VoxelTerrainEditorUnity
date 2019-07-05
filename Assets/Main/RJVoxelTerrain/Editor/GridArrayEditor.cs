using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[CustomEditor(typeof(GridArray))]
public class GridArrayEditor : Editor {

    void OnSceneGUI()
    {
    }

    public override void OnInspectorGUI()
    {
        GridArray gridArray = target as GridArray;
        gridArray.sizeXZ = EditorGUILayout.IntField("Size XZ", gridArray.sizeXZ);
        gridArray.sizeY = EditorGUILayout.IntField("Size Y", gridArray.sizeY);
        gridArray.height = EditorGUILayout.IntField("Height", gridArray.height);
        gridArray.scale = EditorGUILayout.FloatField("Scale", gridArray.scale);
        gridArray.perlinScale = EditorGUILayout.FloatField("Perlin Scale", gridArray.perlinScale);
        gridArray.perlinFreq = EditorGUILayout.FloatField("Perlin Freq", gridArray.perlinFreq);
        gridArray.pointDensity = EditorGUILayout.IntField("Point Density", gridArray.pointDensity);
        gridArray.voxelSurfacePrototype = EditorGUILayout.ObjectField(
            "voxelSurfacePrototype", gridArray.voxelSurfacePrototype, typeof(VoxelSurface), false) as VoxelSurface;
        gridArray.editMode = (EditMode)EditorGUILayout.EnumPopup(gridArray.editMode);

        gridArray.buildRadius = EditorGUILayout.FloatField("Build Radius", gridArray.buildRadius);
        gridArray.material = EditorGUILayout.ObjectField(
            "material", gridArray.material, typeof(Material), false) as Material;
        gridArray.materialScale = EditorGUILayout.FloatField("Material Scale", gridArray.materialScale);
        EditorGUILayout.LabelField("HasNavMesh " + (gridArray.navMesh != null));
        if (gridArray.navMesh != null)
        {
            EditorGUILayout.LabelField("connections=" + gridArray.navMesh.connections.Count);
            EditorGUILayout.LabelField("positions=" + gridArray.navMesh.positions.Count);
        }

        if (gridArray.path != null)
        {
            EditorGUILayout.LabelField("path length=" + gridArray.path.Count);
        }
        else
        {
            EditorGUILayout.LabelField("no path");
        }

        if (GUILayout.Button("Initialize"))
        {
            gridArray.Initialize();
        }

        if (GUILayout.Button("Generate"))
        {
            gridArray.Generate();
        }

        if (GUILayout.Button("Set Material"))
        {
            gridArray.SetMaterial();
        }

        if (GUILayout.Button("Grow Tree"))
        {
            FindObjectOfType<TreeGrower>().GrowTree();
        }

        gridArray.UpdateMesh();
    }
}
