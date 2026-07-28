using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace FontExamine.Services
{
    internal class SupportedTables
    {
        public enum DefinedTables
        {
            Regular,
            Filled,
            SegoeUISymbol,
            SegoeUIFluent
        }
        public Dictionary<DefinedTables, List<int>> Tables { get; set; } = new();
        public List<int> Regular
        {
            get => Tables[DefinedTables.Regular];
        }
        public List<int> Filled
        {
            get => Tables[DefinedTables.Filled];
        }
        public List<int> SegoeUISymbol => Tables[DefinedTables.SegoeUISymbol];
        public List<int> SegoeUIFluent => Tables[DefinedTables.SegoeUIFluent];
        internal void Init()
        {
            if(string.IsNullOrEmpty(Settings1.Default.ProjectPath))
            {
                throw new Exception("Project path is not set in settings.");
            }
            if(!Directory.Exists(Settings1.Default.ProjectPath))
            {
                throw new Exception($"Project path '{Settings1.Default.ProjectPath}' does not exist.  Please adjust the project path in settings to the current project directory.");
            }
            var location = Path.Combine(Settings1.Default.ProjectPath, "Supported");
            Tables[DefinedTables.Regular] = LoadSupported(Path.Combine(location, "Regular.json"));
            Tables[DefinedTables.Filled] = LoadSupported(Path.Combine(location, "Filled.json"));
            Tables[DefinedTables.SegoeUISymbol] = LoadSupported(Path.Combine(location, "Segoe UI Symbol map.json")).Where(i => i >= 0xe000).ToList();
            Tables[DefinedTables.SegoeUIFluent] = LoadSupported(Path.Combine(location, "Segoe UI Fluent map.json")).Where(i => i >= 0xe000).ToList();
        }
        public static List<int> LoadSupported(string filename)
        {
            return JsonSerializer.Deserialize<List<int>>(System.IO.File.ReadAllText(filename)) ?? new();
        }
        public static T LoadJsonContent<T>(string filename)
        {
            return JsonSerializer.Deserialize<T>(System.IO.File.ReadAllText(filename)) ?? throw new Exception($"Failed to deserialize {filename}");
        }
    }
}
