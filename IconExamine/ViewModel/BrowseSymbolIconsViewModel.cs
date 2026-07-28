using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FontExamine.Model;
using FontExamine.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Automation;

namespace FontExamine.ViewModel;

public partial class BrowseSymbolIconsViewModel : ObservableObject
{
    const string Filename = "SymbolDefinitionsProject.json";
    [ObservableProperty]
    private ObservableCollection<LightGlyphDefn> _glyphIds;
    [ObservableProperty]
    private LightGlyphDefn _activeGlyph;
    [ObservableProperty]
    private GlyphDocument _editDocument;
    [ObservableProperty]
    private int _initialEditDocumentHash;
    [ObservableProperty]
    private SymbolIconDefinitionsProject _symbolIconDefinitionsProject;
    [ObservableProperty]
    private SymbolIconDefinitions _activeDefinition;
    public BrowseSymbolIconsViewModel()
    {
        var projectFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Settings1.Default.DataDir, Filename);

        GlyphIds = new ObservableCollection<LightGlyphDefn>(from i in Singleton<SupportedTables>.Instance.SegoeUISymbol select new LightGlyphDefn() { GlyphId = i, UnicodeChar = char.ConvertFromUtf32(i) });
        SymbolIconDefinitionsProject = SymbolIconDefinitionsProject.LoadDefinitions(projectFile);
        if (!string.IsNullOrEmpty(SymbolIconDefinitionsProject.ActiveDefinitionName) && SymbolIconDefinitionsProject.Definitions.FirstOrDefault(dfn => dfn.Name == SymbolIconDefinitionsProject.ActiveDefinitionName) is SymbolIconDefinitions dfn)
            ActiveDefinition = dfn;
        
    }


    partial void OnActiveDefinitionChanged(SymbolIconDefinitions? oldValue, SymbolIconDefinitions newValue)
    {
        SymbolIconDefinitionsProject.ActiveDefinitionName = newValue.Name;
    }
    partial void OnActiveGlyphChanged(LightGlyphDefn value)
    {
        // TODO: add code to signify the document has changed and prompt to save to current project
        if (EditDocument != null && EditDocument.GetHashCode() != InitialEditDocumentHash && !string.IsNullOrEmpty(EditDocument.GlyphName))
        {
            if (ActiveDefinition == null)
            {
                ActiveDefinition = new SymbolIconDefinitions() { Name = $"New Project", Description = "A new project for the collection" };
                SymbolIconDefinitionsProject.Definitions.Add(ActiveDefinition);
                ActiveDefinition.DefinedGlyphs.Add(EditDocument);
                value.HasDocument = true;
            }
            else if (ActiveDefinition != null && !ActiveDefinition.DefinedGlyphs.Contains(EditDocument))
            {
                ActiveDefinition.DefinedGlyphs.Add(EditDocument);
                value.HasDocument = true;
            }
        }
        else if (ActiveDefinition?.DefinedGlyphs.FirstOrDefault(d => d.GlyphId == value.GlyphId) is GlyphDocument document)
            EditDocument = document;
        else
            EditDocument = value;



        InitialEditDocumentHash = EditDocument.GetHashCode();
    }
    [RelayCommand]
    private void AddGlyphToProject()
    {
        if (EditDocument == null)
            return;

        if (ActiveDefinition == null)
        {
            ActiveDefinition = new SymbolIconDefinitions() { Name = $"New Project {SymbolIconDefinitionsProject.Definitions.Count + 1}", Description = "A new project for the collection" };
            SymbolIconDefinitionsProject.Definitions.Add(ActiveDefinition);
            SymbolIconDefinitionsProject.ActiveDefinitionName = ActiveDefinition.Name;
        }

        if (!ActiveDefinition.DefinedGlyphs.Contains(EditDocument))
        {
            ActiveDefinition.DefinedGlyphs.Add(EditDocument);
            InitialEditDocumentHash = EditDocument.GetHashCode();
        }
    }
    [RelayCommand]
    private void GenerateXaml(object p)
    {
        if (ActiveGlyph != null && p is string str)
        {
            Clipboard.SetText(ActiveGlyph.GenerateXaml(str));
        }
    }
    [RelayCommand]
    private void GenerateStaticClass()
    {
        if (ActiveDefinition != null)
        {
            var xaml = SymbolIconDefinitionsProject.GenerateStaticClass(ActiveDefinition.Name);
            ActiveDefinition.LastGenerated = DateTime.Now;
            if (!string.IsNullOrEmpty(xaml)) Clipboard.SetText(xaml);
        }
    }
    [RelayCommand]
    private void SaveSymbolDefinitionsProject()
    {
        var outfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Settings1.Default.DataDir, Filename);
        SymbolIconDefinitionsProject.SaveDefinitions(outfile);
        // output the project to a file in the Projects directory, currently hardcoded to my local machine, but this should be changed to a relative path or user-defined path stored in settings. For now, this is just for testing purposes.
        //if (ActiveDefinition != null && !string.IsNullOrEmpty(ActiveDefinition.OutputPath))
        //{
        //    var projectDir = Path.Combine(ActiveDefinition.OutputPath, "GlueGlyph.json");
        //    ActiveDefinition.ExportDefinitions(projectDir);
        //}
    }
    [RelayCommand]
    private void ExportSymbolDefinitionsProject()
    {
        if (ActiveDefinition != null && !string.IsNullOrEmpty(ActiveDefinition.OutputPath))
        {
            var projectDir = Path.Combine(ActiveDefinition.OutputPath, "GlueGlyph.json");
            ActiveDefinition.ExportDefinitions(projectDir);
        }
    }
    [RelayCommand]
    private void AddProject()
    {
        var newProject = new SymbolIconDefinitions() { Name = $"New Project {SymbolIconDefinitionsProject.Definitions.Count + 1}", Description = "A new project for the collection" };
        SymbolIconDefinitionsProject.Definitions.Add(newProject);
        SymbolIconDefinitionsProject.ActiveDefinitionName = newProject.Name;
    }
    [RelayCommand]
    private void BrowseExportPath()
    {
        if (ActiveDefinition != null)
        {
            var dlg = new Microsoft.Win32.OpenFolderDialog();
            if (dlg.ShowDialog() == true)
            {
                ActiveDefinition.OutputPath = dlg.FolderName;
            }
        }
    }
}