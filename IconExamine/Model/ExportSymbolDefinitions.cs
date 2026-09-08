using CommunityToolkit.Mvvm.ComponentModel;
using FontExamine.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace FontExamine.Model
{
    public partial class ExportSymbolDefinitions:ExportGlyphDefinitionsBase
    {
        [ObservableProperty]
        private List<GlyphDocument> _selectedGlyphs = new List<GlyphDocument>();
        internal ExportSymbolDefinitions(string name, string symbolName, string namespaceName, GlyphUsedFor usedFor, DevelopementLayer envelopeLayer)
            : base(name, symbolName, namespaceName, usedFor, envelopeLayer)
        {
        }
        internal static ExportSymbolDefinitions CreateDefinition(string name, string symbolName, string namespaceName, GlyphUsedFor usedFor, DevelopementLayer envelopeLayer, IEnumerable<GlyphDocument> selectedGlyphs)
        {
            var exportDefinition = new ExportSymbolDefinitions(name, symbolName, namespaceName, usedFor, envelopeLayer);
            exportDefinition.SelectedGlyphs.AddRange(selectedGlyphs);
            return exportDefinition;
        }
        //internal void ExportToJson(string exportPath)
        //{
        //    var jsonOptions = Singleton<CachedItems>.Instance.Dynamic?.AppJsonSerializerOptions ?? Singleton<CachedItems>.Instance.DefaultJsonSerializerOptions;
        //    var jsonString = System.Text.Json.JsonSerializer.Serialize(this, jsonOptions);
        //    System.IO.File.WriteAllText(exportPath, jsonString);
        //}
    }
}
