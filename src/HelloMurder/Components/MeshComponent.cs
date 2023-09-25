using Bang.Components;
using HelloMurder.Assets;
using Murder.Utilities;

namespace HelloMurder.Components;

public readonly struct MeshComponent : IComponent
{
    public readonly AssetRef<ModelAsset> Model = new();

    public MeshComponent()
    {
    }
}
