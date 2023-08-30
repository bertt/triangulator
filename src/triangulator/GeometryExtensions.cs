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
    }
}
