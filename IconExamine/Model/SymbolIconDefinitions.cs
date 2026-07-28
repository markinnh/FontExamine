using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FontExamine.Model
{
    public partial class SymbolIconDefinitions : ObservableObject
    {
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private string _description;
        [ObservableProperty]
        private string _outputPath;
        [ObservableProperty]
        private DateTime _lastGenerated=DateTime.Today;
        [ObservableProperty]
        private string _symbolName = "Segoe";
        [ObservableProperty]
        private string _namespaceName = "FontExamine";
        [ObservableProperty]
        private bool _declareNamespace = false;
        [ObservableProperty]
        private GlyphUsedFor _usedFor = GlyphUsedFor.Segoe;  // this is only used when exporting FluentIconDefProject, which changes the default value to FluentRegularIcon
        [ObservableProperty]
        private ObservableCollection<GlyphDocument> _definedGlyphs;

        public SymbolIconDefinitions()
        {
            DefinedGlyphs = new ObservableCollection<GlyphDocument>();
            //IncludeDefinitions = new ObservableCollection<string>();
        }
        partial void OnNameChanged(string? oldValue, string newValue)
        {
            if (!string.IsNullOrEmpty(oldValue))
            {
                WeakReferenceMessenger.Default.Send<ProjectRenamedEventArgs>(new ProjectRenamedEventArgs() { NewName = newValue, OldName = oldValue });
            }
        }
        internal static SymbolIconDefinitions CreateDefinition(string name,string description,string outputPath,GlyphUsedFor usedFor,IEnumerable<GlyphDocument> definedGlyphs)
        {
            var newDefinition = new SymbolIconDefinitions()
            {
                Name = name,
                Description = description,
                OutputPath = outputPath,
                UsedFor = usedFor,
                SymbolName = name,
                DefinedGlyphs = new ObservableCollection<GlyphDocument>(definedGlyphs)
            };
            return newDefinition;
        }
        internal void ExportDefinitions(string projectFilename)
        {
            var fullPath = Path.Combine(OutputPath, projectFilename);
            var options = new JsonSerializerOptions() { WriteIndented = true ,
                Converters = { new JsonStringEnumConverter() } };
            File.WriteAllText(fullPath, JsonSerializer.Serialize(this, options));
        }
    }
}
