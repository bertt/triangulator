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

            Assert.That(triangles.Geometries.Count == 48);

            GltfCreator.CreateGltf(triangles, @"triangulate_line_3_points.gltf");
        }

        [Test]
        public void TriangulateMultiLineString()
        {
            var wkt = "MULTILINESTRING Z ((-10 0 0,0 0 0,0 10 0), (1 0 0,2 0 0, 3 0 0))";

            var line = (MultiLineString)Geometry.Deserialize<WktSerializer>(wkt);

            var triangles = Triangulator.Triangulate(line);

            Assert.That(triangles.Geometries.Count == 96);

            GltfCreator.CreateGltf(triangles, @"triangu.gltf");

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


        [Test]
        public void TriangulateGasPipeline()
        {
            var wkt = "LINESTRING(-7.1959060416556895 -3.0653913647984155 6.5166643764823675,-7.0126300300471485 -2.980768570676446 6.368475022725761,-6.980049450881779 -3.079394771833904 6.350719616748393)";
            var line = (LineString)Geometry.Deserialize<WktSerializer>(wkt);

            var triangles = Triangulator.Triangulate(line, 0.055f);

            GltfCreator.CreateGltf(triangles, @"triangulate_gasline.gltf");
        }

        [Test]
        public void TriangulateGasPipeline1()
        {
            var wkt = "LINESTRING(-8.079088067635894 -4.1475718496367335 7.713269264437258,-7.894537712447345 -4.066954427224118 7.564396608620882,-8.126223187893629 -3.3122587064863183 7.68663608469069)";
            var line = (LineString)Geometry.Deserialize<WktSerializer>(wkt);

            var triangles = Triangulator.Triangulate(line, 0.055f);

            GltfCreator.CreateGltf(triangles, @"triangulate_gasline2.gltf");
        }
    }
}
