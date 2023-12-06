
using Bang.Entities;
using Bang.Systems;
using HelloMurder3D.Components;
using Murder.Diagnostics;
using System.Collections.Immutable;
using Sledge.Formats.Map.Objects;
using Entity = Bang.Entities.Entity;
using MapEntity = Sledge.Formats.Map.Objects.Entity;
using Bang;
using Sledge.Formats.Map.Formats;
using HelloMurder.Components;
using HelloMurder.Core;
using Murder;
using Sledge.Formats.Bsp;

namespace HelloMurder3D.Systems;

[Watch(typeof(QMapComponent))]
public class QMapSpawnerSystem : IReactiveSystem
{
    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        var quake = new QuakeMapFormat();

        foreach (var e in entities)
        {
            var mapComponent = e.GetQMap();

            if (mapComponent.Map.TryAsset?.FilePath is not string path)
                continue;

            FileInfo fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
                continue;

            switch (fileInfo.Extension.ToLower())
            {
                case ".bsp": // Compiled map
                    var bsp = new BspFile(fileInfo.OpenRead());
                    GameLogger.Warning("BSP format is not supported yet");
                    break;

                case ".map": // Quake format
                    MapFile map = quake.ReadFromFile(path);
                    LoadMeshes(world, map.Worldspawn);
                    break;
                default:
                    break;
            }


        }
    }

    private void LoadMeshes(World world, MapObject mapObject)
    {
        switch (mapObject)
        {
            case MapEntity entity:
                GameLogger.Log($"Loading entity {entity.ClassName}");
                break;
            case Solid solid:
                {
                    GameLogger.Log($"Loading solid");
                    foreach ((Model3D m, string texture)in Model3D.FromSolid(solid))
                    {
                        var e = world.AddEntity();
                        e.SetMesh(m, Game.Data.TryFetchTexture(System.IO.Path.Join("images", texture.TrimStart('_'))));
                    }
                }
                break;
            default:
                GameLogger.Log($"Loading generic object");
                break;
        }
        foreach (var child in mapObject.Children)
        {
            LoadMeshes(world, child);
        }
    }

    public void OnModified(World world, ImmutableArray<Entity> entities)
    {
        
    }

    public void OnRemoved(World world, ImmutableArray<Entity> entities)
    {
        
    }
}
