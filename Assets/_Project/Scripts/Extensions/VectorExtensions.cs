using UnityEngine;

namespace Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 WithY(this Vector3 v, float y)
        {
            v.y = y;
            return v;
        }

        public static Vector3 WithX(this Vector3 v, float x)
        {
            v.x = x;
            return v;
        }
    }
}