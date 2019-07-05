using System;
using UnityEngine;

[Serializable]
public class Cell
{
    public int count;
    public Vector3 p;

    public Vector3 GetCenter()
    {
        return p / count;
    }
}

[Serializable]
public struct CellIndex : IEquatable<CellIndex>
{
    public int p1, p2, p3;

    public CellIndex(int[] p)
    {
        p1 = p[0];
        p2 = p[1];
        p3 = p[2];
    }

    public CellIndex(int p1, int p2, int p3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
    }

    public bool Equals(CellIndex other)
    {
        return p1 == other.p1 && p2 == other.p2 && p3 == other.p3;
    }

    public override int GetHashCode()
    {
        return (p1 << 16) ^ (p2 << 8) ^ p3;
    }

    public override string ToString()
    {
        return "[" + p1 + ", " + p2 + ", " + p3 + "]";
    }

    public static CellIndex operator *(CellIndex a, int v)
    {
        return new CellIndex(a.p1 * v, a.p2 * v, a.p3 * v);
    }

    public static Vector3 operator *(CellIndex a, float v)
    {
        return new Vector3(a.p1 * v, a.p2 * v, a.p3 * v);
    }

    public static CellIndex operator +(CellIndex a, CellIndex b)
    {
        return new CellIndex(a.p1 + b.p1, a.p2 + b.p2, a.p3 + b.p3);
    }
}

