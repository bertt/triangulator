using System.Collections.Generic;
using System.Numerics;
using Wkx;

namespace Triangulate
{
    public static class LineStringExtensions
    {
        public static List<Vector3> ToVector3(this LineString lineString)
        {
            var points = new List<Vector3>();
            foreach (var point in lineString.Points)
            {
                var z = point.Z ?? 0;
                points.Add(new Vector3((float)point.X, (float)point.Y, (float)z));
            }

            return points;
        }

    }
}
