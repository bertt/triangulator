﻿using NUnit.Framework;
using Wkx;

namespace Triangulate.Tests;
public class NormalTests
{
    [Test]
    public void DoNotGetNegativeNormal()
    {
        // arrange
        var wkt = "POLYGON Z ((-75.51889015799998 39.15624977500005 17.71,-75.51880883299998 39.15605855000007 17.71,-75.51882902999995 39.156053351000025 17.71,-75.51874978099994 39.155858297000066 17.71,-75.51882395499996 39.155839280000066 17.71,-75.51889671399994 39.15601041700006 17.71,-75.51894566199996 39.15599799200004 17.71,-75.51903664399998 39.15621200700008 17.71,-75.51889015799998 39.15624977500005 17.71))";
        var polygon = (Polygon)Geometry.Deserialize<WktSerializer>(wkt);

        // act
        var normal = polygon.GetNormal();

        // assert
        Assert.That(normal.X, Is.EqualTo(0).Within(1e-11));
        Assert.That(normal.Y, Is.EqualTo(0));
        Assert.That(normal.Z, Is.EqualTo(-1));
    }

}
