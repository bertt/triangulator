using NUnit.Framework;
using System.Numerics;
using System;
using Wkx;

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
