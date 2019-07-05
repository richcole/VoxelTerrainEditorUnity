using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UpdateMeshJob
{
    public EntityField entityField;
    public MeshBuilder meshBuilder;
    public NavMesh navMesh;
    public int size;

    static int[] EDGES = {
            0, 1, 2,
            2, 0, 1,
            1, 2, 0
        };

    static CellIndex[] DIMS = {
            new CellIndex(1, 0, 0),
            new CellIndex(0, 1, 0),
            new CellIndex(0, 0, 1)
    };

    public UpdateMeshJob(EntityField entityField, int size, float materialScale)
    {
        this.entityField = entityField;
        this.size = size;
        this.meshBuilder = new MeshBuilder(materialScale);
        this.navMesh = new NavMesh();
    }

    public void Execute()
    {
        Dictionary<CellIndex, Cell> cellCenters = new Dictionary<CellIndex, Cell>();

        ForEachCell1(size, v =>
        {
            Vector3 vp = GetPosition(size, v);
            ForEachEdge((d1, d2, d3) =>
            {
                CellIndex d1d2 = d1 + d2;
                Vector3 e = vp + GetPosition(size, d1d2) * .5f;
                Vector3 e1 = e + GetPosition(size, d3) * .5f;
                Vector3 e2 = e - GetPosition(size, d3) * .5f;

                int sign = entityField.GetMidpointSign(e1, e2);
                if (sign != 0)
                {
                    Vector3 midPoint = entityField.GetMidPoint(e1, e2);
                    AddCellCenter(cellCenters, midPoint, v);
                    AddCellCenter(cellCenters, midPoint, v + d1);
                    AddCellCenter(cellCenters, midPoint, v + d1d2);
                    AddCellCenter(cellCenters, midPoint, v + d2);
                }
            });
        });

        ForEachCell2(size, v =>
        {
            Vector3 vp = GetPosition(size, v);
            ForEachEdge((d1, d2, d3) =>
            {
                CellIndex d1d2 = d1 + d2;
                Vector3 e = vp + GetPosition(size, d1d2) * .5f;
                Vector3 e1 = e + GetPosition(size, d3) * .5f;
                Vector3 e2 = e - GetPosition(size, d3) * .5f;

                int sign = entityField.GetMidpointSign(e1, e2);
                if (sign != 0)
                {
                    var vd1 = v + d1;
                    var vd2 = v + d2;
                    var vd1d2 = vd1 + d2;
                    var vd1p = GetCellCenter(cellCenters, vd1);
                    var vd2p = GetCellCenter(cellCenters, vd2);
                    var vd1d2p = GetCellCenter(cellCenters, vd1d2);

                    Vector3 v1 = GetCellCenter(cellCenters, v);
                    Vector3 v2 = GetCellCenter(cellCenters, vd1);
                    Vector3 v3 = GetCellCenter(cellCenters, vd1d2);
                    Vector3 v4 = GetCellCenter(cellCenters, vd2);
                    if (sign > 0)
                    {
                        meshBuilder.AddQuad(v1, v4, v3, v2);
                    }
                    else
                    {
                        meshBuilder.AddQuad(v1, v2, v3, v4);
                    }

                    navMesh.Connect(v, vd1, v1, vd1p);
                    navMesh.Connect(vd1, vd1d2, vd1p, vd1d2p);
                    navMesh.Connect(v, vd1d2, v1, vd1d2p);

                    navMesh.Connect(vd2, vd1d2, vd2p, vd1d2p);
                    navMesh.Connect(v, vd2, v1, vd2p);

                    //navMesh.Connect(vd1, vd2, vd1p, vd2p);
                }
            });
        });
    }

    public static void ForEachCell1(int size, Action<CellIndex> action)
    {
        CellIndex cell;
        for (cell.p1 = -1; cell.p1 < size + 1; cell.p1 += 1)
        {
            for (cell.p2 = -1; cell.p2 < size + 1; cell.p2 += 1)
            {
                for (cell.p3 = -1; cell.p3 < size + 1; cell.p3 += 1)
                {
                    action.Invoke(cell);
                }
            }
        }
    }

    public static void ForEachCell2(int size, Action<CellIndex> action)
    {
        CellIndex cell;
        for (cell.p1 = 0; cell.p1 < size; cell.p1 += 1)
        {
            for (cell.p2 = 0; cell.p2 < size; cell.p2 += 1)
            {
                for (cell.p3 = 0; cell.p3 < size; cell.p3 += 1)
                {
                    action.Invoke(cell);
                }
            }
        }
    }

    public static void ForEachEdge(Action<CellIndex, CellIndex, CellIndex> action)
    {
        for (int i = 0; i < EDGES.Length; i += 3)
        {
            action(DIMS[EDGES[i + 0]], DIMS[EDGES[i + 1]], DIMS[EDGES[i + 2]]);
        }
    }

    public static float GetDensity(EntityField entityField, int size, CellIndex p)
    {
        return entityField.GetDensity(GetPosition(size, p));
    }

    public bool HasPositiveDensity(float f)
    {
        return f >= 0f;
    }

    // return the position in local coorindates from 0 to 1
    public static Vector3 GetPosition(int size, CellIndex index)
    {
        return index * (1.0f / size);
    }

    public static Cell GetCell(Dictionary<CellIndex, Cell> cellCenters, CellIndex index)
    {
        if (! cellCenters.ContainsKey(index))
        {
            cellCenters.Add(index, new Cell());
        }

        return cellCenters[index];
    }

    internal static void AddCellCenter(Dictionary<CellIndex, Cell> cellCenters, Vector3 p, CellIndex v)
    {
        Cell cell = GetCell(cellCenters, v);
        cell.count += 1;
        cell.p += p;
    }

    internal static Vector3 GetCellCenter(Dictionary<CellIndex, Cell> cellCenters, CellIndex index)
    {
        return GetCell(cellCenters, index).GetCenter();
    }
}