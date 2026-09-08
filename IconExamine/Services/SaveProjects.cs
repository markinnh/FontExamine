using FontExamine.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace FontExamine.Services
{
    internal class SerializeProjects
    {
        public static string DataDir { get; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Settings1.Default.DataDir);
        internal static string ProjectsDataFilePath { get; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Settings1.Default.DataDir, "ProjectsData.json");
        public static void SaveProjects(FLuentSymbolsProjectsDefn defn)
        {
            //var datadir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Settings1.Default.DataDir);
            var options = Singleton<CachedItems>.Instance.Dynamic.AppJsonSerializerOptions ?? Singleton<CachedItems>.Instance.DefaultJsonSerializerOptions;
            string json = System.Text.Json.JsonSerializer.Serialize(defn, options);
            System.IO.File.WriteAllText(ProjectsDataFilePath, json);
        }
        public static FLuentSymbolsProjectsDefn LoadProjects()
        {
            //var datadir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Settings1.Default.DataDir);
            if (System.IO.File.Exists(ProjectsDataFilePath))
            {
                string json = System.IO.File.ReadAllText(ProjectsDataFilePath);
                var options = Singleton<CachedItems>.Instance.Dynamic.AppJsonSerializerOptions;// ?? Singleton<CachedItems>.Instance.DefaultJsonSerializerOptions;
                return System.Text.Json.JsonSerializer.Deserialize<FLuentSymbolsProjectsDefn>(json,options) ?? GetDefaultProjects();
            }
            else
            {

                return GetDefaultProjects();
            }
        }
        private static FLuentSymbolsProjectsDefn GetDefaultProjects()
        {
            var defn = new FLuentSymbolsProjectsDefn();
#if DEBUG
            defn.AddDefaultProject();
#endif
            return defn;
        }
    }
}
