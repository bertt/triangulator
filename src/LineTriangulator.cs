using System;
using System.Collections.Generic;
using System.Numerics;
using Wkx;

namespace Triangulate
{
    public static class LineTriangulator
    {

        private static Point ToPoint(Vector3 v)
        {
            return new Point(v.X, v.Y, v.Z);
        }
        public static MultiPolygon GetTriangles(List<List<Vector3>> circles, int? radialSegments)
        {
            var result = new MultiPolygon
            {
                Dimension = Dimension.Xyz
            };

            var polygons = new List<Polygon>();

            for (int i = 0; i < circles.Count - 1; i++)
            {
                var currentCircle = circles[i];
                var nextCircle = circles[i + 1];

                for (int j = 0; j < radialSegments; j++)
                {
                    int nextJ = (j + 1) % radialSegments.GetValueOrDefault();

                    var polygon = new Polygon();
                    polygon.ExteriorRing.Points.Add(ToPoint(currentCircle[j]));
                    polygon.ExteriorRing.Points.Add(ToPoint(nextCircle[j]));
                    polygon.ExteriorRing.Points.Add(ToPoint(currentCircle[nextJ]));
                    polygon.ExteriorRing.Points.Add(ToPoint(currentCircle[j]));

                    polygons.Add(polygon);

                    polygon = new Polygon();
                    polygon.ExteriorRing.Points.Add(ToPoint(currentCircle[nextJ]));
                    polygon.ExteriorRing.Points.Add(ToPoint(nextCircle[j]));
                    polygon.ExteriorRing.Points.Add(ToPoint(nextCircle[nextJ]));
                    polygon.ExteriorRing.Points.Add(ToPoint(currentCircle[nextJ]));

                    polygons.Add(polygon);
                }

                i++;
            }

            result.Geometries.AddRange(polygons);
            return result;
        }

        public static List<List<Vector3>> GetCircles(List<Vector3> path, float radius, int? radialSegments)
        {
            var circles = new List<List<Vector3>>();

            for(var i=0; i <path.Count -1; i++)
            {
                var start = path[i];
                var end = path[i + 1];

                // get the direction of the line
                var direction = Vector3.Normalize(end - start);

                var circlePointsStart = GetCirclePoints(start, direction, radius, radialSegments);
                circles.Add(circlePointsStart);

                var circlePointsEnd = GetCirclePoints(end, direction, radius, radialSegments);
                circles.Add(circlePointsEnd);
            }

            return circles;
        }

        public static List<Vector3> GetCirclePoints(Vector3 point, Vector3 direction, float radius, int? radialSegments = 8)
        {
            var points = new List<Vector3>();

            Vector3 up = new Vector3(0, 1, 0);
            Vector3 right = new Vector3(1, 0, 0);

            Vector3 v1 = Vector3.Cross(direction, up);
            if (v1.Length() < 0.0001f)
            {
                v1 = Vector3.Cross(direction, right);
            }
            v1 = Vector3.Normalize(v1);
            Vector3 v2 = Vector3.Cross(direction, v1);
            v2 = Vector3.Normalize(v2);

            float angleStep = (float)(2.0 * Math.PI / radialSegments);
            for (int i = 0; i < radialSegments; i++)
            {
                float angle = i * angleStep;
                Vector3 cirkelPoint = point +
                    (v1 * (float)Math.Cos(angle) + v2 * (float)Math.Sin(angle)) * radius;
                points.Add(cirkelPoint);
            }

            return points;

        }



    }
}
