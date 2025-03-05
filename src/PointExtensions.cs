using Wkx;
using System.Numerics;

namespace Triangulate
{
    public static class PointExtensions
    {
        public static Vector3 ToVector3(this Point p)
        {
            // If Z is not set, use 0
            var z= p.Z.HasValue ? (float)p.Z.Value : 0;
            return new Vector3((float)p.X, (float)p.Y, z);
        }
    }
}
