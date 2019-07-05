using UnityEngine;

    class PointField : DensityField
    {
        [SerializeField]
        Vector3 extent;

        [SerializeField]
        float[] density;

        [SerializeField]
        int n;

        public int GetIndex(Vector3 p)
        {
            if (p.x >= extent.x || p.y >= extent.y || p.z >= extent.z
                || p.x < 0 || p.y < 0 || p.z < 0)

            {
                return -1;
            }

            int x = (int)(p.x / extent.x * n);
            int y = (int)(p.y / extent.y * n);
            int z = (int)(p.z / extent.z * n);

            return GetIndex(x, y, z);
        }

        public int GetIndex(int x, int y, int z)
        {
            return (x * n * n) + (y * n) + z;
        }

        public PointField(Vector3 extent, int n)
        {
            this.extent = extent;
            this.n = n;
            this.density = new float[n * n * n];
        }

        public override float GetDensity(Vector3 p)
        {
            int index = GetIndex(p);
            if (index == -1)
            {
                return -1f;
            }

            return density[index];
        }

        public void DrawSphere(Vector3 c, float r)
        {
            for (int x = 0; x < n; ++x)
            {
                for (int y = 0; y < n; ++y)
                {
                    for (int z = 0; z < n; ++z)
                    {
                        Vector3 p = new Vector3(extent.x * x / n, extent.y * y / n, extent.z * z / n);
                        density[GetIndex(x, y, z)] = r - (p - c).magnitude;
                    }
                }
            }
        }

        public void Errode(Vector3 c, float r)
        {
            for (int x = 0; x < n; ++x)
            {
                for (int y = 0; y < n; ++y)
                {
                    for (int z = 0; z < n; ++z)
                    {
                        Vector3 p = new Vector3(extent.x * x / n, extent.y * y / n, extent.z * z / n);
                        if ((p - c).magnitude < r)
                        {
                            density[GetIndex(x, y, z)] = -1;
                        }
                    }
                }
            }
        }

    }
