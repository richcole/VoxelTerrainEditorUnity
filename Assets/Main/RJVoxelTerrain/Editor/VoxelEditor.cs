using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoxelSurface))]
public class VoxelEditor : Editor {

    void OnSceneGUI()
    {
        VoxelSurface voxelSurface = target as VoxelSurface;

        FUtils.Elvis(voxelSurface.GetComponentInParent<GridArray>(), gridArray => 
        {
            gridArray.Update();
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (gridArray != null)
                    {
                        gridArray.Hit(hit.point);
                    }
                }
            }
        });
    }

    public override void OnInspectorGUI()
    {
        VoxelSurface voxelSurface = (VoxelSurface)target;
        voxelSurface.size = EditorGUILayout.IntField("Size", voxelSurface.size);
        voxelSurface.drawGizmos = EditorGUILayout.Toggle("Draw Gizmos", voxelSurface.drawGizmos);

        if (voxelSurface.entityField != null)
        {
            EditorGUILayout.IntField ("Number of Entities", voxelSurface.entityField.entities.Count);
        }

        if (voxelSurface.navMesh != null)
        {
            EditorGUILayout.IntField("NavMesh positions Count", voxelSurface.navMesh.positions.Count);
            EditorGUILayout.IntField("NavMesh connections Count", voxelSurface.navMesh.connections.Count);
        }
        else
        {
            EditorGUILayout.LabelField("No nav mesh");
        }

        if (GUILayout.Button("Initialize"))
        {
            voxelSurface.Initialize();
        }

        if (GUILayout.Button("Create Own Mesh"))
        {
            voxelSurface.CreateOwnMesh();
        }
    }
}
