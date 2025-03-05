using System.Numerics;
using Wkx;

namespace Triangulate
{
    public static class PolygonExtensions
    {
        public static Vector3 GetNormal(this Polygon polygon)
        {
            // normal calculation via Newell's method
            double sumX = 0, sumY = 0, sumZ = 0;
            double 
                x1 = (double)polygon.ExteriorRing.Points[0].X,
                y1 = (double)polygon.ExteriorRing.Points[0].Y,
                z1 = (double)polygon.ExteriorRing.Points[0].Z,
                x2, y2, z2;
            
            for (int i = 0; i < polygon.ExteriorRing.Points.Count-2; i++)
            {
                x2 = (double)polygon.ExteriorRing.Points[i+1].X;
                y2 = (double)polygon.ExteriorRing.Points[i+1].Y;
                z2 = (double)polygon.ExteriorRing.Points[i+1].Z;

                sumX += (y1 - y2) * (z1 + z2);
                sumY += (z1 - z2) * (x1 + x2);
                sumZ += (x1 - x2) * (y1 + y2);

                x1 = x2;
                y1 = y2;
                z1 = z2;
            }
            
            x2 = (double)polygon.ExteriorRing.Points[0].X;
            y2 = (double)polygon.ExteriorRing.Points[0].Y;
            z2 = (double)polygon.ExteriorRing.Points[0].Z;

            sumX += (y1 - y2) * (z1 + z2);
            sumY += (z1 - z2) * (x1 + x2);
            sumZ += (x1 - x2) * (y1 + y2);

            sumX /= (polygon.ExteriorRing.Points.Count-1);
            sumY /= (polygon.ExteriorRing.Points.Count-1);
            sumZ /= (polygon.ExteriorRing.Points.Count-1);
            
            var normal = new Vector3((float)sumX, (float)sumY, (float)sumZ);
            return Vector3.Normalize(normal);
        }
    }
}
