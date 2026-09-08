using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace FontExamine.Model
{
    public partial class LightGlyphDefn:ObservableObject
    {
        [ObservableProperty]
        [property: JsonIgnore]
        private string _unicodeChar;
        [ObservableProperty]
        private int _glyphId;
        [ObservableProperty]
        [property: JsonIgnore]
        private string _xamlString;
        
        [ObservableProperty]
        [property: JsonIgnore]
        private bool _hasDocument=false;
        public static implicit operator LightGlyphDefn(int glyphId)
        {
            return new LightGlyphDefn() { UnicodeChar = char.ConvertFromUtf32(glyphId), GlyphId = glyphId };
        }
        partial void OnGlyphIdChanged(int oldValue, int newValue)
        {
            XamlString = $"&#x{newValue:x4};";
        }
        //public string FontFamilyName { get; set; } = "Segoe UI Symbol";  // Obviously, this is a default value. It can be changed to any other font family name as needed.
        internal string GenerateXaml(string str,string activeFontFamilyName)
        {
            var styleName= activeFontFamilyName.Replace(" ", "");
            if (Enum.TryParse<XamlElementSupported>(str, true, out var element))
                return GenerateXaml(element, activeFontFamilyName,styleName);
            else
                return string.Empty;

            //switch (str)
            //{
            //    case "Button":
            //        return $"<Button Content=\"{XamlString}\" Style=\"{{StaticResource SegoeUISymbolIcon}}\"/>";
            //        break;
            //    case "TextBlock":
            //        return $"<TextBlock Text=\"{XamlString}\" FontSize=\"20\" FontFamily=\"{FontFamilyName}\" />";
            //    default:
            //        Debug.WriteLine($"Generate Xaml not supported for {str}");
            //        return string.Empty;
            //        break;
            //}
        }
        internal string GenerateXaml(XamlElementSupported element,string ActiveFontFamilyName,string StyleName) => element
            switch
        {
            XamlElementSupported.TextBlock => $"<TextBlock Text=\"{XamlString}\" FontSize=\"20\" FontFamily=\"{ActiveFontFamilyName}\" />",
            XamlElementSupported.Label => $"<Label Content=\"{XamlString}\" FontSize=\"20\" FontFamily=\"{ActiveFontFamilyName}\" />",
            XamlElementSupported.FontSymbolIcon => $"<FontImageSource FontFamily=\"{ActiveFontFamilyName}\" Glyph=\"{XamlString}\" />",
            XamlElementSupported.Button => $"<Button Content=\"{XamlString}\" Style=\"{{StaticResource {StyleName}IconButton}}\" Command=\"{{Binding NotImplementedCommand}}\"/>",
            XamlElementSupported.ToggleButton => $"<ToggleButton Content=\"{XamlString}\" Style=\"{{StaticResource {StyleName}SymbolIconToggleButton}}\" IsChecked=\"{{Binding ToBeDetermined}}\"/>",
            _ => string.Empty
        };
    }
}
