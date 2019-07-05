using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder
{
    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uv = new List<Vector2>();
    List<int> triangles = new List<int>();
    float materialScale = 1.0f;

    public MeshBuilder(float materialScale)
    {
        this.materialScale = materialScale;
    }

    public Vector2 ComputeUV1(Vector3 a)
    {
        Vector3 n = a.normalized;
        float u = (float)(Math.Atan2(n.x, n.z) / (2f * Math.PI) + 0.5f);
        float v = n.y * 0.5f + 0.5f;
        return new Vector2(u, v);
    }

    public Vector2 ComputeUV2(Vector3 a)
    {
        return new Vector2(a.x, a.z);
    }

    public Vector2 ComputeUV3(Vector3 a)
    {
        return new Vector2(a.x + 0.2f * a.y, a.z + 0.3f * a.y);
    }

    public void AddVertex(Vector3 a)
    {
        uv.Add(ComputeUV3(a) * materialScale);
        vertices.Add(a);
    }

    public void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        int index = vertices.Count;

        AddVertex(a);
        AddVertex(b);
        AddVertex(c);

        triangles.Add(index);
        triangles.Add(index + 1);
        triangles.Add(index + 2);
    }

    public void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector4 d)
    {
        int index = vertices.Count;

        AddVertex(a);
        AddVertex(b);
        AddVertex(c);
        AddVertex(d);

        triangles.Add(index);
        triangles.Add(index + 1);
        triangles.Add(index + 2);

        triangles.Add(index + 2);
        triangles.Add(index + 3);
        triangles.Add(index);
    }

    public void CopyToMesh(Mesh mesh)
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

}
