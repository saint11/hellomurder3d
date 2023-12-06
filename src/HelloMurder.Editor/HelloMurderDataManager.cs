
using HelloMurder.Assets;
using HelloMurder3D.Assets;
using Murder;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor;
using Murder.Editor.Data;
using Murder.Serialization;

namespace HelloMurder.Editor;

internal class HelloMurderDataManager : EditorDataManager
{
    public string ModelsPath = "models";

    public HelloMurderDataManager(IMurderGame? game) : base(game)
    {
    }

    public override void LoadContent()
    {
        // Cleanup generated assets folder
        FileHelper.DeleteDirectoryIfExists(FileHelper.GetPath(Path.Join(Game.Profile.GenericAssetsPath, "GeneratedModels")));


        if (!Directory.Exists(FileHelper.GetPath(EditorSettings.GameSourcePath)))
        {
            GameLogger.Warning($"Please specify a valid \"Game Source Path\" in \"Editor Settings\". Unable to find the resources to build the atlas from.");
            return;
        }

        string sourcePackedPath = FileHelper.GetPath(EditorSettings.SourcePackedPath);
        if (!Directory.Exists(sourcePackedPath))
        {
            GameLogger.Warning($"Didn't find packed folder. Creating one.");
            FileHelper.GetOrCreateDirectory(sourcePackedPath);
        }

        string binPackedPath = FileHelper.GetPath(EditorSettings.BinResourcesPath);
        FileHelper.GetOrCreateDirectory(binPackedPath);

        string rawResourcesPath = FileHelper.GetPath(EditorSettings.RawResourcesPath, ModelsPath);

        LoadModels(rawResourcesPath, sourcePackedPath, binPackedPath);

        base.LoadContent();
    }

    private void LoadModels(string rawResourcesPath, string sourcePackedPath, string binPackedPath)
    {
        GameLogger.Verify(Path.IsPathRooted(rawResourcesPath) && Path.IsPathRooted(sourcePackedPath));
        var timeStart = DateTime.Now;

        // Make sure our target exists.
        string sourceDirectoryPath = Path.Join(sourcePackedPath, ModelsPath);
        _ = FileHelper.GetOrCreateDirectory(sourcePackedPath);

        // Make sure we also have the path at the binaries path.
        string binDirectoryPath = Path.Join(binPackedPath, ModelsPath);
        _ = FileHelper.GetOrCreateDirectory(binDirectoryPath);

        DirectoryInfo rawResourcesDirectory = FileHelper.GetOrCreateDirectory(rawResourcesPath);
        FileInfo[] files = rawResourcesDirectory.GetFiles("*.*", SearchOption.AllDirectories);

        foreach (FileInfo fi in files)
        {

            switch (fi.Extension.ToLower())
            {
                case ".map":
                    {
                        GameLogger.Log($"Found Quake .MAP format: {fi.FullName}");
                        var map = new ExternalMapAsset(fi);
                        map.Name = Path.GetFileNameWithoutExtension(fi.Name);
                        SaveAsset(map);
                    }
                    break;

                case ".bsp":
                    {
                        GameLogger.Log($"Found Quake .BSP format: {fi.FullName}");
                        var map = new ExternalMapAsset(fi);
                        map.Name = Path.GetFileNameWithoutExtension(fi.Name);
                        SaveAsset(map);
                    }
                    break;

                case ".obj":
                    {
                        GameLogger.Log($"Found model {fi.FullName}");
                        var model = new ModelAsset(fi);
                        model.Name = Path.GetFileNameWithoutExtension(fi.Name);
                        SaveAsset(model);
                    }
                    break;
                case ".png":
                    GameLogger.Log($"Found texture {fi.FullName}");
                    break;
            }
        }
    }

}
