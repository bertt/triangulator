using System.Numerics;
using Wkx;

namespace triangulator
{
    public static class PolygonExtensions
    {
        public static Vector3 GetNormal(this Polygon polygon)
        {
            var vect1 = polygon.ExteriorRing.Points[1].ToVector3() - polygon.ExteriorRing.Points[0].ToVector3();
            var vect2 = polygon.ExteriorRing.Points[2].ToVector3() - polygon.ExteriorRing.Points[0].ToVector3();
            var vectProd = Vector3.Cross(vect1, vect2);
            return vectProd;
        }
    }
}
