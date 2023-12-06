using HelloMurder.Core;
using Microsoft.Xna.Framework.Graphics;
using Murder;
using Murder.Assets;
using Murder.Diagnostics;
using Murder.Serialization;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace HelloMurder.Assets;

public class ModelAsset : GameAsset
{
    public override string EditorFolder => "Models";
    public Core.Model3D Model;

    public ModelAsset(FileInfo fileInfo)
    {
        if (fileInfo.Extension.ToLower() ==".obj")
        {
            Model = Core.Model3D.CreateFromObj(fileInfo.FullName);
        }
        else
        {
            GameLogger.Error($"Invalid extension for model {fileInfo.FullName}");
        }

        using var md5 = MD5.Create();
        Guid = new Guid(md5.ComputeHash(Encoding.Default.GetBytes(fileInfo.FullName)));
    }

    public ModelAsset()
    {
    }
}
