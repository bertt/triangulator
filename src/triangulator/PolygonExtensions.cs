using System;
using System.Numerics;
using Wkx;

namespace Triangulate
{
    public static class PolygonExtensions
    {
        public static Vector3 GetNormal(this Polygon polygon)
        {
            var vect01 = polygon.ExteriorRing.Points[1].ToVector3() - polygon.ExteriorRing.Points[0].ToVector3();
            var vect02 = polygon.ExteriorRing.Points[polygon.ExteriorRing.Points.Count - 2].ToVector3() - polygon.ExteriorRing.Points[0].ToVector3();
            
            var maxProd = Vector3.Cross(vect01, vect02);
            var maxLen = maxProd.Length();
            
            for (var i = 3; i < polygon.ExteriorRing.Points.Count-1; i++)
            {
                var vect0i = polygon.ExteriorRing.Points[polygon.ExteriorRing.Points.Count - i].ToVector3() - polygon.ExteriorRing.Points[0].ToVector3();
                var vectProd = Vector3.Cross(vect01, vect0i);
                var vectLen = vectProd.Length();

                if (vectLen > maxLen) {
                    maxLen = vectLen;
                    maxProd = vectProd;
                }
            }

            return Vector3.Normalize(maxProd);
        }
    }
}
