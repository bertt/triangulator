using NUnit.Framework;
using OSGeo.OGR;
using System;
using Wkx;

namespace Triangulate.Tests
{
    public class CityGmlTests
    {
        [Test]
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
            // featureGml = buildingLayer.GetNextFeature();

            var geometry = featureGml.GetGeometryRef();
            var wkt = string.Empty;
            geometry.ExportToWkt(out wkt);
            Assert.IsTrue(wkt.Contains("MULTILINESTRING"));
            var multilinestring = (MultiLineString)Wkx.Geometry.Deserialize<WktSerializer>(wkt);

            Assert.IsTrue(multilinestring.Geometries.Count == 5);

            var polyhedral= GetPolyhedral(multilinestring);

            var triangulatedPolyhedral = Triangulator.Triangulate(polyhedral);

            // todo: improve results...

            GltfCreator.CreateGltf(triangulatedPolyhedral, @"gml.gltf");
        }

        private PolyhedralSurface GetPolyhedral(MultiLineString multilinestring) 
        { 
            var polyhedralsurface = new PolyhedralSurface
            {
                Dimension = Dimension.Xyz
            };

            foreach(var linestring in multilinestring.Geometries)
            {
                var polygon = GetPolygon(linestring);
                polyhedralsurface.Geometries.Add(polygon);
            }

            return polyhedralsurface;
        }

        private Polygon GetPolygon(LineString linestring)
        {
            var polygon = new Polygon()
            {
                Dimension = Dimension.Xyz
            };

            polygon.ExteriorRing.Points.AddRange(linestring.Points);
            polygon.ExteriorRing.Points.Add(linestring.Points[linestring.Points.Count-1]);

            return polygon;
        }
    }
}
