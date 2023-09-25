using HelloMurder.Core;
using Microsoft.Xna.Framework.Graphics;
using Murder;
using Murder.Assets;
using Murder.Serialization;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace HelloMurder.Assets;

public class ModelAsset : GameAsset
{
    public override string EditorFolder => "Models";
    
    public ObjModel Model;

    public ModelAsset(string path)
    {
        Model = ObjModel.Create(path);
        
        using var md5 = MD5.Create();
        Guid = new Guid(md5.ComputeHash(Encoding.Default.GetBytes(path)));
    }

    public ModelAsset()
    {
    }
}
