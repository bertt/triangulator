﻿using System;
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
            else if (geom is Polygon polygon)
            {
                return Triangulate(polygon);
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

        public static MultiPolygon Triangulate(MultiLineString lineString, float radius = 1, int radialSegments = 8)
        {
            var polygons = new List<Polygon>();

            foreach (var geom in lineString.Geometries)
            {
                var triangles = Triangulate(geom, radius, radialSegments, false);

                polygons.AddRange(triangles.Geometries);
            }

            var result = new MultiPolygon
            {
                Dimension = Dimension.Xyz
            };
            result.Geometries.AddRange(polygons);
            return result;
        }

        public static MultiPolygon Triangulate(LineString lineString, float radius, int radialSegments = 8, bool closed = false)
        {
            var path = lineString.ToVector3();
            var result = new MultiPolygon
            {
                Dimension = Dimension.Xyz
            };

            var circles = new List<List<Point>>();

            // Genereer cirkels langs het pad
            for (int i = 0; i < path.Count; i++)
            {
                var circlePoints = new List<Point>();

                var up = new Vector3(0, 1, 0);
                var direction = i < path.Count - 1 ?
                    new Vector3(
                        path[i + 1].X - path[i].X,
                        path[i + 1].Y - path[i].Y,
                        path[i + 1].Z - path[i].Z) :
                    new Vector3(
                        path[i].X - path[i - 1].X,
                        path[i].Y - path[i - 1].Y,
                        path[i].Z - path[i - 1].Z);

                var right = Vector3.Cross(up, direction);
                right = Vector3.Normalize(right);
                var newUp = Vector3.Cross(direction, right);
                newUp = Vector3.Normalize(newUp);

                // Genereer punten rond de cirkel
                for (int j = 0; j < radialSegments; j++)
                {
                    float angle = (float)(j * 2 * Math.PI / radialSegments);
                    float x = (float)Math.Cos(angle) * radius;
                    float y = (float)Math.Sin(angle) * radius;

                    Point circlePoint = new Point(
                        path[i].X + x * right.X + y * newUp.X,
                        path[i].Y + x * right.Y + y * newUp.Y,
                        path[i].Z + x * right.Z + y * newUp.Z
                    );
                    circlePoints.Add(circlePoint);
                }
                circles.Add(circlePoints);
            }

            var polygons = new List<Polygon>();

            for (int i = 0; i < circles.Count - 1; i++)
            {
                List<Point> currentCircle = circles[i];
                List<Point> nextCircle = circles[i + 1];

                for (int j = 0; j < radialSegments; j++)
                {
                    int nextJ = (j + 1) % radialSegments;

                    var polygon = new Polygon();
                    polygon.ExteriorRing.Points.Add(currentCircle[j]);
                    polygon.ExteriorRing.Points.Add(nextCircle[j]);
                    polygon.ExteriorRing.Points.Add(currentCircle[nextJ]);
                    polygon.ExteriorRing.Points.Add(currentCircle[j]);

                    polygons.Add(polygon);

                    polygon = new Polygon();
                    polygon.ExteriorRing.Points.Add(currentCircle[nextJ]);
                    polygon.ExteriorRing.Points.Add(nextCircle[j]);
                    polygon.ExteriorRing.Points.Add(nextCircle[nextJ]);
                    polygon.ExteriorRing.Points.Add(currentCircle[nextJ]);

                    polygons.Add(polygon);
                }
            }

            result.Geometries.AddRange(polygons);

            return result;
        }

        private static MultiPolygon Triangulate(Polygon polygon)
        {
            var polygons = TriangulatePolygon(polygon);
            var result = new MultiPolygon
            {
                Dimension = Dimension.Xyz
            };
            result.Geometries.AddRange(polygons);
            return result;
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
                var triangles = TriangulatePolygon(g);
                result.AddRange(triangles);
            }
            return result;
        }

        private static List<Wkx.Polygon> TriangulatePolygon(Polygon inputpolygon)
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

        private static Polygon Flatten(Polygon inputpolygon, System.Numerics.Vector3 normal)
        {
            var polygonflat = new Polygon();

            if (Math.Abs(normal.X) > Math.Abs(normal.Y) && Math.Abs(normal.X) > Math.Abs(normal.Z))
            {
                //  (yz) projection
                foreach (var p in inputpolygon.ExteriorRing.Points)
                {
                    polygonflat.ExteriorRing.Points.Add(new Point((double)p.Y, (double)p.Z));
                }
                foreach (var ring in inputpolygon.InteriorRings)
                {
                    var newRing = new LinearRing();
                    foreach (var p in ring.Points)
                    {
                        newRing.Points.Add(new Point((double)p.Y, (double)p.Z));
                    }
                    polygonflat.InteriorRings.Add(newRing);
                }
            }
            else if (Math.Abs(normal.Y) > Math.Abs(normal.Z))
            {
                // # (zx) projection
                foreach (var p in inputpolygon.ExteriorRing.Points)
                {
                    polygonflat.ExteriorRing.Points.Add(new Point((double)p.X, (double)p.Z));
                }
                foreach (var ring in inputpolygon.InteriorRings)
                {
                    var newRing = new LinearRing();
                    foreach (var p in ring.Points)
                    {
                        newRing.Points.Add(new Point((double)p.X, (double)p.Z));
                    }
                    polygonflat.InteriorRings.Add(newRing);
                }
            }
            else
            {
                // (xy) projextion
                foreach (var p in inputpolygon.ExteriorRing.Points)
                {
                    polygonflat.ExteriorRing.Points.Add(new Point((double)p.X, (double)p.Y));
                }

                foreach (var ring in inputpolygon.InteriorRings)
                {
                    var newRing = new LinearRing();
                    foreach (var p in ring.Points)
                    {
                        newRing.Points.Add(new Point((double)p.X, (double)p.Y));
                    }
                    polygonflat.InteriorRings.Add(newRing);
                }
            }

            return polygonflat;
        }

        private static Polygon GetPolygon(Polygon inputpolygon, System.Numerics.Vector3 normal, List<int> trianglesIndices, int i)
        {
            var t = new Polygon();

            var firstPoint = GetPoint(inputpolygon, trianglesIndices[i * 3]);
            t.ExteriorRing.Points.Add(firstPoint);
            t.ExteriorRing.Points.Add(GetPoint(inputpolygon, trianglesIndices[i * 3 + 1]));
            t.ExteriorRing.Points.Add(GetPoint(inputpolygon, trianglesIndices[i * 3 + 2]));
            t.ExteriorRing.Points.Add(firstPoint);

            // check crossproduct again...
            var normalTriangles = t.GetNormal();
            var dot = System.Numerics.Vector3.Dot(normal, normalTriangles);
            var mustInvert = dot < 0;

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

            foreach (var p in points)
            {
                data.Add((double)p.X);
                data.Add((double)p.Y);
            }

            foreach (var interiorRing in footprint.InteriorRings)
            {
                holeIndices.Add(data.Count/2);
                foreach (var p in interiorRing.Points)
                {
                    data.Add((double)p.X);
                    data.Add((double)p.Y);
                }
            }

            var trianglesIndices = EarcutNet.Earcut.Tessellate(data, holeIndices);
            return trianglesIndices;
        }

        private static Point GetPoint(Polygon polygon, int index)
        {
            if (index < polygon.ExteriorRing.Points.Count)
            {
                return polygon.ExteriorRing.Points[index];
            }
            else
            {
                // make a list of the vertices of the interior rings
                var interiorRingVertices = new List<Point>();
                foreach (var ring in polygon.InteriorRings)
                {
                    interiorRingVertices.AddRange(ring.Points);
                }
                return interiorRingVertices[index - polygon.ExteriorRing.Points.Count];
            }
        }
    }
}
