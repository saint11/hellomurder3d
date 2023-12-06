using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using HelloMurder.Components;
using Microsoft.Xna.Framework.Graphics;
using Murder;
using Murder.Core.Graphics;
using System.Numerics;

using Vector3 = Microsoft.Xna.Framework.Vector3;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Murder.Utilities;
using Murder.Services;
using HelloMurder3D.Components;
using HelloMurder3D.Assets;
using HelloMurder.Core;
using HelloMurder3D.Services;

namespace HelloMurder.Systems;

[Filter(typeof(MeshComponent))]
internal class ObjRenderSystem : IMurderRenderSystem, IExitSystem, IStartupSystem
{
    private readonly BasicEffect _effect = new BasicEffect(Game.GraphicsDevice);
    private readonly RenderTarget2D _target = new RenderTarget2D(
        Game.GraphicsDevice,
        Game.Width, Game.Height,
        false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.DiscardContents);

    private Matrix _world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
    private Model3D _gizmo;

    public void Draw(RenderContext render, Context context)
    {
        if (context.World.TryGetUniqueEntity<Camera3dComponent>() is not Entity cameraEntiy)
            return;

        Camera3dComponent camera3D = cameraEntiy.GetCamera3d();
        Transform3D cameraTransform = cameraEntiy.GetTransform3D();

        Game.GraphicsDevice.SetRenderTarget(_target);
        Game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Gray, 1f, 0);
        Game.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

        _effect.World = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        _effect.Projection = Matrix.CreatePerspectiveFieldOfView(Calculator.TO_RAD * 65, 800f / 480f, 0.1f, 100f);

        _effect.View = Matrix.CreateLookAt(cameraTransform.Position, cameraTransform.Position + Vector3.Transform(Vector3.UnitZ, Camera3DServices.GetRotationMatrix(cameraTransform.Rotation)), Vector3.Up);

        _effect.EnableDefaultLighting();
        _effect.FogEnabled = true;
        _effect.FogStart = 1.5f;
        _effect.FogEnd = 162f;

        var device = Murder.Game.GraphicsDevice;

        RasterizerState state = new RasterizerState();
        state.CullMode = CullMode.CullClockwiseFace;
        state.FillMode = FillMode.Solid;

        device.DepthStencilState = new DepthStencilState()
        {
            DepthBufferEnable = true,
            DepthBufferWriteEnable = true,
        };
        device.BlendState = BlendState.Opaque;
        device.RasterizerState = state;

        int entityCount = context.Entities.Count();
        foreach (var e in context.Entities)
        {
            var mesh = e.GetMesh();
            if (mesh.Texture is Texture2D texture)
            {
                _effect.Texture = texture;
                _effect.TextureEnabled = true;
            }
            else
            {
                _effect.TextureEnabled = false;
            }

            if (mesh.Model is Core.Model3D model)
            {
                model.Draw(_effect);
            }
        }

        //_gizmo.Draw(_effect);

        render.UiBatch.Draw(_target, Vector2.Zero, _target.Bounds.Size.ToVector2(), _target.Bounds, 0, 0, Vector2.One, ImageFlip.None, Color.White, Vector2.Zero, RenderServices.BLEND_NORMAL);
    }

    public void Exit(Context context)
    {
        _target.Dispose();
    }

    public void Start(Context context)
    {
        _gizmo = ((GameProfile3D)Game.Profile).Gizmo.Asset.Model;
    }
}
