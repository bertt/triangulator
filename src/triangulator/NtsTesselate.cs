using NetTopologySuite.IO;
using NetTopologySuite.Triangulate;
using System.Collections.Generic;

namespace Triangulate;
public static class NtsTesselate
{
    public static List<string> Tesselate(string Wkt)
    {
        var geom = new WKTReader().Read("POLYGON Z ((-75.5316053939999 39.093700497 0,-75.531553209 39.093692102 0,-75.531565147 39.093647042 0,-75.5316173309999 39.0936554370001 0,-75.531653144 39.0935202580001 0,-75.531809698 39.093545444 0,-75.5317500099999 39.0937707430001 0,-75.5315934559999 39.093745557 0,-75.5316053939999 39.093700497 0))");
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
