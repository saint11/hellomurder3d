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

namespace HelloMurder.Systems;

[Filter(typeof(MeshComponent))]
internal class ObjRenderSystem : IMurderRenderSystem, IExitSystem
{
    private readonly BasicEffect _effect = new BasicEffect(Game.GraphicsDevice);
    private readonly RenderTarget2D _target = new RenderTarget2D(Game.GraphicsDevice, Game.Width,Game.Height);

    private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
    private Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
    private Matrix projection = Matrix.CreatePerspectiveFieldOfView(Calculator.TO_RAD * 45, 800f / 480f, 0.1f, 100f);

    public void Draw(RenderContext render, Context context)
    {
        Game.GraphicsDevice.SetRenderTarget(_target);
        Game.GraphicsDevice.Clear(Color.Transparent);

        Vector2 orbit = new Vector2(0, 1).Rotate(Game.Now);

        view = Matrix.CreateLookAt(new Vector3(orbit.Y, 0, orbit.X) * 6, new Vector3(0, 0, 0), Vector3.UnitY);

        _effect.World = world;
        _effect.View = view;
        _effect.Projection = projection;

        foreach (var e in context.Entities)
        {
            var mesh = e.GetMesh();
            var model = mesh.Model.Asset.Model;
            
            model.Draw(_effect);
        }

        render.UiBatch.Draw(_target, Vector2.Zero, _target.Bounds.Size.ToVector2(), _target.Bounds, 0, 0, Vector2.One, ImageFlip.None, Color.White, Vector2.Zero, RenderServices.BLEND_NORMAL);
    }

    public void Exit(Context context)
    {
        _target.Dispose();
    }
}
