using SharpGLTF.Geometry;
using SharpGLTF.Materials;
using System.Numerics;
using Wkx;

namespace Triangulate.Tests
{
    using VERTEX = SharpGLTF.Geometry.VertexTypes.VertexPosition;

    public static class GltfCreator
    {
        public static void CreateGltf(PolyhedralSurface triangulatedGeometry, string outputfile)
        {
            // convert to glTF to be able to inspect the result...
            var material1 = new MaterialBuilder()
               .WithDoubleSide(true)
               .WithSpecularGlossinessShader()
               .WithChannelParam(KnownChannel.SpecularGlossiness, new Vector4(0.7f, 0, 0f, 1.0f))
               .WithEmissive(new Vector3(0.2f, 0.3f, 0.1f));

            var mesh = new MeshBuilder<VERTEX>("mesh");

            var prim = mesh.UsePrimitive(material1);
            foreach (var t in triangulatedGeometry.Geometries)
            {
                var z0 = t.ExteriorRing.Points[0].Z??0;
                var z1 = t.ExteriorRing.Points[1].Z ?? 0;
                var z2 = t.ExteriorRing.Points[2].Z ?? 0;

                prim.AddTriangle(
                    new VERTEX((float)t.ExteriorRing.Points[0].X, (float)t.ExteriorRing.Points[0].Y, (float)z0),
                    new VERTEX((float)t.ExteriorRing.Points[1].X, (float)t.ExteriorRing.Points[1].Y, (float)z1),
                    new VERTEX((float)t.ExteriorRing.Points[2].X, (float)t.ExteriorRing.Points[2].Y, (float)z2));
            }

            var scene = new SharpGLTF.Scenes.SceneBuilder();
            scene.AddRigidMesh(mesh, Matrix4x4.Identity);
            var model = scene.ToGltf2();
            model.SaveGLTF(outputfile);
        }
    }
}
