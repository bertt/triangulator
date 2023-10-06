using System;
using System.Collections.Generic;
using System.Numerics;
using Wkx;
using SharpGeometry.Triangulation;

namespace Triangulate
{
    public static class Triangulator
    {
        public static Geometry Triangulate(Geometry geom)
        {
            if (geom is PolyhedralSurface polyhedral)
            {
                return Triangulate(polyhedral);
            }
            else if (geom is MultiPolygon multiPolygon)
            {
                return Triangulate(multiPolygon);
            }
            else
            {
                throw new NotSupportedException($"Geometry type {geom.GeometryType} is not supported");
            }
        }       
        public static byte[] Triangulate(byte[] wkb)
        {
            var geom = Geometry.Deserialize<WkbSerializer>(wkb);
            var result = Triangulate(geom);
            return result.AsBinary();
        }

        private static MultiPolygon Triangulate(MultiPolygon multipolygon)
        {
            var result = new MultiPolygon
            {
                Dimension = Dimension.Xyz
            };

            result.Geometries.AddRange(GetTriangles(multipolygon.Geometries));
            return result;
        }

        private static PolyhedralSurface Triangulate(PolyhedralSurface polyhedral)
        {
            var result = new PolyhedralSurface
            {
                Dimension = Dimension.Xyz
            };

            result.Geometries.AddRange(GetTriangles(polyhedral.Geometries));

            return result;
        }


        private static List<Polygon> GetTriangles(List<Polygon> geometries)
        {
            var result = new List<Polygon>();
            foreach (var g in geometries)
            {
                var triangles = Triangulate(g);
                result.AddRange(triangles);
            }
            return result;
        }

        private static List<Polygon> Triangulate(Polygon inputpolygon)
        {
            var points = inputpolygon.ExteriorRing.Points;
            points.RemoveAt(points.Count - 1);
            var v3 = new List<Vector3>();
            foreach(var point in points)
            {
                v3.Add(point.ToVector3());
            }
            
            var triangles = PolygonTriangulator.Default.Triangulate(v3);

            var polygons = new List<Polygon>();
            foreach(var t in triangles)
            {
                var polygon = new Polygon();
                var point0 = new Point(t.PositionA.X, t.PositionA.Y, t.PositionA.Z);
                var point1 = new Point(t.PositionB.X, t.PositionB.Y, t.PositionB.Z);
                var point2 = new Point(t.PositionC.X, t.PositionC.Y, t.PositionC.Z);

                polygon.ExteriorRing.Points.Add(point0);
                polygon.ExteriorRing.Points.Add(point1);
                polygon.ExteriorRing.Points.Add(point2);
                polygon.ExteriorRing.Points.Add(point0);
                polygon.Dimension = Dimension.Xyz;

                polygons.Add(polygon);
            }
            return polygons;
        }
    }
}
