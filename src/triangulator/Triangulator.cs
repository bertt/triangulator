using EarcutNet;
using System;
using System.Collections.Generic;
using System.Numerics;
using Wkx;

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
            var normal = inputpolygon.GetNormal();
            var polygonflat = Flatten(inputpolygon, normal);
            var trianglesIndices = Tesselate(polygonflat);

            var polygons = new List<Polygon>();
            for (var i = 0; i < trianglesIndices.Count / 3; i++)
            {
                var poly = GetPolygon(inputpolygon, normal, trianglesIndices, i);
                polygons.Add(poly);
            }
            return polygons;
        }

        private static Polygon Flatten(Polygon inputpolygon, Vector3 normal)
        {
            var polygonflat = new Polygon();

            if (Math.Abs(normal.X) > Math.Abs(normal.Y) && Math.Abs(normal.X) > Math.Abs(normal.Z))
            {
                //  (yz) projection
                foreach (var p in inputpolygon.ExteriorRing.Points)
                {
                    polygonflat.ExteriorRing.Points.Add(new Point((double)p.Y, (double)p.Z));
                }
            }
            else if (Math.Abs(normal.Y) > Math.Abs(normal.Z))
            {
                // # (zx) projection
                foreach (var p in inputpolygon.ExteriorRing.Points)
                {
                    polygonflat.ExteriorRing.Points.Add(new Point((double)p.X, (double)p.Z));
                }
            }
            else
            {
                // (xy) projextion
                foreach (var p in inputpolygon.ExteriorRing.Points)
                {
                    polygonflat.ExteriorRing.Points.Add(new Point((double)p.X, (double)p.Y));
                }
            }

            return polygonflat;
        }

        private static Polygon GetPolygon(Polygon inputpolygon, Vector3 normal, List<int> trianglesIndices, int i)
        {
            var t = new Polygon();

            var firstPoint = GetPoint(inputpolygon, trianglesIndices[i * 3]);
            t.ExteriorRing.Points.Add(firstPoint);
            t.ExteriorRing.Points.Add(GetPoint(inputpolygon, trianglesIndices[i * 3 + 1]));
            t.ExteriorRing.Points.Add(GetPoint(inputpolygon, trianglesIndices[i * 3 + 2]));
            t.ExteriorRing.Points.Add(firstPoint);

            // check crossproduct again...
            var normalTriangles = t.GetNormal();
            var dot = Vector3.Dot(normal, normalTriangles);
            var mustInvert =  dot < 0;

            if (mustInvert)
            {
                t = InvertPolygon(t);
            }

            t.Dimension = Dimension.Xyz;
            return t;
        }

        private static Polygon InvertPolygon(Polygon t)
        {
            var res = new Polygon();
            res.ExteriorRing.Points.Add(t.ExteriorRing.Points[1]);
            res.ExteriorRing.Points.Add(t.ExteriorRing.Points[0]);
            res.ExteriorRing.Points.Add(t.ExteriorRing.Points[2]);
            res.ExteriorRing.Points.Add(t.ExteriorRing.Points[1]);
            t = res;
            return t;
        }

        private static List<int> Tesselate(Polygon footprint)
        {
            var points = footprint.ExteriorRing.Points;

            var data = new List<double>();
            var holeIndices = new List<int>();

            for(var p=0;p< points.Count-1;p++)
            {
                data.Add((double)points[p].X);
                data.Add((double)points[p].Y);
            }

            var trianglesIndices = Earcut.Tessellate(data, holeIndices);
            return trianglesIndices;
        }

        private static Point GetPoint(Polygon polygon, int index)
        {
            return polygon.ExteriorRing.Points[index];
        }
    }
}
