using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using HelloMurder.Components;
using HelloMurder.Core;
using HelloMurder3D.Components;
using HelloMurder3D.Services;
using Microsoft.Xna.Framework.Input;
using Murder.Components;
using Murder.Core.Input;
using Murder.Utilities;
using System.Numerics;
using Game = Murder.Game;

namespace HelloMurder3D.Systems;


[Filter(typeof(PlayerComponent), typeof(Transform3D))]
internal class FPSCameraSystem : IFixedUpdateSystem
{

    private MouseState _previousMouseState;
    private Vector3 _mouseRotationBuffer;
    public void FixedUpdate(Context context)
    {
        float deltaX = 0f;
        float deltaY = 0f;
        float verticalMovement = 0;
        Vector2 groundMovement = Vector2.Zero;


        if (Game.Instance.IsActive)
        {
            // Get mouse input
            MouseState ms = Mouse.GetState();

            if (ms != _previousMouseState)
            {
                deltaX = ms.X - (Game.GraphicsDevice.Viewport.Width / 2);
                deltaY = ms.Y - (Game.GraphicsDevice.Viewport.Height / 2);

                float mouseSensitivity = 0.25f;
                _mouseRotationBuffer.X -= mouseSensitivity * deltaX * (float)Game.DeltaTime * 1.4f;
                _mouseRotationBuffer.Y -= mouseSensitivity * deltaY * (float)Game.DeltaTime;

                if (_mouseRotationBuffer.Y < -75.0f * Calculator.TO_RAD)
                {
                    _mouseRotationBuffer.Y = _mouseRotationBuffer.Y - (_mouseRotationBuffer.Y - -75.0f * Calculator.TO_RAD);
                }

                if (_mouseRotationBuffer.Y > 75.0f * Calculator.TO_RAD)
                {
                    _mouseRotationBuffer.Y = _mouseRotationBuffer.Y - (_mouseRotationBuffer.Y - 75.0f * Calculator.TO_RAD);
                }
            }

            groundMovement = Game.Input.GetOrCreateAxis(MurderInputAxis.Movement).Value;
            if (Game.Input.GetOrCreateButton(InputButtons.Jump).Down)
            {
                verticalMovement -= 1;
            }

            if (Game.Input.GetOrCreateButton(InputButtons.Crouch).Down)
            {
                verticalMovement += 1;
            }

            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            Game.Instance.IsMouseVisible = false;
        }
        else
        {
            Game.Instance.IsMouseVisible = true;

            deltaX = 0f;
            deltaY = 0f;
            verticalMovement = 0f;
            groundMovement = Vector2.Zero;
        }

        foreach (var entity in context.Entities)
        {
            if (entity.TryGetTransform3D() is not Transform3D currentTransform)
                return;

            Vector3 rotation = currentTransform.Rotation;

            if (deltaX != 0 && deltaY != 0)
            {
                rotation = new Vector3(
                -Math.Clamp(_mouseRotationBuffer.Y, -75.0f * Calculator.TO_RAD, 75.0f * Calculator.TO_RAD),
                Microsoft.Xna.Framework.MathHelper.WrapAngle(_mouseRotationBuffer.X),
                0);
            }

            Matrix4x4 rotationMatrix = Camera3DServices.GetRotationMatrix(rotation);

            Vector3 position = currentTransform.Position - Vector3.Transform(new Vector3(groundMovement.X, verticalMovement, groundMovement.Y), rotationMatrix) * Game.FixedDeltaTime * 2f;
            entity.RemoveAgentImpulse();

            entity.SetTransform3D(
                position,
                rotation,
                currentTransform.Scale
                );

        }
    }

}