using System;
using System.Collections.Generic;
using System.Numerics;
using Wkx;

namespace Triangulate
{
    public static class Triangulator
    {
        public static PolyhedralSurface Triangulate(PolyhedralSurface polyhedral)
        {
            var result = new PolyhedralSurface
            {
                Dimension = Dimension.Xyz
            };
            foreach (var g in polyhedral.Geometries)
            {
                var triangles = Triangulate(g);
                result.Geometries.AddRange(triangles);
            }

            return result;
        }

        public static byte[] Triangulate(byte[] wkb)
        {
            var polyhedral = (PolyhedralSurface)Geometry.Deserialize<WkbSerializer>(wkb);
            var triangulatedPolyhedral = Triangulate(polyhedral);
            return triangulatedPolyhedral.AsBinary();
        }

        public static List<Polygon> Triangulate(Polygon inputpolygon)
        {
            var normal = inputpolygon.GetNormal();
            var polygonflat = Flatten(inputpolygon, normal);
            var wkt = polygonflat.SerializeString<WktSerializer>();
            var wkts = NtsTesselate.Tesselate(wkt);

            var polygons = new List<Polygon>();
            foreach(var w in wkts)
            {
                polygons.Add((Polygon)Geometry.Deserialize<WktSerializer>(w));
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
    }
}
