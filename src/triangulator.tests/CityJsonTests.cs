using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Triangulate.Tests
{
    public class CityJsonTests
    {
        [Test]
        public void FirstCityJsonParsingTest()
        {
            var file = @"./testdata/twobuildings.json";
            var json = File.ReadAllText(file);

            var res = JsonConvert.DeserializeObject<CityJSON>(json);

            Assert.IsTrue(res.Type == "CityJSON");
            Assert.IsTrue(res.Version == "1.0");
            Assert.IsTrue(res.Vertices.Length== 255);
            Assert.IsTrue(res.CityObjects.Count == 2);
            var firstBuilding = res.CityObjects.First();
            var building = firstBuilding.Value;
            Assert.IsTrue(building.Type == "Building");
            Assert.IsTrue(building.Geometry.Length == 1);
            Assert.IsTrue(building.Geometry.First().Type == "MultiSurface");
            Assert.IsTrue(building.Geometry.First().Boundaries.Length == 110);
            Assert.IsTrue(building.Geometry.First().Boundaries[0].Length == 1);
            Assert.IsTrue(building.Geometry.First().Boundaries[0][0].Length == 3);
            Assert.IsTrue(building.Geometry.First().Boundaries[0][0][0] == 198);
            Assert.IsTrue(building.Geometry.First().Boundaries[0][0][1] == 199);
            Assert.IsTrue(building.Geometry.First().Boundaries[0][0][2] == 200);
        }
    }

    public class CityJSON
    {
        public string Type { get; set; }
        public string Version { get; set; }
        public Metadata Metadata { get; set; }
        public Dictionary<string, Building> CityObjects { get; set; }
        public float[][] Vertices { get; set; }
    }

    public class Metadata
    {
        public float[] GeographicalExtent { get; set; }
    }

    public class Building
    {
        public Geometry[] Geometry { get; set; }
        public string Type { get; set; }
    }

    public class Geometry
    {
        public int[][][] Boundaries { get; set; }
        public int Lod { get; set; }
        public Semantics Semantics { get; set; }
        public Texture Texture { get; set; }
        public string Type { get; set; }
    }

    public class Semantics
    {
        public Surface[] Surfaces { get; set; }
        public int[] Values { get; set; }
    }

    public class Surface
    {
        public string Type { get; set; }
    }

    public class Texture
    {
        public RhinoTexturing Rhinotexturing { get; set; }
    }

    public class RhinoTexturing
    {
        public int[][][] Values { get; set; }
    }
}