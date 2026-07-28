using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace FontExamine.Model
{
    public partial class GlyphDocument : ObservableObject
    {
        internal static bool LoadingDocument {  get; set; }

        const string getUpperCharRegex = @"(?<character>[A-Z])";
        static Regex splitUpperCharRegex = new Regex(getUpperCharRegex, RegexOptions.Compiled);
        [ObservableProperty]
        private int glyphId;
        [ObservableProperty]
        private string glyphName;
        [ObservableProperty]
        private string glyphDescription = string.Empty;

        public static implicit operator GlyphDocument(LightGlyphDefn glyph)
        {
            return new GlyphDocument() { GlyphId = glyph.GlyphId };
        }
        [JsonIgnore]
        public string UnicodeChar => char.ConvertFromUtf32(GlyphId);
        partial void OnGlyphNameChanged(string? oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(GlyphDescription) && !LoadingDocument)
            {
                var curpos = 0;
                var result = string.Empty;
                do
                {
                    if (char.IsUpper(newValue[curpos]))
                        result += " " + newValue[curpos].ToString().ToLower();
                    else
                        result += newValue[curpos];

                    curpos++;
                }while(curpos<newValue.Length);
                result += " glyph";
                result= result.Trim();
                Debug.WriteLine($"Suggested description {result}");
                GlyphDescription = result;
                //var matches = splitUpperCharRegex.Split(newValue);
                //if (matches.Length > 0)
                //{
                //    var result = string.Empty; 

                //    var firstchar = true;
                //    for (int i = 0; i < matches.Length; i++)
                //    {
                //        if (i == 0 && !string.IsNullOrEmpty(matches[0]))
                //        {
                //            result = matches[i].ToLower();
                //            firstchar = false;
                //        }
                //        else if (matches[i].Length == 1 && firstchar)
                //        {
                //            result = matches[i].ToLower();
                //            firstchar = false;
                //        }
                //        else if (matches[i].Length == 1 && !firstchar)
                //            result += " " + matches[i].ToLower();
                //        else
                //            result += matches[i];
                //    }
                //    result += " glyph";
                //    //var result = string.Join("", from match in matches select match.ToLower()) + " glyph";
                //    GlyphDescription = result;
                //}
            }
        }
        
        public string DictionaryEntry(string symbolName="SegoeSymbol") => $"{{ {symbolName}Symbols.{GlyphName} , char.ConvertFromUtf32(0x{GlyphId:x4})}}";
    }
}
