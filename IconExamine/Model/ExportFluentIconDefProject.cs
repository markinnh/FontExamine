using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;
using FontExamine.Services;

namespace FontExamine.Model
{
    /// <summary>
    /// Represents the export definition for Fluent icons.
    /// </summary>
    public partial class ExportFluentIconDefProject:ExportGlyphDefinitionsBase
    {
        
        internal ExportFluentIconDefProject(string name, string symbolName, string namespaceName, GlyphUsedFor usedFor, DevelopementLayer envelopeLayer)
            : base(name, symbolName, namespaceName, usedFor, envelopeLayer)
        {
        }
        [ObservableProperty]
        private List<GlyphDocument> regularGlyphs = new List<GlyphDocument>();
        [ObservableProperty]
        private List<GlyphDocument> filledGlyphs = new List<GlyphDocument>();
        internal static ExportFluentIconDefProject CreateDefinition(string name,string symbolName, string namespaceName, GlyphUsedFor usedFor,DevelopementLayer envelopeLayer, IEnumerable<GlyphDocument> selectedRegularGlyphs, IEnumerable<GlyphDocument> selectedFilledGlyphs)
        {
            var exportDefinition = new ExportFluentIconDefProject(name, symbolName, namespaceName, usedFor, envelopeLayer);
            
            exportDefinition.RegularGlyphs.AddRange(selectedRegularGlyphs);
            exportDefinition.FilledGlyphs.AddRange(selectedFilledGlyphs);
            return exportDefinition;
        }
        internal void ExportToJson(string exportPath)
        {
            var jsonOptions = Singleton<CachedItems>.Instance.Dynamic?.AppJsonSerializerOptions ?? Singleton<CachedItems>.Instance.DefaultJsonSerializerOptions;
            var jsonString = JsonSerializer.Serialize(this, jsonOptions);
            File.WriteAllText(exportPath, jsonString);
        }
    }
}
