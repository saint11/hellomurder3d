
using Bang.Components;
using Murder.Attributes;
using Murder.Utilities;
using System.Numerics;
using System.Text.Json.Serialization;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace HelloMurder3D.Components;

public readonly struct Camera3dComponent : IComponent
{
    public readonly float FieldOfView = 45f;

    [JsonIgnore, HideInEditor]
    public readonly Matrix Projection = Matrix.Identity;

    public Camera3dComponent(float FieldOfView)
    {
        Projection = Matrix.CreatePerspectiveFieldOfView(Calculator.TO_RAD * FieldOfView, 800f / 480f, 0.1f, 100f);
    }
}
