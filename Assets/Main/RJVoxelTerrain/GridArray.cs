using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public enum EditMode
{
    BUILD,
    DIG
}

public class GridArray : MonoBehaviour, Interactor, ISerializationCallbackReceiver
{
    [SerializeField]
    public VoxelSurface voxelSurfacePrototype;

    [SerializeField]
    public int sizeXZ = 10;

    [SerializeField]
    public int sizeY = 3;

    [SerializeField]
    public int height = 3;

    [SerializeField]
    public float perlinScale = 1;

    [SerializeField]
    public float perlinFreq = 1;

    [SerializeField]
    public float scale = 10;

    [SerializeField]
    public int pointDensity = 10;

    [SerializeField]
    public EditMode editMode = EditMode.BUILD;

    [SerializeField]
    public float buildRadius = 2f;

    [SerializeField]
    public Material material;

    [SerializeField]
    public float materialScale = 1.0f;

    private HashSet<Task> tasks = new HashSet<Task>();
    private CustomEventsManager eventManager;

    public SerialiableNavMesh serializableNavMesh;
    public NavMesh navMesh;

    public SimpleController player;
    public MonsterController monster;
    public Vector3 localCreaturePosition;
    public Vector3 localPlayerPosition;
    public List<CellIndex> path;
    public CellIndex? playerCell;
    public CellIndex? monsterCell;

    public void Initialize()
    {
        foreach (VoxelSurface surface in GetComponentsInChildren<VoxelSurface>())
        {
            DestroyImmediate(surface.gameObject);
        }

        for (int x = 0; x < sizeXZ; ++x)
        {
            for (int y = 0; y < sizeY; ++y)
            {
                for (int z = 0; z < sizeXZ; ++z)
                {
                    var surface = Instantiate(voxelSurfacePrototype, transform);
                    surface.gameObject.layer = this.gameObject.layer;
                    surface.size = pointDensity;
                    surface.meshCoords = new CellIndex(x, y, z);
                    surface.name = "VoxelSurface x=" + x + " y=" + y + " z=" + z;
                    surface.transform.localPosition = new Vector3(x * scale, y * scale, z * scale);
                    surface.transform.localScale = Vector3.one * scale;
                    surface.SetMaterial(material);
                    surface.CreateOwnMesh();
                    surface.Initialize();
                    surface.AddPerlinGround(height, perlinScale, perlinFreq);
                    tasks.Add(surface.GenerateMesh());
                }
            }
        }
    }

    public void Start()
    {
        eventManager = FindObjectOfType<CustomEventsManager>();
    }

    public void Update()
    {
        UpdateMesh();
    }

    public void UpdateMesh()
    {
        if (tasks == null)
        {
            tasks = new HashSet<Task>();
        }

        if (tasks.Count > 0 && FUtils.All(tasks, task => task == null || task.IsCompleted || task.IsFaulted || task.IsCanceled))
        {
            tasks.Clear();
            FUtils.Elvis(eventManager, eventManager =>
                eventManager.DiggingChanged(DiggingMode.NotDigging));

            navMesh = new NavMesh();
            foreach (VoxelSurface surface in GetComponentsInChildren<VoxelSurface>())
            {
                surface.UpdateMesh();
                navMesh.Merge(surface.navMesh, surface.meshCoords, pointDensity, surface.transform);
            }

            Debug.Log("Finished generating mesh positions count=" + navMesh.positions.Count);
            if (navMesh.positions.Count > 0)
            {
                player = FindObjectOfType<SimpleController>();
                monster = FindObjectOfType<MonsterController>();
                localPlayerPosition = transform.InverseTransformPoint(player.transform.position);
                localCreaturePosition = transform.InverseTransformPoint(monster.transform.position);
                playerCell = navMesh.ClosestCell(localPlayerPosition);
                monsterCell = navMesh.ClosestCell(localCreaturePosition);
                path = navMesh.FindPath(localPlayerPosition, localCreaturePosition);
            }

        }
    }

    public void Hit(Vector3 position)
    {
        if (tasks == null)
        {
            tasks = new HashSet<Task>();
        }
        if (editMode == EditMode.DIG)
        {
            Dig(position, buildRadius);
        }
        else
        {
            Build(position, buildRadius);
        }
    }

    public void Dig(Vector3 position, float blastRadius)
    {
        if (tasks.Count == 0)
        {
            foreach (VoxelSurface surface in GetComponentsInChildren<VoxelSurface>())
            {
                if (surface.OverlapsSphere(position, buildRadius))
                {
                    tasks.Add(surface.Dig(position, buildRadius));
                }
            }
            if (tasks.Count > 0 && eventManager != null)
            {
                eventManager.DiggingChanged(DiggingMode.Digging);
            }
        }
    }

    public void Build(Vector3 position, float blastRadius)
    {
        if (tasks.Count == 0)
        {
            foreach (VoxelSurface surface in GetComponentsInChildren<VoxelSurface>())
            {
                if (surface.OverlapsSphere(position, buildRadius))
                {
                    tasks.Add(surface.Build(position, buildRadius));
                }
            }
            if (tasks.Count > 0)
            {
                FUtils.Elvis(eventManager, eventManager => eventManager.DiggingChanged(DiggingMode.Digging));
            }
        }
    }

    public void Generate()
    {
        foreach (VoxelSurface surface in GetComponentsInChildren<VoxelSurface>())
        {
            surface.size = pointDensity;
            surface.materialScale = materialScale;
            surface.gameObject.layer = this.gameObject.layer;
            tasks.Add(surface.GenerateMesh());
        }
    }

    public void SetMaterial()
    {
        foreach (VoxelSurface surface in GetComponentsInChildren<VoxelSurface>())
        {
            surface.SetMaterial(material);
        }
    }

    public void Interact(Ray ray, RaycastHit hit)
    {

    }

    void OnDrawGizmos()
    {
        if (true)
        {
            return;
        }

        if (navMesh != null)
        {
            navMesh.DrawGizmos(Matrix4x4.identity);

            if (playerCell.HasValue)
            {
                Vector3? position = navMesh.GetPosition(playerCell.Value);
                if (position.HasValue)
                {
                    Gizmos.DrawCube(transform.TransformPoint(position.Value), Vector3.one);
                }
            }

            if (monsterCell.HasValue)
            {
                Vector3? position = navMesh.GetPosition(monsterCell.Value);
                if (position.HasValue)
                {
                    Gizmos.DrawCube(transform.TransformPoint(position.Value), Vector3.one);
                }
            }


            Gizmos.color = Color.red;
            if (path != null && navMesh != null)
            {
                Vector3? prevPosition = new Nullable<Vector3>();
                foreach (CellIndex cellIndex in path)
                {
                    Vector3? position = navMesh.GetPosition(cellIndex);
                    if (position.HasValue && prevPosition.HasValue)
                    {
                        var from = transform.TransformPoint(prevPosition.Value);
                        var to = transform.TransformPoint(position.Value);
                        Gizmos.DrawLine(from, to);
                    }
                    prevPosition = position;
                }
            }
        }
    }

    public void OnBeforeSerialize()
    {
        if (navMesh != null)
        {
            serializableNavMesh = navMesh.AsSerialiableNavMesh();
        }
    }

    public void OnAfterDeserialize()
    {
        if (serializableNavMesh != null)
        {
            navMesh = new NavMesh(serializableNavMesh);
        }
    }

}
