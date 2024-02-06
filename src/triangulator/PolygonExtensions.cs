using System;
using System.Numerics;
using Wkx;

namespace Triangulate
{
    public static class PolygonExtensions
    {
        public static Vector3 GetNormal(this Polygon polygon)
        {
            // calculating normal vector whith skipping the consecutive duplicate vertices
            double epsilon = 1e-8;
            
            double startX = (double)polygon.ExteriorRing.Points[0].X;
            double startY = (double)polygon.ExteriorRing.Points[0].Y;
            double startZ = (double)polygon.ExteriorRing.Points[0].Z;
           
            for (int i = 1; i < polygon.ExteriorRing.Points.Count-1; i++)
            {
                // find the first vector for cross product
                // skip point if it duplicates ptStart
                double v1X = (double)polygon.ExteriorRing.Points[i].X - startX;
                double v1Y = (double)polygon.ExteriorRing.Points[i].Y - startY;
                double v1Z = (double)polygon.ExteriorRing.Points[i].Z - startZ;
                
                double v1Length = Math.Sqrt(v1X*v1X + v1Y*v1Y + v1Z*v1Z);
                if (v1Length < epsilon)
                    continue;

                // the first vector found
                // get the second vector and calculate cross product
                // select the best cross product: max cross product
                double maxProdLen = 0.0;
                double maxCrossProdX = 0.0;
                double maxCrossProdY = 0.0;
                double maxCrossProdZ = 0.0;

                for (int j = i+1; j < polygon.ExteriorRing.Points.Count-1; j++)
                {
                    // skip point if it duplicates ptStart
                    double v2X = (double)polygon.ExteriorRing.Points[j].X - startX;
                    double v2Y = (double)polygon.ExteriorRing.Points[j].Y - startY;
                    double v2Z = (double)polygon.ExteriorRing.Points[j].Z - startZ;

                    double v2Length = Math.Sqrt(v2X*v2X + v2Y*v2Y + v2Z*v2Z);
                    if (v2Length < epsilon)
                        continue;

                    // the second vector found
                    // calculate and get the best cross product
                    double crossProdX = v1Y*v2Z - v1Z*v2Y;
                    double crossProdY = v1Z*v2X - v1X*v2Z;
                    double crossProdZ = v1X*v2Y - v1Y*v2X;

                    double prodLen = Math.Sqrt(crossProdX*crossProdX + crossProdY*crossProdY + crossProdZ*crossProdZ);
                    if (prodLen > maxProdLen) {
                        maxProdLen = prodLen;
                        maxCrossProdX = crossProdX;
                        maxCrossProdY = crossProdY;
                        maxCrossProdZ = crossProdZ;
                    }
                }

                var maxProd = new Vector3((float)maxCrossProdX, (float)maxCrossProdY, (float)maxCrossProdZ);
                return Vector3.Normalize(maxProd);
            }

            return new Vector3(0, 0, 0);
        }
    }
}
