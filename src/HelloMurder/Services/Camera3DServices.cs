using System.Numerics;

namespace HelloMurder3D.Services;

public static class Camera3DServices
{
    public static Matrix4x4 GetRotationMatrix(Vector3 rotation)
    {
        return Matrix4x4.CreateRotationX(rotation.X) * Matrix4x4.CreateRotationY(rotation.Y);
    }
}
