using System.IO;
using System.Text;
using Wkx;

namespace Triangulate
{
    public static class GeometryExtensions
    {
        public static string AsText(this Geometry geometry)
        {
            var stream = new MemoryStream();
            geometry.Serialize<WktSerializer>(stream);
            var wkt = Encoding.UTF8.GetString(stream.ToArray());
            return wkt;
        }

        public static byte[] AsBinary(this Geometry geometry)
        {
            var stream = new MemoryStream();
            geometry.Serialize<WkbSerializer>(stream);
            return stream.ToArray();
        }

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
            polygon.ExteriorRing.Points.Add(linestring.Points[^1]);

            return polygon;
        }
    }
}
