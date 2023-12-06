using Bang.Components;
using HelloMurder3D.Assets;
using Murder.Utilities;

namespace HelloMurder3D.Components;

public readonly struct QMapComponent : IComponent
{
    public readonly AssetRef<ExternalMapAsset> Map = new();
    public QMapComponent()
    {
    }
}
