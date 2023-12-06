using Bang.Components;
using HelloMurder.Assets;
using HelloMurder.Core;
using Microsoft.Xna.Framework.Graphics;
using Murder.Utilities;
using Sledge.Formats.Map.Objects;

namespace HelloMurder.Components;

public readonly struct MeshComponent : IComponent
{
    public readonly Model3D Model = new();

    public readonly Texture2D? Texture = null;
    public MeshComponent()
    {
    }

    public MeshComponent(Model3D model, Texture2D texture)
    {
        Model = model;
        Texture = texture;
    }
}
