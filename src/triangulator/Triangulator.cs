using EarcutNet;
using System;
using System.Collections.Generic;
using System.Numerics;
using Wkx;

namespace triangulator
{
    public static class Triangulator
    {
        public static List<Polygon> Triangulate(byte[] wkb)
        {
            var polyhedral = (PolyhedralSurface)Geometry.Deserialize<WkbSerializer>(wkb);

            var allTriangles = new List<Polygon>();
            foreach (var g in polyhedral.Geometries)
            {
                var triangles = Triangulate(g);
                allTriangles.AddRange(triangles);
            }

            return allTriangles;
        }

        private static List<Polygon> Triangulate(Polygon inputpolygon)
        {
            var normal = inputpolygon.GetNormal();
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

            var trianglesIndices = Triangulator.Tesselate(polygonflat);

            var polygons = new List<Polygon>();
            for (var i = 0; i < trianglesIndices.Count / 3; i++)
            {
                var t = new Polygon();

                var firstPoint = Unflatten(inputpolygon, trianglesIndices[i * 3]);
                t.ExteriorRing.Points.Add(firstPoint);
                t.ExteriorRing.Points.Add(Unflatten(inputpolygon, trianglesIndices[i * 3 + 1]));
                t.ExteriorRing.Points.Add(Unflatten(inputpolygon, trianglesIndices[i * 3 + 2]));
                t.ExteriorRing.Points.Add(firstPoint);

                // check crossproduct again...
                var normalTriangles = t.GetNormal();
                var mustInvert = Vector3.Dot(normal, normalTriangles) < 0;

                if (mustInvert)
                {
                    var res = new Polygon();
                    res.ExteriorRing.Points.Add(t.ExteriorRing.Points[1]);
                    res.ExteriorRing.Points.Add(t.ExteriorRing.Points[0]);
                    res.ExteriorRing.Points.Add(t.ExteriorRing.Points[2]);
                    res.ExteriorRing.Points.Add(t.ExteriorRing.Points[1]);
                    t = res;
                }

                polygons.Add(t);
            }
            return polygons;
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

        private static Point Unflatten(Polygon polygon, int index)
        {
            return polygon.ExteriorRing.Points[index];
        }
    }
}
