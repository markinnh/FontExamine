using FontExamine;
using FontExamine.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace FontExamine.Model
{
    public class GlyphLists
    {
        public ObservableCollection<FluentGlyphDefn> RegularIconDefn { get; set; }
        public ObservableCollection<FluentGlyphDefn> FilledIconDefn { get; set; }

        public void LoadIconLists()
        {
            var datadir =Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Settings1.Default.DataDir);
            RegularIconDefn = LoadIcons.LoadFluentSymbolGlyphList(Path.Combine(datadir,"FluentSystemIcons-Regular.json"));
            FilledIconDefn = LoadIcons.LoadFluentSymbolGlyphList(Path.Combine(datadir,"FluentSystemIcons-Filled.json"));
        }
    }
}
