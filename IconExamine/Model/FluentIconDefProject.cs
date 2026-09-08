using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json.Serialization;

namespace FontExamine.Model;
public partial class FluentIconDefProject : ObservableObject
{
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private string _description;
    [ObservableProperty]
    private string? _lastCommonNameSelected;
    [ObservableProperty]
    private float _fileVersion = 1.0f;
    [ObservableProperty]
    private string _symbolName = "Fluent";
    [ObservableProperty]
    private string _namespaceName = "Filament";
    [ObservableProperty]
    private bool _declareNamespace = false;
    [ObservableProperty]
    private GlyphUsedFor _usedFor= GlyphUsedFor.FluentRegular;
    [ObservableProperty]
    private ObservableCollection<string> selectedIcons;
    [ObservableProperty]
    private ObservableCollection<string> includeProjects;
    [ObservableProperty]
    private ObservableCollection<ExportFileDefn> exportDefns;
    [ObservableProperty]
    private string _exportPath;

    public FluentIconDefProject()
    {
        WeakReferenceMessenger.Default.Register<ProjectRenamedEventArgs>(this, HandleProjectRenamed);
        WeakReferenceMessenger.Default.Register<ProjectDeletedEventArgs>(this, HandleProjectDeleted);
        WeakReferenceMessenger.Default.Register<FluentIconDefinitionLoadedEventArgs>(this, HandleDefinitionsLoaded);
        ExportDefns = new ObservableCollection<ExportFileDefn>();
        SelectedIcons = new ObservableCollection<string>();
        IncludeProjects = new ObservableCollection<string>();
    }

    private void HandleDefinitionsLoaded(object recipient, FluentIconDefinitionLoadedEventArgs message)
    {
        if(FileVersion == 1.0f && ExportDefns.Count == 0 && !string.IsNullOrEmpty(ExportPath))
        {
            ExportDefns.Add(new ExportFileDefn() { FilePath= ExportPath,GlyphUsedFor = UsedFor,ProjectName="ToBeNamed1" });
            FileVersion=1.1f;
            message.Modified = true;
        }
    }

    private void HandleProjectDeleted(object recipient, ProjectDeletedEventArgs message)
    {
        if (message != null && (IncludeProjects?.Contains(message.ProjectName)??false))
        {
            IncludeProjects.Remove(message.ProjectName);
        }
    }

    private void HandleProjectRenamed(object recipient, ProjectRenamedEventArgs message)
    {
        if (IncludeProjects?.Contains(message.OldName) ?? false)
        {
            IncludeProjects.Remove(message.OldName);
            IncludeProjects.Add(message.NewName);
        }
    }
    partial void OnNameChanged(string? oldValue, string newValue)
    {
        // event ignored when the project is first being named
        if (!string.IsNullOrEmpty(oldValue))
        {
            WeakReferenceMessenger.Default.Send<ProjectRenamedEventArgs>(new ProjectRenamedEventArgs() { NewName = newValue, OldName = oldValue });
        }
    }
    [RelayCommand]
    [property:JsonIgnore]
    private void BrowseExportPath()
    {
        var dialog = new Microsoft.Win32.OpenFolderDialog();
        dialog.Title = "Select Export Path";
        if (dialog.ShowDialog() == true)
        {
            ExportPath = dialog.FolderName;
        }
    }
    [RelayCommand]
    [property: JsonIgnore]
    private void Delete(object p)
    {
        if(p is ExportFileDefn defn)
        {
            ExportDefns.Remove(defn);
        }
    }
}
