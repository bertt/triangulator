﻿using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

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

            Assert.IsTrue(res.type == "CityJSON");
            Assert.IsTrue(res.version == "1.0");
            Assert.IsTrue(res.vertices.Length== 255);
            Assert.IsTrue(res.CityObjects.Count == 2);
        }
    }

    public class CityJSON
    {
        public string type { get; set; }
        public string version { get; set; }
        public Metadata metadata { get; set; }
        public Dictionary<string, Building> CityObjects { get; set; }
        public float[][] vertices { get; set; }
    }

    public class Metadata
    {
        public float[] geographicalExtent { get; set; }
    }

    public class Building
    {
        public Geometry[] geometry { get; set; }
        public string type { get; set; }
    }

    public class Geometry
    {
        public int[][][] boundaries { get; set; }
        public int lod { get; set; }
        public Semantics semantics { get; set; }
        public Texture texture { get; set; }
        public string type { get; set; }
    }

    public class Semantics
    {
        public Surface[] surfaces { get; set; }
        public int[] values { get; set; }
    }

    public class Surface
    {
        public string type { get; set; }
    }

    public class Texture
    {
        public RhinoTexturing Rhinotexturing { get; set; }
    }

    public class RhinoTexturing
    {
        public int[][][] values { get; set; }
    }
}