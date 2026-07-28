using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FontExamine.Model;
using FontExamine.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace FontExamine.ViewModel
{
    public partial class ProjectPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private FLuentSymbolsProjectsDefn projects;
        [ObservableProperty]
        private FluentIconDefProject? selectedProject;
        [ObservableProperty]
        private CollectionViewSource projectsCollectionViewSource;
        [ObservableProperty]
        private CollectionViewSource regularIconCollectionViewSource;
        [ObservableProperty]
        private ObservableCollection<CheckableString> _includeProjects;
        [ObservableProperty]
        private CollectionViewSource filledIconCollectionViewSource;
        [ObservableProperty]
        private FluentGlyphDefn? _selectedRegularIcon;
        [ObservableProperty]
        private FluentGlyphDefn? _selectedFilledIcon;
        [ObservableProperty]
        private string _generatedCode;
        [ObservableProperty]
        private string _searchText;
        [ObservableProperty]
        private string _selectedCommonName;
        [ObservableProperty]
        private string _testTemplate= "<Button Content=\"{UnicodeXamlString}\" Style=\"{{StaticResource FluentButton{StyledName}Glyph}}\" />";
        [ObservableProperty]
        private string _appliedTemplate;
        //public string SearchText
        //{
        //    get => _searchText;
        //    set
        //    {
        //        SetProperty(ref _searchText, value);
        //        // Apply filtering to the CollectionViewSource based on the search text

        //        RegularIconCollectionViewSource.View.Refresh();
        //        FilledIconCollectionViewSource.View.Refresh();
        //    }
        //}

        partial void OnSearchTextChanged(string oldValue, string newValue)
        {
            // Apply filtering to the CollectionViewSource based on the search text
            RegularIconCollectionViewSource.View.Refresh();
            FilledIconCollectionViewSource.View.Refresh();
        }
        partial void OnSelectedCommonNameChanged(string oldValue, string newValue)
        {
            if (!string.IsNullOrWhiteSpace(newValue))
            {
                SelectedProject?.LastCommonNameSelected = newValue;
                var regularMatch = Singleton<GlyphLists>.Instance.RegularIconDefn.FirstOrDefault(d => d.CommonName == newValue);
                var filledMatch = Singleton<GlyphLists>.Instance.FilledIconDefn.FirstOrDefault(d => d.CommonName == newValue);
                if (regularMatch != null)
                {
                    SelectedRegularIcon = regularMatch;
                }
                if (filledMatch != null)
                {
                    SelectedFilledIcon = filledMatch;
                }
            }
        }
        public ProjectPageViewModel()
        {
            Projects = SerializeProjects.LoadProjects();
            int initialHash = Projects.GetHashCode();
#if DEBUG
            Singleton<FLuentSymbolsProjectsDefn>.Instance.AddDefaultProject();
#endif
            ProjectsCollectionViewSource = new CollectionViewSource() { Source = Projects.CurrentProjects };
            if (Projects.CurrentProjects.Count > 0)
            {
                SelectedProject = Projects.CurrentProjects.FirstOrDefault(p => p.Name == Projects.LastSelectedProjectName);
            }
            else
                SelectedProject = null;

            RegularIconCollectionViewSource = new CollectionViewSource() { Source = Singleton<GlyphLists>.Instance.RegularIconDefn };
            FilledIconCollectionViewSource = new CollectionViewSource() { Source = Singleton<GlyphLists>.Instance.FilledIconDefn };
            RegularIconCollectionViewSource.Filter += new FilterEventHandler(ApplyFilter);
            FilledIconCollectionViewSource.Filter += new FilterEventHandler(ApplyFilter);
            SearchText = string.Empty;
        }
        private void ApplyFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is FluentGlyphDefn defn)
            {
                e.Accepted = FilterIcons(defn);
            }
        }
        partial void OnSelectedProjectChanged(FluentIconDefProject? oldValue, FluentIconDefProject? newValue)
        {
            if (newValue != null)
            {
                Projects.LastSelectedProjectName = newValue.Name;
                // Update the IsSelected property of icons based on the selected project
                foreach (var d in Singleton<GlyphLists>.Instance.RegularIconDefn)
                {
                    d.IsSelected = newValue.SelectedIcons.Contains(d.CommonName);
                }
                foreach (var d in Singleton<GlyphLists>.Instance.FilledIconDefn)
                {
                    d.IsSelected = newValue.SelectedIcons.Contains(d.CommonName);
                }
                // refresh the includedProjectCollection including updating the checkboxes
                // first update the old project if it exists
                if (oldValue != null)
                {
                    var oldIncludeProjects = from d in IncludeProjects where d.Checked select d.Name;
                    oldValue.IncludeProjects = new(oldIncludeProjects);
                }
                if (newValue != null)
                {
                    var newIncludeProjects = from p in Projects.CurrentProjects where p.Name != newValue.Name select new CheckableString() { Name = p.Name, Checked = newValue.IncludeProjects?.Contains(p.Name) ?? false };
                    IncludeProjects = new ObservableCollection<CheckableString>(newIncludeProjects);
                }
            }
        }
        private bool FilterIcons(object obj)
        {
            if (obj is FluentGlyphDefn defn)
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                    return true; // No filter applied

                return defn.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
        [RelayCommand]
        private void ApplyTemplate()
        {
            if (SelectedFilledIcon != null)
            {
                AppliedTemplate = SelectedFilledIcon.ApplyTemplate(TestTemplate);
            }
            else if (SelectedRegularIcon != null)
            {
                AppliedTemplate = SelectedRegularIcon.ApplyTemplate(TestTemplate);
            }
            else
            {
                MessageBox.Show("You must first select a glyph to apply the template to, priority is given to the filled icons.");
            }
        }
        [RelayCommand]
        private void NotImplemented()
        {
            MessageBox.Show("This feature is not yet implemented.");
        }
        [RelayCommand]
        private void AddProject()
        {
            var newProject = new FluentIconDefProject() { Name = $"Project {Projects.CurrentProjects.Count + 1}", Description = "New project description", SelectedIcons = new ObservableCollection<string>() };
            Projects.CurrentProjects.Add(newProject);
            SelectedProject = newProject;
        }
        [RelayCommand]
        private void RemoveProject()
        {
            if (SelectedProject != null)
            {
                var ask = MessageBox.Show($"Deleted {SelectedProject.Name}? Once deleted projects cannot be recovered.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (ask == MessageBoxResult.Yes)
                {
                    
                    WeakReferenceMessenger.Default.Send(new ProjectDeletedEventArgs() { ProjectName = new(SelectedProject.Name) });
                    Projects.CurrentProjects.Remove(SelectedProject);
                    SelectedProject = null;
                }
            }
        }
        [RelayCommand]
        private void SelectRegular()
        {
            SynchronizeContent();
        }
        [RelayCommand]
        private void ExportSelectedProject()
        {
            if (SelectedProject != null)
            {
                SynchronizeContent();
                Projects.ExportCurrentProject();
            }
        }
        private void SynchronizeContent()
        {
            var updateList = from d in Singleton<GlyphLists>.Instance.FilledIconDefn where d.IsSelected select d;
            foreach (var d in Singleton<GlyphLists>.Instance.RegularIconDefn)
            {

                d.IsSelected = updateList.Any(u => u.CommonName == d.CommonName);
            }
            if (SelectedProject != null)
            {
                SelectedProject.SelectedIcons = new(from d in Singleton<GlyphLists>.Instance.RegularIconDefn where d.IsSelected select d.CommonName);
                SelectedProject.IncludeProjects = new(from d in IncludeProjects where d.Checked select d.Name);
            }
        }

        [RelayCommand]
        private void SaveProject()
        {
            if (SelectedProject != null)
            {
                SynchronizeContent();
                // Update the selected icons in the project based on the current selections
                //var selectedIcons = from d in Singleton<GlyphLists>.Instance.RegularIconDefn where d.IsSelected select d.CommonName;
                //SelectedProject.SelectedIcons = new(selectedIcons);
                //SelectedProject.IncludeProjects = new(from d in IncludeProjects where d.Checked select d.Name);
                // Save the projects to persistent storage
                SerializeProjects.SaveProjects(Projects);
            }
        }
        [RelayCommand]
        private void GenerateStyledXaml(object p)
        {
            if (p is object[] objs && objs.Length == 2)
            {
                var args = objs.Select(o => o.ToString()).ToArray();
                var targetIcon = args[0] == "Filled" ? SelectedFilledIcon : SelectedRegularIcon;
                if (targetIcon != null && !string.IsNullOrEmpty(args[1]))
                {
                    var xaml = targetIcon.GetXamlContent(args[1]);
                    Clipboard.SetText(xaml);
                    GeneratedCode = xaml;
                    MessageBox.Show("XAML code for the selected font glyph has been copied to the clipboard and placed on the code page.");
                }
            }
        }
        /*
        [RelayCommand]
        private void GenerateXamlButton(object p)
        {
            if (p is string str)
            {
                var targetIcon = str == "Filled" ? SelectedFilledIcon : SelectedRegularIcon;
                if (targetIcon != null)
                {
                    var xaml = targetIcon.GetXamlContent( GlyphDefn.XamlElementSupported.Button);
                    Clipboard.SetText(xaml);
                    GeneratedCode = xaml;
                    MessageBox.Show("XAML code for the selected font glyph has been copied to the clipboard and placed on the code page.");
                }

            }
        }
        [RelayCommand]
        private void GenerateXamlTextBlock(object p)
        {
            if (p is string str)
            {
                var targetIcon = str == "Filled" ? SelectedFilledIcon : SelectedRegularIcon;
                if (targetIcon != null)
                {
                    var xaml = targetIcon.GetXamlContent(GlyphDefn.XamlElementSupported.TextBlock);
                    Clipboard.SetText(xaml);
                    GeneratedCode = xaml;
                    MessageBox.Show("XAML code for the selected font glyph has been copied to the clipboard and placed on the code page.");
                }
            }
        }
        [RelayCommand]
        private void GenerateXamlLabel(object p)
        {
            if (p is string str)
            {
                var targetIcon = str == "Filled" ? SelectedFilledIcon : SelectedRegularIcon;
                if (targetIcon != null)
                {
                    var xaml = targetIcon.GetXamlContent( GlyphDefn.XamlElementSupported.Label);
                    Clipboard.SetText(xaml);
                    GeneratedCode = xaml;
                    MessageBox.Show("XAML code for the selected font glyph has been copied to the clipboard and placed on the code page.");
                }
            }
        }
        [RelayCommand]
        private void GenerateXamlFontSymbolIcon(object p)
        {
            if (p is string str)
            {
                var targetIcon = str == "Filled" ? SelectedFilledIcon : SelectedRegularIcon;
                if (targetIcon != null)
                {
                    var xaml = targetIcon.GetXamlContent(GlyphDefn.XamlElementSupported.FontSymbolIcon);
                    Clipboard.SetText(xaml);
                    GeneratedCode = xaml;
                    MessageBox.Show("XAML code for the selected font glyph has been copied to the clipboard and placed on the code page.");
                }
            }
        }
        private string generateButtonXaml(GlyphDefn icon)
        {
            return $"<Button Content=\"{icon.UnicodeXamlString}\" Style=\"{{StaticResource FluentButton{(icon.CanCheck ? "Filled" : "Regular")}Glyph}}\" Command=\"{{Binding NotImplementedCommand}}\"/>";
        }
        private string generateTextBlockXaml(GlyphDefn icon)
        {
            return $"<TextBlock Text=\"{icon.UnicodeXamlString}\" Style=\"{{StaticResource FluentTextBlock{(icon.CanCheck ? "Filled" : "Regular")}Glyph}}\"/>";
        }
        private string generateXamlLabel(GlyphDefn icon)
        {
            return $"<Label Content=\"{icon.UnicodeXamlString}\" Style=\"{{StaticResource FluentLabel{(icon.CanCheck ? "Filled" : "Regular")}Glyph}}\"/>";
        }
        */
        [RelayCommand]
        private void GenerateCode()
        {
            SynchronizeContent();
            //var code = new StringBuilder();
            // Code generation logic goes here, using the selected icons from both lists
            // the enums would be the easiest to generate
            //code.AppendLine("// This code is generated based on the selected icons in the project.");
            //code.AppendLine("// It defines an enum for the icons and a dictionary to map the enum values to their corresponding Unicode characters.");
            //code.AppendLine($"// this code is generated by a tool, it can be automatically regenerated.  Generated :{DateTime.Now:f}");
            //code.AppendLine("namespace YourNamespace.Markup;");
            //code.AppendLine("public static class FluentIconHelper");
            //code.AppendLine("{");
            //var tabcount = 1;
            //var tabs = new string('\t', tabcount);
            //code.AppendLine($"{tabs}public enum FluentSymbols");
            //code.AppendLine($"{tabs}{{");
            //var firstline = true;
            //tabs = new string('\t', tabcount + 1);
            //var commonNames = CombinedDistinctCommonNames();
            //var enumNames = GetCombinedEnumNames();
            //var query = GetDistinctSelections(Singleton<GlyphLists>.Instance.RegularIconDefn);
            //var distinctList = query.Distinct<GlyphDefn>();
            //var enumCode =CodeGeneration.GenerateEnums("FluentSymbols", enumNames);
            //code.Append(string.Join(",\n", commonNames));
            ////foreach (var d in enumNames)
            ////{
            ////    // Save for later \u{d.Hexcode.Replace("\"", " ").Trim().Remove(0,2)}

            ////    if (!firstline)
            ////        code.AppendLine(",");
            ////    else
            ////        firstline = false;

            ////    code.Append(@$"{tabs}{d}");
            ////}
            //code.AppendLine();
            //tabs = new string('\t', tabcount);
            //code.AppendLine($"{tabs}}}");
            // generate the dictionary for the icon hex values and names
            //var filledGlyphs = GetDistinctSelections(Singleton<GlyphLists>.Instance.FilledIconDefn);
            //var dictionaryCode =CodeGeneration.GenerateDictionaryDefinition("Regular", "FluentSymbols", filledGlyphs.Select(d => d.DictionaryEntry("Fluent")));
            //code.AppendLine($"{tabs}public static readonly Dictionary<FluentSymbols, string> Regular=new()");
            ////tabs = new string('\t', tabcount+1);
            //code.AppendLine($"{tabs}{{");
            //firstline = true;
            //tabs = new string('\t', tabcount + 1);
            ////query = from d in Singleton<GlyphLists>.Instance.RegularIconDefn where d.IsSelected select d;
            //code.Append(string.Join(",\n", Singleton<GlyphLists>.Instance.RegularIconDefn.Where(d => commonNames.Contains(d.CommonName)).Select(d=>d.DictionaryEntry)));
            ////foreach (var d in Singleton<GlyphLists>.Instance.RegularIconDefn.Where(d=>commonNames.Contains(d.CommonName)))
            ////{
            ////    if (commonNames.Contains(d.CommonName))
            ////    {
            ////        if (!firstline)
            ////            code.AppendLine(",");
            ////        else
            ////            firstline = false;

            ////        code.Append(@$"{tabs}{d.DictionaryEntry}");
            ////    }
            ////}
            ////code.AppendLine("}");
            //code.AppendLine("};");
            //code.AppendLine();
            //code.AppendLine("// Similar code can be generated for filled icons if needed");
            //dictionaryCode +=CodeGeneration.GenerateDictionaryDefinition("Filled", "FluentSymbols", GetDistinctSelections(Singleton<GlyphLists>.Instance.FilledIconDefn).Select(d => d.DictionaryEntry("Fluent")));
            //tabs = new string('\t', tabcount);
            //code.AppendLine($"{tabs}public static readonly Dictionary<FluentSymbols, string> Filled=new()");
            //code.AppendLine($"{tabs}{{");
            //firstline = true;
            //tabs = new string('\t', tabcount + 1);
            ////query = GetDistinctSelections(Singleton<GlyphLists>.Instance.FilledIconDefn);
            //code.Append(string.Join(",\n", Singleton<GlyphLists>.Instance.FilledIconDefn.Where(d => commonNames.Contains(d.CommonName)).Select(d => d.DictionaryEntry)));
            ////foreach (var d in Singleton<GlyphLists>.Instance.FilledIconDefn)
            ////{
            ////    if (commonNames.Contains(d.CommonName))
            ////    {
            ////        if (!firstline)
            ////            code.AppendLine(",");
            ////        else
            ////            firstline = false;

            ////        code.Append(@$"{tabs}{d.DictionaryEntry}");
            ////    }
            ////}
            ////code.AppendLine("}");
            //code.AppendLine("};");
            //tabs = new string('\t', tabcount);
            //code.AppendLine($"}}  // End of generated code");
            var content = Projects.GenerateStaticClass();
            GeneratedCode = content;
            Clipboard.SetText(content);
            MessageBox.Show("Rudimentary code generation is implemented.  The result are placed on the clipboard.");
        }

        private List<FluentGlyphDefn> GetDistinctSelections(IEnumerable<FluentGlyphDefn> glyphDefns)
        {
            var query = (from d in glyphDefns where d.IsSelected select d).ToList();
            var add = new List<FluentGlyphDefn>();
            foreach (var proj in Projects.CurrentProjects)
            {
                if (SelectedProject?.IncludeProjects.Contains(proj.Name) ?? false)
                {
                    var iconAdd = from d in glyphDefns where proj.SelectedIcons.Contains(d.CommonName) select d;
                    query.AddRange(iconAdd);
                }

            }

            return query.Distinct().ToList();
        }
        private List<string> CombinedDistinctCommonNames()
        {
            var result = SelectedProject?.SelectedIcons.ToList()?? new List<string>();
            foreach(var proj in Projects.CurrentProjects)
            {
                if (SelectedProject?.IncludeProjects.Contains(proj.Name) ?? false)
                    result.AddRange(proj.SelectedIcons);
            }
            return result.Order().Distinct().ToList();
        }
        private List<string> GetCombinedEnumNames()
        {
            return GetDistinctSelections(Singleton<GlyphLists>.Instance.RegularIconDefn).Select(d=>d.EnumName).ToList();
        }
    }
}
