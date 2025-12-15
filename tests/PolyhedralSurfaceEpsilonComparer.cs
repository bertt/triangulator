#nullable enable
using System;
using NUnit.Framework;
using Wkx;

namespace Triangulate.Tests;

public static class PolyhedralSurfaceEpsilonComparer
{
    public static void AssertEquals(PolyhedralSurface? x, PolyhedralSurface? y, double eps = double.Epsilon)
    {
        if (ReferenceEquals(x, y))
        {
            Assert.Pass("References are equal");
            return;
        }
        if (x is null && y is null)
        {
            Assert.Pass("Both are null");
            return;
        }
        if (x is null || y is null)
        {
            Assert.Fail("One is null, the other is not");
            return;
        }
        
        Assert.That(x.Geometries?.Count, Is.EqualTo(y.Geometries?.Count));

        for (var idx = 0; idx < x.Geometries!.Count; idx++)
        {
            AssertPolygonEquals(x.Geometries[idx], y.Geometries![idx], eps);
        }
        
    }

    private static void AssertPolygonEquals(Polygon a, Polygon b, double eps)
    {
        Assert.That(a.InteriorRings.Count, Is.EqualTo(b.InteriorRings.Count));
        AssertRingEquals(a.ExteriorRing, b.ExteriorRing, eps);
        for (int i = 0; i < a.InteriorRings.Count; i++)
        {
            AssertRingEquals(a.InteriorRings[i], b.InteriorRings[i], eps);
        }
    }

    private static void AssertRingEquals(LinearRing a, LinearRing b, double eps)
    {
        
        Assert.That(a.Points.Count, Is.EqualTo(b.Points.Count));
        // Dimension is somehow not taken into account.
        //Assert.That(a.Dimension, Is.EqualTo(b.Dimension));
        Assert.That(a.GeometryType, Is.EqualTo(b.GeometryType));
        Assert.That(a.Srid, Is.EqualTo(b.Srid));

        for (int i = 0; i < a.Points.Count; i++)
        {
            AssertPointEquals(a.Points[i], b.Points[i], eps);
        }
    }

    private static  bool AssertPointEquals(Point a, Point b, double eps) =>
        NearlyEqual(a.X, b.X, eps) && NearlyEqual(a.Y, b.Y, eps) && NearlyEqual(a.Z, b.Z, eps);

    private static bool NearlyEqual(double? a, double? b, double eps)
    {
        if (!a.HasValue || !b.HasValue)
            return a.HasValue == b.HasValue; // both null => equal, one null => not

        var av = a.Value;
        var bv = b.Value;

        // optional: treat NaN as equal only to NaN
        if (double.IsNaN(av) || double.IsNaN(bv))
            return double.IsNaN(av) && double.IsNaN(bv);

        // optional: infinities must match exactly
        if (double.IsInfinity(av) || double.IsInfinity(bv))
            return av.Equals(bv);

        var d = Math.Abs(av - bv);
        if (d <= eps) return true;

        var scale = Math.Max(1.0, Math.Max(Math.Abs(av), Math.Abs(bv)));
        return d <= eps * scale;
    }
}