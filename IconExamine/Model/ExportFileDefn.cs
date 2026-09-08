using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Text;
using System.Text.Json.Serialization;

namespace FontExamine.Model
{
    public partial class ExportFileDefn : ObservableObject
    {
        [ObservableProperty]
        [property: RegularExpression("^[a-zA-Z0-9_]+$")]
        private string? _projectName;
        [ObservableProperty]
        private string? _filePath;
        [ObservableProperty]
        private GlyphUsedFor _glyphUsedFor;
        [ObservableProperty]
        private DevelopementLayer _devLayer = DevelopementLayer.WPF;  // this property is used to determine which development layer the glyphs are intended for, which can affect how the file is processed in the T4 template, mainly, which using statements are emitted.
        [ObservableProperty]
        private bool _active = true;
        [ObservableProperty]
        private string? _fileDescription;
        partial void OnProjectNameChanged(string? oldValue, string? newValue)
        {
            try
            {
                Validator.ValidateProperty(newValue, new ValidationContext(this) { MemberName = nameof(ProjectName) });
            }
            catch (ValidationException ex)
            {
                // Handle validation error
                //var dlg =  System.Windows.MessageBox();
                if (newValue?.Contains(' ') ?? false)
                    System.Windows.MessageBox.Show("Project name cannot contain spaces.", "Validation Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                else
                    System.Windows.MessageBox.Show("Problems with Project Name, probably an invalid character" + ex.Message, "Validation Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        [RelayCommand]
        [property: JsonIgnore]
        private void BrowseExportFilePath()
        {
            // Implement file browsing logic here
            var dlg = new Microsoft.Win32.OpenFolderDialog();
            if (!string.IsNullOrEmpty(FilePath))
            {
                try
                {
                    dlg.InitialDirectory = System.IO.Path.GetDirectoryName(FilePath);
                }
                catch
                {
                    // ignore errors and use default initial directory
                }
            }
            if (dlg.ShowDialog() == true)
            {
                FilePath = dlg.FolderName;
            }
        }
        internal string GetExportFilename() => GlyphUsedFor switch
        {
            GlyphUsedFor.Segoe => "GlyphLink.json",
            GlyphUsedFor.SegoeFluent => "GlyphLinkFluent.json",
            GlyphUsedFor.FluentRegular => "Fluent.json",
            GlyphUsedFor.FluentFilled => "Fluent.json",
            _ => throw new NotImplementedException($"No export filename defined for GlyphUsedFor: {GlyphUsedFor}")
        };
    }
}
