using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



public class VoxelSurface : MonoBehaviour, ISerializationCallbackReceiver, Interactor
{
    [SerializeField]
    public int[] n = { 3, 3, 3 };

    [SerializeField]
    public int size = 10;

    [SerializeField]
    public int perlinGroundScale = 1;

    [SerializeField]
    public int perlinGroundFreq = 1;

    [SerializeField]
    public float materialScale = 1;

    [SerializeField]
    public bool drawGizmos = false;

    public EntityField entityField;

    [SerializeField]
    public List<EntityData> entityFieldData;

    [SerializeField]
    private SerialiableNavMesh serializableNavMesh;

    [SerializeField]
    public CellIndex meshCoords;

    public Task<UpdateMeshJob> updateMeshTask;

    public DigController digController;

    public NavMesh navMesh;

    public void Initialize()
    {
        entityField = new EntityField();
    }

    public void AddGround(float height)
    {
        float localHeight = transform.InverseTransformPoint(Vector3.up * height).y;
        entityField.AddGround(localHeight);
    }

    public void AddPerlinGround(float height, float scale, float freq)
    {
        entityField.AddPerlinGround(transform.localToWorldMatrix, height, scale, freq);
    }

    public void CreateOwnMesh()
    {
        GetComponent<MeshFilter>().sharedMesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh.name = "SurfaceMesh-" + gameObject.name;
    }

    public Task GenerateMesh()
    {
        if (updateMeshTask == null)
        {
            updateMeshTask = new Task<UpdateMeshJob>(() =>
            {
                UpdateMeshJob updateMeshJob = new UpdateMeshJob(entityField.Clone(), size, materialScale);
                updateMeshJob.Execute();
                return updateMeshJob;
            });
            updateMeshTask.Start();
            return updateMeshTask;
        }
        return null;
    }

    void Start()
    {
        digController = FindObjectOfType<DigController>();
    }

    void Update()
    {
    }

    public void UpdateMesh()
    {
        lock (this)
        {
            if (updateMeshTask != null && updateMeshTask.IsCompleted)
            {
                var sharedMesh = GetComponent<MeshFilter>().sharedMesh;
                sharedMesh.name = "Mesh-" + gameObject.name;

                updateMeshTask.Result.meshBuilder.CopyToMesh(sharedMesh);
                GetComponent<MeshCollider>().sharedMesh = sharedMesh;

                navMesh = updateMeshTask.Result.navMesh;

                updateMeshTask = null;
            }
        }
    }

    // operate based on global position
    public Task Build(Vector3 position, float radius)
    {
        Vector3 localPosition = transform.InverseTransformPoint(position);
        float localRadius = transform.InverseTransformVector(Vector3.one.normalized * radius).magnitude;
        if (entityField != null)
        {
            entityField.Fill(localPosition, localRadius);
            return GenerateMesh();
        }
        else
        {
            Debug.Log("EntityField is null");
            return null;
        }
    }

    public Task Dig(Vector3 position, float radius)
    {
        Vector3 localPosition = transform.InverseTransformPoint(position);
        float localRadius = transform.InverseTransformVector(Vector3.one.normalized * radius).magnitude;
        if (entityField != null)
        {
            entityField.Errode(localPosition, localRadius);
            return GenerateMesh();
        }
        else
        {
            Debug.Log("EntityField is null");
            return null;
        }
    }

    public bool OverlapsSphere(Vector3 center, float radius)
    {
        float localRadius = transform.InverseTransformVector(Vector3.one.normalized * radius).magnitude;
        Vector3 localCenter = transform.InverseTransformPoint(center);
        Vector3 cubeCenter = Vector3.one * 0.5f;
        float clickDistance = (localCenter - cubeCenter).magnitude;
        if (clickDistance < 0.9 + localRadius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnDrawGizmosSelected()
    {

        if (navMesh != null)
        {
           // navMesh.DrawGizmos(transform);
        }
        if (false)
        {
        }

        if (entityField == null)
        {
            return;
        }

        if (!drawGizmos)
        {
            return;
        }

        Color onColor = new Color(0.2f, 0.1f, 1f, 0.5f);
        Color offColor = new Color(1f, 0.1f, 0.2f, 0.5f);
        Color vertexColor = new Color(0.2f, 1f, 0.1f);
        Vector3 cubeSize = Vector3.one * 0.05f;
        // Vector3 jitter = Vector3.one * 0.01f;
        Vector3 jitter = Vector3.zero;
    }


    public void SetMaterial(Material material)
    {
        GetComponent<MeshRenderer>().material = material;
    }

    public void OnBeforeSerialize()
    {
        if (entityField != null)
        {
            entityFieldData = entityField.asEntityFieldData();
        }

        if (navMesh != null)
        {
            serializableNavMesh = navMesh.AsSerialiableNavMesh();
        }
    }

    public void OnAfterDeserialize()
    {
        if (entityFieldData != null)
        {
            entityField = new EntityField(entityFieldData);
        }

        if (serializableNavMesh != null)
        {
            navMesh = new NavMesh(serializableNavMesh);
        }
    }

    public void Interact(Ray ray, RaycastHit hit)
    {
        digController.Interact(ray, hit);
    }
}
