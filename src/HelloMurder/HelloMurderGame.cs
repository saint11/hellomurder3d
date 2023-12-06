using HelloMurder.Core;
using HelloMurder3D.Assets;
using Microsoft.Xna.Framework.Input;
using Murder;
using Murder.Assets;
using Murder.Core.Input;

namespace HelloMurder
{
    public class HelloMurderGame : IMurderGame
    {
        public string Name => "HelloMurder";

        public static GameProfile3D Profile => (GameProfile3D)Game.Profile;

        public GameProfile CreateGameProfile() => new GameProfile3D();

        public void Initialize()
        {
            Game.Input.Register(MurderInputAxis.Movement,
                new InputButtonAxis(Keys.W, Keys.A, Keys.S, Keys.D),
                new InputButtonAxis(Keys.Up, Keys.Left, Keys.Down, Keys.Right));

            Game.Input.Register(InputButtons.Jump, Keys.Space, Keys.Q);
            Game.Input.Register(InputButtons.Crouch, Keys.LeftControl, Keys.C);

        }
    }
}
