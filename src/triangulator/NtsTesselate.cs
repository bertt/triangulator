using NetTopologySuite.IO;
using NetTopologySuite.Triangulate;
using System.Collections.Generic;

namespace Triangulate;
public static class NtsTesselate
{
    public static List<string> Tesselate(string Wkt)
    {
        var geom = new WKTReader().Read(Wkt);
        var dtb = new DelaunayTriangulationBuilder();
        dtb.SetSites(geom);
        var resultTriangles = dtb.GetTriangles(geom.Factory);
        var result = new List<string>();
        foreach (var t in resultTriangles)
        {
            result.Add(t.AsText());
        }
        return result;
    }

}
