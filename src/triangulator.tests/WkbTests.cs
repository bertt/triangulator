using NUnit.Framework;
using System.IO;
using Wkx;

namespace Triangulate.Tests
{
    public class Tests
    {
        [Test]
        public void TriangulateWkbTest()
        {
            var buildingWkb = File.ReadAllBytes(@"testdata/building.wkb");
            var wkbTriangulated = Triangulator.Triangulate(buildingWkb);
            var polyhedral = (PolyhedralSurface)Wkx.Geometry.Deserialize<WkbSerializer>(wkbTriangulated);
            Assert.IsTrue(polyhedral.Geometries.Count == 22);

            GltfCreator.CreateGltf(polyhedral, @"wkb.gltf");
        }

    }
}