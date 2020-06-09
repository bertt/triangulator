using Wkx;
using System.Numerics;

namespace Triangulator
{
    public static class PointExtensions
    {
        public static Vector3 ToVector3(this Point p)
        {
            return new Vector3((float)p.X, (float)p.Y, (float)p.Z);
        }
    }
}
