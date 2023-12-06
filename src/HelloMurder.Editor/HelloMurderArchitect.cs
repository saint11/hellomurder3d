using HelloMurder3D.Assets;
using Murder.Editor;

namespace HelloMurder.Editor
{
    public class HelloMurderArchitect : HelloMurderGame, IMurderArchitect
    {
        public GameProfile3D CreateGameProfile => new GameProfile3D();
    }
}