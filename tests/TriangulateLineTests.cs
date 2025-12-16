using NUnit.Framework;
using System.Numerics;
using System;
using Wkx;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Triangulate.Tests
{
    public class TriangulateLineTests
    {
        [Test]
        public void GenerateCirclePoints_BasicTest()
        {
            // Arrange
            var center = new Vector3(0, 0, 0);
            Vector3 direction = new Vector3(0, 1, 0); // Y-as
            float radius = 5f;
            int segments = 4;

            // Act
            var points = LineTriangulator.GetCirclePoints(
                center,
                direction,
                radius,
                segments);

            // Assert
            Assert.That(segments== points.Count, "Number of points does not match the number of segments");

            const float tolerance = 0.0001f;
            foreach (Vector3 point in points)
            {
                float distance = Vector3.Distance(center, point);
                Assert.That(Math.Abs(distance - radius) < tolerance,
                    $"Point {point} has incorrect distance to center: {distance}");
                
                Assert.That(point.Y, Is.EqualTo(0).Within(tolerance), "Eerste punt X incorrect");

            }

            Assert.That(points[0].X, Is.EqualTo(0).Within(tolerance), "Eerste punt X incorrect");
            Assert.That(points[0].Y, Is.EqualTo(0).Within(tolerance), "Eerste punt Y incorrect");
            Assert.That(points[0].Z, Is.EqualTo(-5).Within(tolerance), "Eerste punt Z incorrect");

            Assert.That(points[1].X, Is.EqualTo(-5).Within(tolerance), "Tweede punt X incorrect");
            Assert.That(points[1].Y, Is.EqualTo(0f).Within(tolerance), "Tweede punt Y incorrect");
            Assert.That(points[1].Z, Is.EqualTo(0).Within(tolerance), "Tweede punt Z incorrect");

        }


        [Test]
        public void TriangulateLine2Points()
        {
            var wkt = "LINESTRING (1 0 0, 20 0 0)";
            var line = (LineString)Geometry.Deserialize<WktSerializer>(wkt);

            var triangles = Triangulator.Triangulate(line, 2);

            Assert.That(triangles.Geometries.Count == 16);

            GltfCreator.CreateGltf(triangles, @"triangulate_line_2_points.gltf");
        }

        public Dictionary<int, Vector3> GetOuterBend(List<Vector3> circle, Vector3 endpoint)
        {
            // create a list with indices and distances
            var distances = new List<(int index, float distance)>();

            for (int i = 0; i < circle.Count; i++)
            {
                var current = circle[i];
                var distance = Vector3.Distance(current, endpoint);
                distances.Add((i, distance));
            }

            distances.Sort((a, b) => b.distance.CompareTo(a.distance));

            // get minimal the top half list of the distances
            var topHalf = distances.GetRange(0, distances.Count / 2 + 1);

            // convert to list of indices
            var indices = topHalf.ConvertAll(x => x.index);
            indices.Sort();
            var result = new Dictionary<int,Vector3>();
            foreach (var index in indices)
            {
                result.Add(index, circle[index]);
            }
            return result;
        }

        public int GetCorrespondingVertex(Dictionary<int,Vector3> input, Vector3 find)
        {
            double distance = Double.MaxValue;
            int nearestId = -1;
            foreach(var item in input)
            {
                var d = Vector3.Distance(item.Value, find);
                if (d < distance)
                {
                    distance = d;
                    nearestId = item.Key;
                }
            }

            return nearestId;
        }

        [Test]
        public void TestOuterbend()
        {
            var segments = 4;
            var wkt = "LINESTRING(-10 0 0,0 0 0,0 -10 0)";
            var line = (LineString)Geometry.Deserialize<WktSerializer>(wkt);
            var triangles = Triangulator.Triangulate(line, radius: 1, segments);

            var circles = LineTriangulator.GetCircles(line.ToVector3(), 1, segments);
            var endPoint1 = line.Points[2].ToVector3();
            // 0,1,2,3,4
            var outerBend = GetOuterBend(circles[1], endPoint1);

            var endPoint0 = line.Points[0].ToVector3();
            // 0,4,5,6,7
            var outerBend1 = GetOuterBend(circles[2], endPoint0);

            // take first point on first circle
            var bend0_start = outerBend[0];
            var bend0_end = outerBend.Last().Value;

            // find nearest point on bend1

            var nea0_start = GetCorrespondingVertex(outerBend1, bend0_start);
            var nea0_end = GetCorrespondingVertex(outerBend1, bend0_end);

            var start = outerBend1[nea0_start];
            var end = outerBend1[nea0_end];
            // triangles.Geometries.Clear();

            for (var i = 0; i < outerBend.Count-1; i++)
            {
                var vertices = new List<Vector3>();
                var p0 = outerBend.ElementAt(i).Value;
                var p1 = outerBend.ElementAt(i+1).Value;
                var p2 = outerBend1.ElementAt(i+1).Value;

                var tri = new List<Vector3>() { p0, p1, p2, p0 };
                var poly = ToPolygon(tri);
                triangles.Geometries.Add(poly);
            }

            for (var j = 1; j < outerBend1.Count-1; j++)
            {
                var vertices = new List<Vector3>();
                var p0 = outerBend1.ElementAt(j).Value;
                var p1 = outerBend1.ElementAt(j + 1).Value;
                var p2 = outerBend.ElementAt(j).Value;
                p0 = new Vector3(1, 0, 0);
                p1 = new Vector3(0, 0, 1);
                p2 = new Vector3(0, 1, 0);

                var tri = new List<Vector3>() { p0, p1, p2, p0 };
                var poly = ToPolygon(tri);
                triangles.Geometries.Add(poly);
            }

            GltfCreator.CreateGltf(triangles, "lines_triangulated.gltf");
        }

        private Polygon ToPolygon(List<Vector3> vertices)
        {
            var polygon = new Polygon();
            foreach (var v in vertices)
            {
                polygon.ExteriorRing.Points.Add(new Point(v.X, v.Y, v.Z));
            }
            return polygon;
        }

        [Test]
        public void TriangulateLine3Points()
        {
            var wkt = "LINESTRING(-10 0 0,0 0 0,0 10 0)";
            var line = (LineString)Geometry.Deserialize<WktSerializer>(wkt);

            var triangles = Triangulator.Triangulate(line, radius: 1);

            Assert.That(triangles.Geometries.Count == 32);

            GltfCreator.CreateGltf(triangles, @"triangulate_line_3_points.gltf");
        }

        [Test]
        public void TriangulateMultiLineString()
        {
            var wkt = "MULTILINESTRING Z ((-10 0 0,0 0 0,0 10 0), (1 0 0,2 0 0, 3 0 0))";

            var line = (MultiLineString)Geometry.Deserialize<WktSerializer>(wkt);

            var triangles = Triangulator.Triangulate(line);

            Assert.That(triangles.Geometries.Count == 64);

            GltfCreator.CreateGltf(triangles, @"triangulate_multiline.gltf");

        }

        [Test]
        public void TriangulateLineWithoutZ()
        {
            var wkt = "LINESTRING (1 0, 20 0)";
            var line = (LineString)Geometry.Deserialize<WktSerializer>(wkt);

            var triangles = Triangulator.Triangulate(line, 2);

            Assert.That(triangles.Geometries.Count == 16);

            GltfCreator.CreateGltf(triangles, @"triangulate_line_without_z.gltf");
        }
    }
}
