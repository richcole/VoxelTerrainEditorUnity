using UnityEngine;

public abstract class DensityField 
    {
        public abstract float GetDensity(Vector3 p);

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

    }
