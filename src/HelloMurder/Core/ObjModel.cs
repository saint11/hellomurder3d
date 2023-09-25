using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.Globalization;

namespace HelloMurder.Core
{
    public class ObjModel : IDisposable
    {
        public class Mesh
        {
            public string Name = "";
            public int VertexStart;
            public int VertexCount;
        }

        public List<Mesh> Meshes = new List<Mesh>();

        [JsonIgnore]
        public VertexBuffer Vertices { get; private set; }

        [JsonProperty]
        private VertexPositionTexture[] _verts;
        
        private bool ResetVertexBuffer()
        {
            if (Vertices == null || Vertices.IsDisposed || Vertices.GraphicsDevice.IsDisposed)
            {
                Vertices = new VertexBuffer(Murder.Game.GraphicsDevice, typeof(VertexPositionTexture), _verts.Length, BufferUsage.None);
                Vertices.SetData(_verts);
                return true;
            }

            return false;
        }

        public void ReassignVertices()
        {
            if (!ResetVertexBuffer())
                Vertices.SetData(_verts);
        }

        public void Draw(Effect effect)
        {
            ResetVertexBuffer();
            Murder.Game.GraphicsDevice.SetVertexBuffer(Vertices);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Murder.Game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, Vertices.VertexCount / 3);
            }
        }

        public void Dispose()
        {
            Vertices.Dispose();
            Meshes = null;
        }

        public static ObjModel Create(string filename)
        {
            var model = new ObjModel();
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
                        model.Meshes.Add(mesh);

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
                            model.Meshes.Add(mesh);
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
            model.ResetVertexBuffer();

            return model;
        }

        private static float Float(string data)
        {
            return float.Parse(data, CultureInfo.InvariantCulture);
        }
    }
}