using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FontExamine.Services;
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
    /// <summary>
    /// Symbol definitions for Segoe UI Fluent and Segoe UI Symbol icons.  This class is used to define the symbol glyphs that are used in an application.  The definitions are used to export the defined glyph names and indices to a JSON file used by a T4 Template to generate constants for an easier and more readable definition of font glyphs in a UI layer.
    /// </summary>
    public partial class SymbolIconDefinitions : ObservableObject
    {
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private string _description;
        [ObservableProperty]
        private string _outputPath;
        [ObservableProperty]
        private float _fileVersion = 1.0f;

        [ObservableProperty]
        private DateTime _lastGenerated = DateTime.Today;
        [ObservableProperty]
        private string _symbolName = "Segoe";
        [ObservableProperty]
        private string _namespaceName = "FontExamine";
        [ObservableProperty]
        private bool _declareNamespace = false;
        [ObservableProperty]
        private GlyphUsedFor _usedFor = GlyphUsedFor.Segoe;
        [ObservableProperty]
        private ObservableCollection<GlyphDocument> _definedGlyphs;
        [ObservableProperty]
        private ObservableCollection<ExportFileDefn> _exportDefns;
        public SymbolIconDefinitions()
        {
            DefinedGlyphs = new ObservableCollection<GlyphDocument>();
            ExportDefns = new ObservableCollection<ExportFileDefn>();
            WeakReferenceMessenger.Default.Register<SymbolIconDefinitionsLoadedEventArgs>(this, HandleDefinitionsLoaded);
            //IncludeDefinitions = new ObservableCollection<string>();
        }
        /// <summary>
        /// A hook to post process the definitions after they are loaded. This is used to upgrade the file version and add a default export file if none exist.  However, it can be used for other post processing tasks as well.
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        private void HandleDefinitionsLoaded(object recipient, SymbolIconDefinitionsLoadedEventArgs message)
        {
            if (FileVersion == 1.0f && !string.IsNullOrEmpty(OutputPath) && ExportDefns.Count == 0)
            {
                ExportDefns.Add(new ExportFileDefn() { ProjectName = "ToBeNamed1", FilePath = OutputPath, GlyphUsedFor = UsedFor });
                // currently the output path is left as it was, but the file version is updated to 1.1 to indicate that the export files have been added.  However, the SymbolIconDefinitionsProject needs to be saved to disk to persist the new export file.  This is done in the SymbolIconDefinitionsProject.SaveProject method.
                FileVersion = 1.1f;
                message.Modified = true; // indicate that the project has been modified and needs to be saved to disk, however, the Modified property is not changed for SymbolIconDefinitions that are not changed.
            }
            //throw new NotImplementedException();
        }

        partial void OnNameChanged(string? oldValue, string newValue)
        {
            if (!string.IsNullOrEmpty(oldValue))
            {
                WeakReferenceMessenger.Default.Send<ProjectRenamedEventArgs>(new ProjectRenamedEventArgs() { NewName = newValue, OldName = oldValue });
            }
        }
        internal static SymbolIconDefinitions CreateDefinition(string name, string description, string outputPath, GlyphUsedFor usedFor, IEnumerable<GlyphDocument> definedGlyphs)
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
        internal void ExportActiveDefinitions()
        {
            foreach (var defn in ExportDefns.Where(d => d.Active && !string.IsNullOrEmpty(d.FilePath) && (d.GlyphUsedFor == GlyphUsedFor.Segoe || d.GlyphUsedFor == GlyphUsedFor.SegoeFluent)))
            {
                var fullPath = Path.Combine(defn.FilePath, defn.GetExportFilename());
                var exportDefn = new ExportSymbolDefinitions(Name, SymbolName, defn.ProjectName ?? "UndefinedNamespace", defn.GlyphUsedFor, defn.DevLayer)
                {
                    SelectedGlyphs = new List<GlyphDocument>(this.DefinedGlyphs)
                };
                var options = Singleton<CachedItems>.Instance.Dynamic?.AppJsonSerializerOptions as JsonSerializerOptions ?? Singleton<CachedItems>.Instance.DefaultJsonSerializerOptions;
                File.WriteAllText(fullPath, JsonSerializer.Serialize(exportDefn, options));
            }
            //var fullPath = Path.Combine(OutputPath, projectFilename);
            //var options = Singleton<CachedItems>.Instance.Dynamic?.AppJsonSerializerOptions as JsonSerializerOptions ?? Singleton<CachedItems>.Instance.DefaultJsonSerializerOptions;
            //File.WriteAllText(fullPath, JsonSerializer.Serialize(this, options));
        }
        internal void ExportDefinitions(string projectFilename)
        {
            var fullPath = Path.Combine(OutputPath, projectFilename);
            var options = Singleton<CachedItems>.Instance.Dynamic?.AppJsonSerializerOptions as JsonSerializerOptions ?? Singleton<CachedItems>.Instance.DefaultJsonSerializerOptions;
            File.WriteAllText(fullPath, JsonSerializer.Serialize(this, options));
        }
        [RelayCommand]
        [property: JsonIgnore]
        private void Delete(object p)
        {
            if (p is ExportFileDefn defn)
            {
                ExportDefns.Remove(defn);
            }
        }
    }
}
