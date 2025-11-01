using NUnit.Framework;
using System.Numerics;
using System;
using Wkx;
using System.Collections.Generic;

namespace Triangulate.Tests
{
    public class BendTests
    {
        [Test]
        public void TestBendCircleCount()
        {
            // Line with a 90-degree bend: 3 points
            var wkt = "LINESTRING(-10 0 0, 0 0 0, 0 -10 0)";
            var line = (LineString)Geometry.Deserialize<WktSerializer>(wkt);
            
            var circles = LineTriangulator.GetCircles(line.ToVector3(), 1, 8);
            
            // Should have exactly one circle per point
            Assert.That(circles.Count, Is.EqualTo(line.Points.Count));
        }

        [Test]
        public void TestBendCircleContinuity()
        {
            // Line with a 90-degree bend
            var wkt = "LINESTRING(-10 0 0, 0 0 0, 0 -10 0)";
            var line = (LineString)Geometry.Deserialize<WktSerializer>(wkt);
            
            var circles = LineTriangulator.GetCircles(line.ToVector3(), 1, 8);
            
            // Verify that adjacent circles have reasonable spacing
            for (int i = 0; i < circles.Count - 1; i++)
            {
                float maxDist = 0;
                float minDist = float.MaxValue;
                
                for (int j = 0; j < circles[i].Count; j++)
                {
                    var dist = Vector3.Distance(circles[i][j], circles[i+1][j]);
                    maxDist = Math.Max(maxDist, dist);
                    minDist = Math.Min(minDist, dist);
                }
                
                // Adjacent circles should have relatively uniform spacing
                // (max distance shouldn't be more than 3x the min distance)
                float ratio = maxDist / minDist;
                Assert.That(ratio, Is.LessThan(3.0f), 
                    $"Circle {i} to {i+1}: spacing ratio {ratio:F2} is too large (min={minDist:F4}, max={maxDist:F4})");
            }
        }

        [Test]
        public void TestBendTriangleCount()
        {
            // Line with a 90-degree bend: 3 points, 2 segments
            var wkt = "LINESTRING(-10 0 0, 0 0 0, 0 -10 0)";
            var line = (LineString)Geometry.Deserialize<WktSerializer>(wkt);
            
            var triangles = Triangulator.Triangulate(line, radius: 1, radialSegments: 8);
            
            // With 3 points and 8 radial segments, we should have:
            // 2 circle pairs * 8 radial segments * 2 triangles per segment = 32 triangles
            Assert.That(triangles.Geometries.Count, Is.EqualTo(32));
        }

        [Test]
        public void TestSharpBend()
        {
            // Line with a sharp 135-degree bend
            var wkt = "LINESTRING(0 0 0, 10 0 0, 5 5 0)";
            var line = (LineString)Geometry.Deserialize<WktSerializer>(wkt);
            
            var circles = LineTriangulator.GetCircles(line.ToVector3(), 1, 8);
            
            // Should still have one circle per point
            Assert.That(circles.Count, Is.EqualTo(3));
            
            var triangles = Triangulator.Triangulate(line, radius: 1, radialSegments: 8);
            
            // Should have proper number of triangles
            Assert.That(triangles.Geometries.Count, Is.EqualTo(32));
        }

        [Test]
        public void TestMultipleBends()
        {
            // Line with multiple bends (zigzag)
            var wkt = "LINESTRING(0 0 0, 10 0 0, 10 10 0, 20 10 0, 20 20 0)";
            var line = (LineString)Geometry.Deserialize<WktSerializer>(wkt);
            
            var circles = LineTriangulator.GetCircles(line.ToVector3(), 1, 8);
            
            // 5 points should give 5 circles
            Assert.That(circles.Count, Is.EqualTo(5));
            
            var triangles = Triangulator.Triangulate(line, radius: 1, radialSegments: 8);
            
            // 4 segments * 8 radial segments * 2 triangles = 64 triangles
            Assert.That(triangles.Geometries.Count, Is.EqualTo(64));
        }
    }
}
