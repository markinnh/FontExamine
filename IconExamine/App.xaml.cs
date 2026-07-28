using FontExamine.Markup;
using FontExamine.Model;
using FontExamine.Services;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FontExamine
{
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
