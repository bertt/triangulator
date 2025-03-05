using NUnit.Framework;
using Wkx;

namespace Triangulate.Tests
{
    public class TriangulateLineTests
    {
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
