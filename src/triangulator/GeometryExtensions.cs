using Wkx;

namespace Triangulate
{
    public static class GeometryExtensions
    {
        public static PolyhedralSurface ToPolyhedralSurface(this MultiPolygon multipolygon)
        {
            var polyhedralsurface = new PolyhedralSurface
            {
                Dimension = Dimension.Xyz
            };

            foreach (var polygon in multipolygon.Geometries)
            {
                polyhedralsurface.Geometries.Add(polygon);
            }

            return polyhedralsurface;
        }

        public static PolyhedralSurface ToPolyhedralSurface(this MultiLineString multilinestring)
        {
            var polyhedralsurface = new PolyhedralSurface
            {
                Dimension = Dimension.Xyz
            };

            foreach (var linestring in multilinestring.Geometries)
            {
                var polygon = GetPolygon(linestring);
                polyhedralsurface.Geometries.Add(polygon);
            }

            return polyhedralsurface;
        }

        private static Polygon GetPolygon(LineString linestring)
        {
            var polygon = new Polygon()
            {
                Dimension = Dimension.Xyz
            };

            polygon.ExteriorRing.Points.AddRange(linestring.Points);
            polygon.ExteriorRing.Points.Add(linestring.Points[linestring.Points.Count - 1]);

            return polygon;
        }
    }
}
