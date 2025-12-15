#nullable enable
using NUnit.Framework;
using Wkx;

namespace Triangulate.Tests;

public static class PolyhedralSurfaceAssertions
{
    public static void AssertPolyhedralEqualSimple(PolyhedralSurface a, PolyhedralSurface b)
    {
        for (var i = 0; i < a.Geometries.Count; i++)
        {
            Assert.That(a.Geometries[i].ExteriorRing.Points.Count, Is.EqualTo(b.Geometries[i].ExteriorRing.Points.Count));
        }
    }
}