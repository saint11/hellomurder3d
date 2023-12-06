using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Murder.Utilities;
using Newtonsoft.Json;
using Sledge.Formats.Map.Objects;
using System.Collections.Immutable;
using System.Globalization;

namespace HelloMurder.Core
{
    public class Model3D
    {
        public class Mesh
        {
            public string Name = "";
            public int VertexStart;
            public int VertexCount;
        }

        [JsonProperty]
        private VertexPositionTexture[] _verts; // Not immutable because Monogame can't handle it
        public Model3D() { }

        public Model3D(List<VertexPositionTexture> vertices)
        {
            _verts = vertices.ToArray();
        }

        public void Draw(Effect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Murder.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _verts, 0, _verts.Length / 3);
            }
        }

        public static Model3D CreateFromObj(string filename)
        {
            var model = new Model3D();
            var verts = new List<VertexPositionTexture>();
            var vertices = new List<Vector3>();
            var uvs = new List<Vector2>();

            Mesh mesh = null;

            if (File.Exists(filename + ".export"))
            {
                using (var reader = new BinaryReader(File.OpenRead(filename + ".export")))
                {
                    var count = reader.ReadInt32();
                    for (int m = 0; m < count; m++)
                    {
                        if (mesh != null)
                            mesh.VertexCount = verts.Count - mesh.VertexStart;

                        mesh = new Mesh();
                        mesh.Name = reader.ReadString();
                        mesh.VertexStart = verts.Count;
                        // Not used yet
                        // model.Meshes.Add(mesh);

                        int vertexCount = reader.ReadInt32();
                        for (int i = 0; i < vertexCount; i++)
                        {
                            var a = reader.ReadSingle();
                            var b = reader.ReadSingle();
                            var c = reader.ReadSingle();
                            vertices.Add(new Vector3(a, b, c));
                        }

                        int uvCount = reader.ReadInt32();
                        for (int i = 0; i < uvCount; i++)
                        {
                            var a = reader.ReadSingle();
                            var b = reader.ReadSingle();
                            uvs.Add(new Vector2(a, b));
                        }

                        int faceCount = reader.ReadInt32();
                        for (int i = 0; i < faceCount; i++)
                        {
                            var pos = reader.ReadInt32() - 1;
                            var uv = reader.ReadInt32() - 1;

                            verts.Add(new VertexPositionTexture()
                            {
                                Position = vertices[pos],
                                TextureCoordinate = uvs[uv],
                            });
                        }
                    }
                }
            }
            else
            {
                using (var reader = new StreamReader(filename))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var data = line.Split(' ');
                        if (data.Length <= 0)
                            continue;

                        var command = data[0];

                        // Start of Object
                        if (command == "o")
                        {
                            if (mesh != null)
                                mesh.VertexCount = verts.Count - mesh.VertexStart;

                            mesh = new Mesh();
                            mesh.Name = data[1];
                            mesh.VertexStart = verts.Count;
                            // Not used yet
                            // model.Meshes.Add(mesh);
                        }
                        // Add Vertex
                        else if (command == "v")
                        {
                            var position = new Vector3(Float(data[1]), Float(data[2]), Float(data[3]));
                            vertices.Add(position);
                        }
                        // Add Texture Coordinate (UV)
                        else if (command == "vt")
                        {
                            var uv = new Vector2(Float(data[1]), Float(data[2]));
                            uvs.Add(uv);
                        }
                        // Add Face (Primitive)
                        else if (command == "f")
                        {
                            for (int j = 1; j < Math.Min(4, data.Length); j++)
                            {
                                var vertex = new VertexPositionTexture();
                                var component = data[j].Split('/');

                                if (component[0].Length > 0)
                                    vertex.Position = vertices[int.Parse(component[0]) - 1];

                                if (component.Length > 1)
                                {
                                    if (component[1].Length > 0)
                                        vertex.TextureCoordinate = uvs[int.Parse(component[1]) - 1];
                                }

                                verts.Add(vertex);
                            }
                        }
                    }
                }
            }

            if (mesh != null)
                mesh.VertexCount = verts.Count - mesh.VertexStart;

            // upload into single vertex buffer
            model._verts = verts.ToArray();

            return model;
        }

        private static float Float(string data)
        {
            return float.Parse(data, CultureInfo.InvariantCulture);
        }

        internal static IEnumerable<(Model3D model, string texture)> FromSolid(Sledge.Formats.Map.Objects.Solid solid)
        {
            Matrix transformationMatrix = Matrix.CreateScale(0.005f) *
                              Matrix.CreateRotationX(MathHelper.ToRadians(-90)) *
                              Matrix.CreateTranslation(new Vector3(0, 0, 0));
            var verts = new List<VertexPositionTexture>();

            foreach (var face in solid.Faces)
            {
                var model = new Model3D();
                verts.Clear();

                if (face.Vertices.Count < 3) continue;

                var uAxis = face.UAxis;
                var vAxis = face.VAxis;

                for (int i = 0; i < face.Vertices.Count - 2; i++)
                {
                    verts.AddRange(ProcessTriangle(face.Vertices[0], face.Vertices[i + 1], face.Vertices[i + 2], uAxis, vAxis, face, transformationMatrix));
                }

                model._verts = verts.ToArray();
                yield return (model, face.TextureName);
            }
        }


        private static IEnumerable<VertexPositionTexture> ProcessTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 uAxis, Vector3 vAxis, Face face, Matrix transformationMatrix)
        {
            yield return new VertexPositionTexture { Position = Vector3.Transform(v0, transformationMatrix), TextureCoordinate = CalculateUV(v0, uAxis, vAxis, face) };
            yield return new VertexPositionTexture { Position = Vector3.Transform(v1, transformationMatrix), TextureCoordinate = CalculateUV(v1, uAxis, vAxis, face) };
            yield return new VertexPositionTexture { Position = Vector3.Transform(v2, transformationMatrix), TextureCoordinate = CalculateUV(v2, uAxis, vAxis, face) };
        }

        private static Vector2 CalculateUV(Vector3 vertex, Vector3 uAxis, Vector3 vAxis, Face face)
        {
            // Project the 3D vertex position onto the 2D texture plane
            float u = Vector3.Dot(vertex, uAxis);
            float v = Vector3.Dot(vertex, vAxis);

            // Apply texture transformations
            // Note: This is a simplified version and might need adjustments based on your texture's properties
            u = (u + face.XShift) / face.XScale;
            v = (v + face.YShift) / face.YScale;

            // [TODO] Handle rotation if necessary
            // ...

            return new Vector2(u, v) / 64f;
        }
    }
}