using NUnit.Framework;
using OSGeo.OGR;
using Wkx;

namespace Triangulate.Tests
{
    public class CityGmlTests
    {
        // [Test]
        // disabled this test because there are issues with OSGeo.OGR dependency on buildserver
        public void FirstTestCityGml()
        {
            Ogr.RegisterAll();
            // source: Sample input files: https://www.opengeodata.nrw.de/produkte/geobasis/3dg/lod2_gml/
            var file = @"./testdata/LoD2_280_5657_1_NW.gml";
            var gmlDriver = Ogr.GetDriverByName("GML");
            var dsGml = gmlDriver.Open(file, 0);
            var numberOfLayers = dsGml.GetLayerCount();
            Assert.IsTrue(numberOfLayers == 1);
            var buildingLayer = dsGml.GetLayerByName("building");
            var featuresGml = buildingLayer.GetFeatureCount(0);
            Assert.IsTrue(featuresGml == 82);

            // take first geometry
            var featureGml = buildingLayer.GetNextFeature();

            var geometry = featureGml.GetGeometryRef();
            var wkt = string.Empty;
            geometry.ExportToWkt(out wkt);
            Assert.IsTrue(wkt.Contains("MULTILINESTRING"));
            var multilinestring = (MultiLineString)Wkx.Geometry.Deserialize<WktSerializer>(wkt);

            Assert.IsTrue(multilinestring.Geometries.Count == 5);

            var polyhedral= multilinestring.ToPolyhedralSurface();

            var triangulatedPolyhedral = Triangulator.Triangulate(polyhedral);

            // todo: improve results...
            // turns out the mulitlinestrings are not the solids...
            // so the following gltf is a mess
            // needs something different for parsing gml
            GltfCreator.CreateGltf(triangulatedPolyhedral, @"gml.gltf");
        }
    }
}
