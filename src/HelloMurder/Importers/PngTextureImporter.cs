using Murder.Editor.Assets;
using Murder.Editor.Importers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloMurder3D.Importers
{
    [ImporterSettings(FilterType.OnlyTheseFolders, new[] { RelativeDirectory }, new[] { ".png" })]
    internal class PngTextureImporter : PngNoAtlasImporter
    {
        private const string RelativeDirectory = "textures";
        public override string RelativeSourcePath => RelativeDirectory;
        public PngTextureImporter(EditorSettingsAsset editorSettings) : base(editorSettings)
        {
        }
    }
}
