using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CellConnection
{
    public CellIndex from;
    public CellIndex to;

    public CellConnection(CellIndex from, CellIndex to) 
    {
        this.from = from;
        this.to = to;
    }
}

[Serializable]
public struct CellPosition
{
    public CellIndex index;
    public Vector3 position;

    public CellPosition(CellIndex index, Vector3 position)
    {
        this.index = index;
        this.position = position;
    }
}

[Serializable]
public class SerialiableNavMesh
{
    public List<CellConnection> connections;
    public List<CellPosition> positions;
}

public class NavMesh
{
    public Dictionary<CellIndex, List<CellIndex>> connections;
    public Dictionary<CellIndex, Vector3> positions;

    public NavMesh()
    {
        connections = new Dictionary<CellIndex, List<CellIndex>>();
        positions = new Dictionary<CellIndex, Vector3>();
    }

    public NavMesh(SerialiableNavMesh s)
    {
        connections = new Dictionary<CellIndex, List<CellIndex>>();
        positions = new Dictionary<CellIndex, Vector3>();

        if (s.connections != null)
        {
            foreach (CellConnection c in s.connections)
            {
                AddAdjacency(c.from, c.to);
            }
        }

        if (s.positions != null)
        {
            foreach (CellPosition p in s.positions)
            {
                AddCellPosition(p.index, p.position);
            }
        }
    }

    public void Merge(NavMesh other, CellIndex meshCoords, int pointDensity, Transform transform)
    {
        
        CellIndex offset = meshCoords * pointDensity;
        
        foreach (CellIndex cell in other.positions.Keys)
        {
            CellIndex offsetCell = offset + cell;
            AddCellPosition(offsetCell, transform.TransformPoint(other.GetPosition(cell).Value));
        }

        foreach (CellIndex from in other.connections.Keys)
        {
            CellIndex fromOffset = from + offset;
            foreach(CellIndex to in other.connections[from])
            {
                CellIndex toOffset = to + offset;
                AddAdjacency(fromOffset, toOffset);
                AddAdjacency(toOffset, fromOffset);
            }
        }

    }

    public SerialiableNavMesh AsSerialiableNavMesh()
    {
        SerialiableNavMesh ret = new SerialiableNavMesh();
        if (connections != null)
        {
            ret.connections = FUtils.FlatMap(connections.Keys, from =>
                FUtils.Map(connections[from], to => new CellConnection(from, to)));
        }
        else
        {
            ret.connections = new List<CellConnection>();
        }

        if (positions != null)
        {
            ret.positions = FUtils.Map(positions.Keys, from =>
                new CellPosition(from, positions[from]));
        }
        else
        {
            ret.positions = new List<CellPosition>();
        }

        return ret;
    }

    public void Connect(CellIndex from, CellIndex to, Vector3 fromPosition, Vector3 toPosition)
    {
        AddAdjacency(from, to);
        AddAdjacency(to, from);
        AddCellPosition(from, fromPosition);
        AddCellPosition(to, toPosition);
    }

    public void AddAdjacency(CellIndex from, CellIndex to)
    {
        List<CellIndex> adjList;

        if (!connections.ContainsKey(from))
        {
            adjList = new List<CellIndex>();
            connections.Add(from, adjList);
        }
        else
        {
            adjList = connections[from];
        }

        if (!adjList.Contains(to))
        {
            adjList.Add(to);
        }

    }

    public void AddCellPosition(CellIndex from, Vector3 fromPoint)
    {
        if (! positions.ContainsKey(from))
        {
            positions[from] = fromPoint;
        }
    }

    public CellIndex ClosestCell(Vector3 position)
    {
        return ClosestCell(positions.Keys, position);
    }

    public List<CellIndex> FindPath(Vector3 fromPosition, Vector3 toPosition)
    {
        CellIndex from = ClosestCell(positions.Keys, fromPosition);
        CellIndex to = ClosestCell(positions.Keys, toPosition);

        HashSet<CellIndex> visited = new HashSet<CellIndex>();
        HashSet<CellIndex> boundary = new HashSet<CellIndex>();
        Dictionary<CellIndex, CellIndex> parentMap = new Dictionary<CellIndex, CellIndex>();
        Dictionary<CellIndex, float> distanceFromStart = new Dictionary<CellIndex, float>();

        boundary.Add(from);
        distanceFromStart[from] = 0;

        while (boundary.Count > 0)
        {
            CellIndex closest = ClosestBoundaryCell(boundary, toPosition, distanceFromStart);
            float closestDistance = TotalDistance(closest, toPosition, distanceFromStart);

            if (closest.Equals(to))
            {
                return TraceParents(parentMap, to);
            }
            boundary.Remove(closest);
            visited.Add(closest);
            foreach (CellIndex adj in connections[closest]) {
                if (! visited.Contains(adj))
                {
                    boundary.Add(adj);
                    parentMap[adj] = closest;
                    distanceFromStart[adj] = distanceFromStart[closest] + Distance(adj, closest);
                    float totalDistance = TotalDistance(adj, toPosition, distanceFromStart);
                }
            }
        }

        return null;
    }

    public float Distance(CellIndex from, CellIndex to)
    {
        return (positions[from] - positions[to]).magnitude;
    }

    public List<CellIndex> TraceParents(Dictionary<CellIndex, CellIndex> parentMap, CellIndex start)
    {
        List<CellIndex> path = new List<CellIndex>();
        path.Add(start);
        while(parentMap.ContainsKey(start))
        {
            start = parentMap[start];
            path.Add(start);
        }
        path.Reverse();
        return path;
    }

    public CellIndex ClosestCell(IEnumerable<CellIndex> lst, Vector3 p)
    {
        return FUtils.ArgMinStruct(lst, cellIndex => (p - positions[cellIndex]).magnitude).Value;
    }

    public CellIndex ClosestBoundaryCell(IEnumerable<CellIndex> lst, Vector3 p, Dictionary<CellIndex, float> distanceFromStart)
    {
        return FUtils.ArgMinStruct(lst, cellIndex => TotalDistance(cellIndex, p, distanceFromStart)).Value;
    }

    public float TotalDistance(CellIndex cellIndex, Vector3 p, Dictionary<CellIndex, float> distanceFromStart)
    {
        return distanceFromStart[cellIndex] + (p - positions[cellIndex]).magnitude;
    }

    public Vector3? GetPosition(CellIndex cellIndex)
    {
        if (positions.ContainsKey(cellIndex))
        {
            return positions[cellIndex];
        }
        else
        {
            return null;
        }
    }

    public List<CellIndex> GetConnections(CellIndex monsterCell)
    {
        if (connections.ContainsKey(monsterCell))
        {
            return connections[monsterCell];
        }
        return new List<CellIndex>();
    }

    public void DrawGizmos(Matrix4x4 transform)
    {
        Gizmos.color = Color.white;
        foreach (CellIndex from in connections.Keys)
        {
            foreach (CellIndex to in connections[from])
            {
                var fromPosition = GetPosition(from);
                var toPosition = GetPosition(to);
                if (fromPosition.HasValue && toPosition.HasValue)
                {
                    Vector3 a = transform.MultiplyPoint(fromPosition.Value);
                    Vector3 b = transform.MultiplyPoint(toPosition.Value);
                    Gizmos.DrawLine(a, b);
                    Gizmos.DrawSphere(a, 0.2f);
                }
            }
        }


    }

}
