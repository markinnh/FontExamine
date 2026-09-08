using FontExamine.Markup;
using FontExamine.Model;
using FontExamine.Services;
using System.Configuration;
using System.Data;
using System.Text.Json.Serialization;
using System.Windows;

namespace FontExamine
{
    // TODO: fix scripting in T4 files.  9/8/2026 T4 files do not support all scenarios that will use the markup extension methods.  Also, templates have kind of taken a back seat to the new source generator approach.  The T4 files are still useful for generating code that is not supported by the source generator approach.
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            Singleton<SupportedTables>.Instance.Init();
            Singleton<GlyphLists>.Instance.LoadIconLists();
            var count = Singleton<SupportedTables>.Instance.Regular.Count + Singleton<SupportedTables>.Instance.Filled.Count;
            //Singleton<CachedItems>.Instance.Dynamic = new System.Dynamic.ExpandoObject();
            Singleton<CachedItems>.Instance.Dynamic.AppJsonSerializerOptions = Singleton<CachedItems>.Instance.DefaultJsonSerializerOptions;
#if DEBUG
            var ignore= TestGlyphHelper.Symbols[TestGlyphHelper.TestSymbols.Search]; // this is just to trigger the generation of the TestGlyphHelper class and its static constructor to load the symbols to make sure there are no dictionary collisions
#endif
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //MainWindow mainWindow = new MainWindow();
            //mainWindow.Show();
        }
    }

}
