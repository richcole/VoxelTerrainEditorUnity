using UnityEngine;
using UnityEditor.Experimental.AssetImporters;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;

[Serializable]
public class RJVector
{
    public List<float> p;
    public List<int> groups;
}

[Serializable]
public class RJFace
{
    public int index;
    public int materialIndex;
    public RJVector normal;
    public List<int> vertexIndexes;
}

[Serializable]
public class RJUVFace
{
    public List<RJVector> uvList;
}

[Serializable]
public class RJUVLayer
{
    public string name;
    public List<RJUVFace> faces;
}

[Serializable]
public class RJMaterial
{
    public string name;
}

[Serializable]
public class RJMesh
{
    public List<RJFace> faces;
    public List<RJUVLayer> uvLayers;
    public List<RJMaterial> materials;
    public List<RJVector> vertices;
}

[Serializable]
public class RJGroup
{
    public int index;
    public String name;
}

[Serializable]
public class RJObj
{
    public List<RJGroup> groups;
    public RJMesh mesh;
    public string name;
}

[ScriptedImporter(1, "rj")]
public class RJImporter : ScriptedImporter
{
    public float m_Scale = 1;

    public static Vector3 toVector3(List<float> floats)
    {
        return new Vector3(floats[0], floats[1], floats[2]);
    }

    public static Vector3 toVector3(RJVector vector)
    {
        return toVector3(vector.p);
    }

    public static Vector2 toVector2(List<float> floats)
    {
        return new Vector2(floats[0], floats[1]);
    }

    public static Vector2 toVector2(RJVector vector)
    {
        return toVector2(vector.p);
    }

    public static int getGroupIndex(List<RJGroup> groups, String groupName)
    {
        foreach(RJGroup group in groups) {
            if (group.name == groupName)
            {
                return group.index;
            }
        }
        return -1;
    }

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var text = File.ReadAllText(ctx.assetPath);
        var rjObj  = JsonUtility.FromJson<RJObj>(text);
        var rjMesh = rjObj.mesh;
        var rjVertices = rjMesh.vertices;

        List<Material> materialList = new List<Material>();
        var mesh = new Mesh();

        mesh.name = rjObj.name + "Mesh";
        mesh.subMeshCount = rjMesh.materials.Count;
        
        int rootIndex = getGroupIndex(rjObj.groups, "Root");
        int attachPointIndex = getGroupIndex(rjObj.groups, "AttachPoint");

        var mV = new List<Vector3>();
        var mN = new List<Vector3>();
        var mUVA = new List<List<Vector2>>();
        var mEA = new List<List<int>>();

        for (int layerIndex = 0; layerIndex < rjMesh.uvLayers.Count; ++layerIndex)
        {
            mUVA.Add(new List<Vector2>());
        }

        Vector3 rootVector = Vector3.zero;
        Vector3 attachPointVector = Vector3.zero;
        foreach (RJVector vector in rjMesh.vertices)
        {
            if (vector.groups.Contains(rootIndex))
            {
                rootVector = toVector3(vector.p);
                Debug.Log("Root Vector: " + rootVector);
            }
            if (vector.groups.Contains(attachPointIndex))
            {
                attachPointVector = toVector3(vector.p);
                Debug.Log("Attach Point: " + attachPointVector);
            }
        }

        for (int materialIndex=0; materialIndex<rjMesh.materials.Count; ++materialIndex) {
            RJMaterial rjMaterial = rjMesh.materials[materialIndex];
            string materialPath = "Assets/" + rjMaterial.name + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath("Assets/Space/Materials/" + rjMaterial.name + ".mat", typeof(Material)) as Material;
            if (material == null)
            {
                throw new Exception("Cannot load material " + materialPath);
            }
            materialList.Add(material);

            var mE = new List<int>();

            foreach (int faceIndex in Enumerable.Range(0, rjMesh.faces.Count))
            {
                RJFace face = rjMesh.faces[faceIndex];

                if (face.materialIndex != materialIndex)
                {
                    continue;
                }

                List<int> vs = face.vertexIndexes;
                int baseIndex = mV.Count;
                foreach (int vIndex in face.vertexIndexes)
                {
                    mV.Add(toVector3(rjVertices[vIndex]));
                    mN.Add(toVector3(face.normal));
                }

                if (vs.Count == 3)
                {
                    mE.Add(baseIndex + 0);
                    mE.Add(baseIndex + 1);
                    mE.Add(baseIndex + 2);

                }
                else if (vs.Count == 4)
                {
                    mE.Add(baseIndex + 0);
                    mE.Add(baseIndex + 1);
                    mE.Add(baseIndex + 2);

                    mE.Add(baseIndex + 0);
                    mE.Add(baseIndex + 2);
                    mE.Add(baseIndex + 3);
                }
                else
                {
                    throw new Exception("Unexpected number of verticies in face count=" + vs.Count);
                }

                for(int layerIndex=0; layerIndex<rjMesh.uvLayers.Count; ++layerIndex) 
                {
                    RJUVLayer uvLayer = rjMesh.uvLayers[layerIndex];
                    var uvList = uvLayer.faces[faceIndex].uvList;

                    if (uvList.Count != vs.Count)
                    {
                        throw new Exception("Diffing verticies and uvLists vCount=" + vs.Count + " uvList.Count=" + uvList.Count);
                    }
                    foreach (RJVector rjVector in uvList)
                    {
                        mUVA[layerIndex].Add(toVector2(rjVector));
                    }
                }
            }
            mEA.Add(mE);

        }

        mV = FUtils.Map(mV, v => v - rootVector);
        mesh.SetVertices(mV);
        mesh.SetNormals(mN);

        for (int layerIndex = 0; layerIndex < rjMesh.uvLayers.Count; ++layerIndex)
        {
            mesh.SetUVs(layerIndex, mUVA[layerIndex]);
        }

        for(int materialIndex=0; materialIndex < rjMesh.materials.Count; ++materialIndex)
        {
            mesh.SetTriangles(mEA[materialIndex], materialIndex);
        }

        GameObject gameObject = new GameObject(rjObj.name);

        GameObject attachPoint = new GameObject("AttachPoint");
        attachPoint.transform.parent = gameObject.transform;
        attachPoint.transform.localPosition = attachPointVector;

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.materials = materialList.ToArray();

        ctx.AddObjectToAsset("mesh", mesh);
        ctx.AddObjectToAsset("main", gameObject);

        ctx.SetMainObject(gameObject);
    }
}