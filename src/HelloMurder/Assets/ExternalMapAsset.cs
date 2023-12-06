using Murder.Assets;
using Murder.Attributes;
using Murder.Diagnostics;
using Sledge.Formats.Map.Formats;
using Sledge.Formats.Map.Objects;
using System.Security.Cryptography;
using System.Text;

namespace HelloMurder3D.Assets;

public class ExternalMapAsset : GameAsset
{
    public override string EditorFolder => "QMaps";

    public string FilePath;

    public ExternalMapAsset() { }

    public ExternalMapAsset(FileInfo fileInfo)
    {
        FilePath = fileInfo.FullName;

        using var md5 = MD5.Create();
        Guid = new Guid(md5.ComputeHash(Encoding.Default.GetBytes(fileInfo.FullName)));
    }
}
