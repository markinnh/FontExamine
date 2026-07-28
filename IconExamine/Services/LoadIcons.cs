using FontExamine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace FontExamine.Services
{
    public class LoadIcons
    {
        public static ObservableCollection<FluentGlyphDefn> LoadFluentSymbolGlyphList(string filename)
        {
            ObservableCollection<FluentGlyphDefn> icons = new ();
            string[] lines = System.IO.File.ReadAllLines(filename);
            foreach (string line in lines)
            {
                var modline=line.Replace(",", " ");
                string[] parts = modline.Split(':');
                if (parts.Length == 2)
                {
                    FluentGlyphDefn icon = new FluentGlyphDefn
                    {
                        Name = parts[0].Trim(),
                        Hexcode = parts[1].Trim()
                    };
                    icons.Add(icon);
                }
            }
            // eliminate unsupported icons
            var lookuptable = icons.First().CanCheck ? Singleton<SupportedTables>.Instance.Filled: Singleton<SupportedTables>.Instance.Regular;
            for (int i = icons.Count - 1; i >= 0; i--)
            {
                Debug.WriteLineIf(icons[i].UnicodeInt == -1, $"Parse Icon Code failed in {icons[i].CommonName}");
                if (!lookuptable.Contains(icons[i].UnicodeInt))
                {
                    icons.RemoveAt(i);
                }
            }
            return icons;
        }
    }
}
