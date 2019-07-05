using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity
{
    public abstract void OnDrawGizmos();

    public abstract float GetDensity(Vector3 p);

    public abstract bool Intersects(Vector3 p);

    public virtual bool HasMidpoint(Vector3 p, Vector3 q)
    {
        return GetDensity(p) > 0f != GetDensity(q) > 0f;
    }

    public virtual Vector3 GetMidPoint(Vector3 p, Vector3 q)
    {
        float a = GetDensity(p);
        float b = GetDensity(q);
        float alpha = a / (a - b);

        return p + ((q - p) * alpha);
    }

    public virtual int GetMidpointSign(Vector3 p, Vector3 q)
    {
        float a = GetDensity(p);
        float b = GetDensity(q);

        if (a > 0 == b > 0)
        {
            return 0;
        }
        else if (a > 0 && b <= 0)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public abstract EntityData asData();
}

public class Ground : Entity
{
    float height = 0f;

    public Ground(float height)
    {
        this.height = height;
    }

    public override float GetDensity(Vector3 p)
    {
        if (p.y < height)
        {
            return 1f;
        }
        else
        {
            return -1f;
        }
    }

    public override bool Intersects(Vector3 p)
    {
        return true;
    }

    public override void OnDrawGizmos()
    {
        Gizmos.DrawCube(Vector3.zero + Vector3.up * height, new Vector3(1f, 0.1f, 1f));
    }

    public override EntityData asData()
    {
        EntityData ret = new EntityData();
        ret.type = EntityType.Ground;
        ret.height = height;
        return ret;
    }
}

public class PerlinGround : Entity
{
    Matrix4x4 localToGlobalMatrix;
    float height;
    float scale;
    float freq;

    public PerlinGround(Matrix4x4 localToGlobalMatrix, float height, float scale, float freq)
    {
        this.localToGlobalMatrix = localToGlobalMatrix;
        this.height = height;
        this.scale = scale;
        this.freq = freq;
    }

    public override float GetDensity(Vector3 localP)
    {
        Vector3 globalP = localToGlobalMatrix.MultiplyPoint(localP);
        float globalHeight = height + scale * Mathf.PerlinNoise(globalP.x * freq, globalP.z * freq);
        if (globalP.y < globalHeight)
        {
            return 1f;
        }
        else
        {
            return -1f;
        }
    }

    public override bool Intersects(Vector3 p)
    {
        return true;
    }

    public override void OnDrawGizmos()
    {
    }

    public override EntityData asData()
    {
        EntityData ret = new EntityData();
        ret.type = EntityType.PerlinGround;
        ret.scale = scale;
        ret.freq = freq;
        ret.localToGlobalMatrix = localToGlobalMatrix;
        ret.height = height;
        return ret;
    }
}

public class Sphere : Entity
{
    float r = 2f;
    Vector3 c = Vector3.zero;

    public Sphere(Vector3 center, float radius)
    {
        this.c = center;
        this.r = radius;
    }

    public override float GetDensity(Vector3 p)
    {
        return r - (p - c).magnitude;
    }

    public override bool Intersects(Vector3 p)
    {
        return (p - c).magnitude < r;
    }

    public override void OnDrawGizmos()
    {
        Gizmos.DrawSphere(c, r);
    }

    public override EntityData asData()
    {
        EntityData ret = new EntityData();
        ret.type = EntityType.Sphere;
        ret.center = c;
        ret.radius = r;
        return ret;
    }
}

public class SphereVoid : Entity
{
    float r = 2f;
    Vector3 c = Vector3.zero;

    public SphereVoid(Vector3 center, float radius)
    {
        this.c = center;
        this.r = radius;
    }

    public override float GetDensity(Vector3 p)
    {
        return (p - c).magnitude - r;
    }

    public override bool Intersects(Vector3 p)
    {
        return (p - c).magnitude < r;
    }

    public override void OnDrawGizmos()
    {
        Gizmos.DrawSphere(c, r);
    }

    public override EntityData asData()
    {
        EntityData ret = new EntityData();
        ret.type = EntityType.SphereVoid;
        ret.center = c;
        ret.radius = r;
        return ret;
    }

}

public enum EntityType
{
    Ground,
    PerlinGround,
    Sphere,
    SphereVoid
}

[Serializable]
public struct EntityData
{
    public EntityType type;
    public Vector3 center;
    public Matrix4x4 localToGlobalMatrix;
    public float radius;
    public float height;
    public float scale;
    public float freq;
}

public class EntityField : DensityField
{
    public LinkedList<Entity> entities;

    public EntityField(List<EntityData> entityFieldData)
    {
        entities = new LinkedList<Entity>();
        foreach (EntityData entity in entityFieldData)
        {
            entities.AddLast(asEntity(entity));
        }
    }

    public EntityField(EntityField entityField)
    {
        entities = new LinkedList<Entity>();
        foreach (Entity entity in entityField.entities)
        {
            entities.AddLast(entity);
        }
    }

    public EntityField()
    {
        entities = new LinkedList<Entity>();
    }

    public List<EntityData> asEntityFieldData()
    {
        List<EntityData> ret = new List<EntityData>();
        foreach (Entity entity in entities)
        {
            ret.Add(entity.asData());
        }
        return ret;
    }

    public void AddGround(float height)
    {
        entities.AddFirst(new Ground(height));
    }

    public void AddPerlinGround(Matrix4x4 localToGlobalMatrix, float height, float scale, float freq)
    {
        entities.AddFirst(new PerlinGround(localToGlobalMatrix, height, scale, freq));
    }

    public void AddSphere(Vector3 center, float radius)
    {
        entities.AddFirst(new Sphere(center, radius));
    }

    public override float GetDensity(Vector3 p)
    {
        foreach (Entity entity in entities)
        {
            if (entity.Intersects(p))
            {
                return entity.GetDensity(p);
            }
        }

        return -1f;
    }

    public override Vector3 GetMidPoint(Vector3 p, Vector3 q)
    {
        Vector3 m = Vector3.zero;
        int n = 0;
        foreach (Entity entity in entities)
        {
            if (entity.HasMidpoint(p, q))
            {
                m += entity.GetMidPoint(p, q);
                n += 1;
            }
        }
        if (n > 0)
        {
            return m / n;
        }
        else
        {
            return (p + q) / 2;
        }
    }

    public override int GetMidpointSign(Vector3 p, Vector3 q)
    {
        float a = GetDensity(p);
        float b = GetDensity(q);

        bool ap = a > 0;
        bool bp = b > 0;

        if (ap == bp)
        {
            return 0;
        }
        else if (ap && !bp)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public void Errode(Vector3 c, float r)
    {
        entities.AddFirst(new SphereVoid(c, r));
    }

    public void Fill(Vector3 c, float r)
    {
        entities.AddFirst(new Sphere(c, r));
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Entity entity in entities)
        {
            entity.OnDrawGizmos();
        }
    }

    public Entity asEntity(EntityData entityData)
    {
        switch (entityData.type)
        {
            case EntityType.Ground:
                return new Ground(entityData.height);
            case EntityType.PerlinGround:
                return new PerlinGround(entityData.localToGlobalMatrix, entityData.height, entityData.scale, entityData.freq);
            case EntityType.Sphere:
                return new Sphere(entityData.center, entityData.radius);
            case EntityType.SphereVoid:
                return new SphereVoid(entityData.center, entityData.radius);
        }
        throw new Exception("Unknown type" + entityData.type);
    }

    public EntityField Clone()
    {
        return new EntityField(this);
    }
}

