using NUnit.Framework;
using Wkx;

namespace Triangulate.Tests
{
    public class TriangulateLineTests
    {
        [Test]
        public void TriangulateLineOnly2Points()
        {
            var wkt = "LINESTRING (1 0 0, 20 0 0)";
            var line = (LineString)Geometry.Deserialize<WktSerializer>(wkt);

            var triangles = Triangulator.Triangulate(line, 2);

            Assert.That(triangles.Geometries.Count == 16);

            GltfCreator.CreateGltf(triangles, @"carmullromcurves.gltf");
        }


        [Test]
        public void TriangulateLine()
        {
            var wkt = "LINESTRING(-10 0 0,0 0 0,0 10 0)";
            var line = (LineString)Geometry.Deserialize<WktSerializer>(wkt);

            var triangles = Triangulator.Triangulate(line, radius: 1);

            Assert.That(triangles.Geometries.Count == 32);

            GltfCreator.CreateGltf(triangles, @"linecurves.gltf");
        }

        [Test]
        public void TriangulateMultiLineString()
        {
            var wkt = "MULTILINESTRING Z ((-10 0 0,0 0 0,0 10 0), (1 0 0,2 0 0, 3 0 0))";

            var line = (MultiLineString)Geometry.Deserialize<WktSerializer>(wkt);

            var triangles = Triangulator.Triangulate(line);

            Assert.That(triangles.Geometries.Count == 64);

            GltfCreator.CreateGltf(triangles, @"multilines.gltf");

        }

        [Test]
        public void TriangulateLineWithoutZ()
        {
            var wkt = "LINESTRING (1 0, 20 0)";
            var line = (LineString)Geometry.Deserialize<WktSerializer>(wkt);

            var triangles = Triangulator.Triangulate(line, 2);

            Assert.That(triangles.Geometries.Count == 16);

            GltfCreator.CreateGltf(triangles, @"carmullromcurves.gltf");
        }
    }
}
