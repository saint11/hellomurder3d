using Murder.Editor;

namespace HelloMurder.Editor
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var game = new HelloMurderArchitect();
            var dataManager = new HelloMurderDataManager(game);

            using (var editor = new Architect(game, dataManager))
            {
                editor.Run();
            }
        }
    }
}
