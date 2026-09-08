using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace FontExamine.Model
{
    /// <summary>
    /// Base class for exporting glyph definitions. This class is used to define the common properties and methods for exporting glyph definitions to a JSON file. The derived classes will implement the specific export logic for different types of glyphs.
    /// </summary>
    public partial class ExportGlyphDefinitionsBase:ObservableObject
    {
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private string _symbolName = "Fluent";
        [ObservableProperty]
        private string _namespaceName = "FontExamine";
        [ObservableProperty]
        private bool _declareNamespace = false;
        [ObservableProperty]
        private GlyphUsedFor _usedFor;
        [ObservableProperty]
        private DevelopementLayer _envelopeLayer;
        public ExportGlyphDefinitionsBase(string name,string symbolName, string namespaceName, GlyphUsedFor usedFor, DevelopementLayer envelopeLayer)
        {
            Name = name;
            SymbolName = symbolName;
            NamespaceName = namespaceName;
            UsedFor = usedFor;
            EnvelopeLayer = envelopeLayer;
        }
        public ExportGlyphDefinitionsBase(SymbolIconDefinitions definitions,DevelopementLayer envelopeLayer)
        {
            Name = definitions.Name;
            SymbolName = definitions.SymbolName;
            NamespaceName = definitions.NamespaceName;
            UsedFor = definitions.UsedFor;
            EnvelopeLayer = envelopeLayer;
        }
    }
}
