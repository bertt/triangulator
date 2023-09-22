using NUnit.Framework;
using System.Numerics;
using System.Text;
using Wkx;

namespace Triangulate.Tests;
public class PolygonExtensionsTests
{
    [Test]
    public void NormalTest()
    {
        // Test for a polygon with a normal that is not 0,0,0
        // Bag id: NL.IMBAG.Pand.0289100000020984
        // arrange
        var wkt = "POLYGON Z ((173105.4583575502 444132.6294281028 55.79433985427022,173105.45835767197 444132.62943031423 55.64733954053372,173105.45836112826 444132.6294931242 51.47633953765035,173107.0364873325 444133.3794869664 51.4763396801427,173107.03648509335 444133.3794462814 54.1783398212865,173105.4583575502 444132.6294281028 55.79433985427022))";
        var geom = (Polygon)Geometry.Deserialize<WktSerializer>(Encoding.UTF8.GetBytes(wkt));

        // act
        var normal = geom.GetNormal();

        //assert
        Assert.AreNotEqual(normal, new Vector3(0,0,0));
    }
}
