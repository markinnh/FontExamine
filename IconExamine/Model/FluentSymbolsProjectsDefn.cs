using CommunityToolkit.Mvvm.ComponentModel;
using FontExamine.Services;
using IconExamine.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FontExamine.Model
{
    public partial class FLuentSymbolsProjectsDefn : ObservableObject
    {
        [ObservableProperty]
        private string _lastSelectedProjectName = "Default Project";

        [ObservableProperty]
        private ObservableCollection<FluentIconDefProject> _currentProjects;

        [ObservableProperty]
        private ObservableCollection<string> _includeProjects;
        public FLuentSymbolsProjectsDefn()
        {
            CurrentProjects = new ObservableCollection<FluentIconDefProject>();
            IncludeProjects = new ObservableCollection<string>();

        }
        internal void AddDefaultProject()
        {
            if (CurrentProjects.Count == 0)
                CurrentProjects.Add(new FluentIconDefProject() { Name = "Default Project", Description = "Just a test project to see how the app works", SelectedIcons = new(new string[] { "add_20", "airplane_20" }) });
        }
        internal string GenerateStaticClass()
        {
            var currentProject = CurrentProjects.FirstOrDefault(p => p.Name == LastSelectedProjectName);
            var enumNames = GetCombinedEnumNames();
            var enumContent = CodeGeneration.GenerateEnums($"{currentProject?.SymbolName ?? "Undefined"}Symbols", enumNames);
            // Static Dictionary Generation
            var regularGlyphs = GetDistinctSelections(Singleton<GlyphLists>.Instance.RegularIconDefn);
            var filledGlyphs = GetDistinctSelections(Singleton<GlyphLists>.Instance.FilledIconDefn);
            var dictionaryContent = CodeGeneration.GenerateDictionaryDefinition("Filled", $"{currentProject?.SymbolName ?? "Undefined"}Symbols", filledGlyphs.Select(gly => gly.DictionaryEntry(currentProject?.SymbolName ?? "Undefined")));
            dictionaryContent += CodeGeneration.GenerateDictionaryDefinition("Regular", $"{currentProject?.SymbolName ?? "Undefined"}Symbols", regularGlyphs.Select(gly => gly.DictionaryEntry(currentProject?.SymbolName ?? "Undefined")));
            return CodeGeneration.GenerateStaticClass(currentProject?.SymbolName ?? "Undefined", enumContent, dictionaryContent, currentProject?.NamespaceName ?? "Undefined", currentProject?.DeclareNamespace ?? false);
        }
        internal void ExportCurrentProject()
        {
            if (CurrentProjects.FirstOrDefault(p => p.Name == LastSelectedProjectName) is FluentIconDefProject currentProject)
            {
                if(string.IsNullOrEmpty(currentProject.ExportPath))
                {
                    var dlg =new  OpenFileDialog();
                    if (dlg.ShowDialog() == true)
                    {
                        currentProject.ExportPath = dlg.FileName;
                    }
                    else
                        return;
                }
                var selectedGlyphs = GetDistinctSelections(currentProject.UsedFor).Select(gl => new GlyphDocument() { GlyphName = gl.EnumName, GlyphId = gl.UnicodeInt });
                var definition= SymbolIconDefinitions.CreateDefinition(currentProject.SymbolName, currentProject.NamespaceName, currentProject.ExportPath, currentProject.UsedFor, selectedGlyphs);
                definition.ExportDefinitions("GlueGlyphFluent.json");
            }
        }
        private IEnumerable<FluentGlyphDefn> GetDistinctSelections(GlyphUsedFor usedFor) => usedFor switch
        {
            GlyphUsedFor.FluentRegular => GetDistinctSelections(Singleton<GlyphLists>.Instance.RegularIconDefn),
            GlyphUsedFor.FluentFilled => GetDistinctSelections(Singleton<GlyphLists>.Instance.FilledIconDefn),
            _ => throw new NotImplementedException()
        };
        private List<FluentGlyphDefn> GetDistinctSelections(IEnumerable<FluentGlyphDefn> glyphDefns)
        {
            var query = (from d in glyphDefns where d.IsSelected select d).ToList();
            var add = new List<FluentGlyphDefn>();
            var currentProject = CurrentProjects.FirstOrDefault(p => p.Name == LastSelectedProjectName);
            foreach (var proj in CurrentProjects)
            {
                if (currentProject?.IncludeProjects.Contains(proj.Name) ?? false)
                {
                    var iconAdd = from d in glyphDefns where proj.SelectedIcons.Contains(d.CommonName) select d;
                    query.AddRange(iconAdd);
                }

            }

            return query.Distinct().ToList();
        }
        private List<string> CombinedDistinctCommonNames()
        {
            var currentProject = CurrentProjects.FirstOrDefault(p => p.Name == LastSelectedProjectName);
            var result = currentProject?.SelectedIcons.ToList() ?? new List<string>();
            foreach (var proj in CurrentProjects)
            {
                if (currentProject?.IncludeProjects.Contains(proj.Name) ?? false)
                    result.AddRange(proj.SelectedIcons);
            }
            return result.Order().Distinct().ToList();
        }
        private List<string> GetCombinedEnumNames()
        {
            return GetDistinctSelections(Singleton<GlyphLists>.Instance.RegularIconDefn).Select(d => d.EnumName).ToList();
        }
    }
}
