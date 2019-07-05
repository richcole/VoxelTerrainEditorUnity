using UnityEngine;

class SphericalField : DensityField
    {
        [SerializeField]
        Vector3 c;

        [SerializeField]
        float r;

        public SphericalField(Vector3 c, float r)
        {
            this.c = c;
            this.r = r;
        }

        public override float GetDensity(Vector3 p)
        {
            return r - (p - c).magnitude;
        }
    }
