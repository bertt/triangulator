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
            }

            result.Geometries.AddRange(polygons);
            return result;
        }

        public static List<List<Vector3>> GetCircles(List<Vector3> path, float radius, int? radialSegments)
        {
            var circles = new List<List<Vector3>>();

            for (var i = 0; i < path.Count; i++)
            {
                Vector3 direction;

                if (i == 0)
                {
                    // First point: use direction to next point
                    direction = Vector3.Normalize(path[i + 1] - path[i]);
                }
                else if (i == path.Count - 1)
                {
                    // Last point: use direction from previous point
                    direction = Vector3.Normalize(path[i] - path[i - 1]);
                }
                else
                {
                    // Interior point: use average of directions from previous and to next segment
                    var directionToPrev = Vector3.Normalize(path[i] - path[i - 1]);
                    var directionToNext = Vector3.Normalize(path[i + 1] - path[i]);
                    var averageDirection = directionToPrev + directionToNext;
                    
                    // Handle 180-degree bends where the sum might be near zero
                    if (averageDirection.Length() < 0.0001f)
                    {
                        // Use direction from previous segment for sharp reversal
                        direction = directionToPrev;
                    }
                    else
                    {
                        direction = Vector3.Normalize(averageDirection);
                    }
                }

                var circlePoints = GetCirclePoints(path[i], direction, radius, radialSegments);
                circles.Add(circlePoints);
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
                Vector3 circlePoint = point +
                    (v1 * (float)Math.Cos(angle) + v2 * (float)Math.Sin(angle)) * radius;
                points.Add(circlePoint);
            }

            return points;

        }



    }
}
