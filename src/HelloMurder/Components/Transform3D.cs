using Bang.Components;
using Bang.Entities;
using Murder.Attributes;
using Murder.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HelloMurder3D.Components;

public readonly struct Transform3D  :  IComponent, IEquatable<Transform3D>
{
    /// <summary>
    /// Cached transformation matrix
    /// </summary>
    [JsonIgnore, HideInEditor]
    public readonly Matrix4x4? Matrix;

    /// <summary>
    /// Position in units
    /// </summary>
    public readonly Vector3 Position;

    /// <summary>
    /// Euler rotation in degrees
    /// </summary>
    public readonly Vector3 Rotation;

    /// <summary>
    /// Scale, where 1 is the 100% scale
    /// </summary>
    public readonly Vector3 Scale;

    public Transform3D(Vector3 position, Vector3 rotation, Vector3 scale) : this()
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
        Matrix =
            Matrix4x4.CreateRotationX(rotation.X) * Matrix4x4.CreateRotationY(rotation.Y) * Matrix4x4.CreateRotationZ(rotation.Z)
            * Matrix4x4.CreateScale(scale)
            * Matrix4x4.CreateTranslation(position);

    }

    public bool Equals(Transform3D other)
    {
        return Matrix == other.Matrix;
    }
}
