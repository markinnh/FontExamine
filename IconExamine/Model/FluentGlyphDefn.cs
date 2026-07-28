using CommunityToolkit.Mvvm.ComponentModel;
using FontExamine.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace FontExamine.Model
{
public enum XamlElementSupported
        {
            TextBlock,
            Button,
            ToggleButton,
            Label,
            FontSymbolIcon
        }
    public partial class FluentGlyphDefn : ObservableObject
    {
        

        const string unicodeHexPattern = @"""0x(?<code>[a-f0-9]+)""";
        const string regexName = @"ic_fluent_(?<name>[a-z_]+_[0-9]+)";
        const string sizePattern = @"(?<size>[0-9]+)$";
        const string templateTokenPattern = @"{(?<token>[^{^}]+)}";
        static Regex regex = new Regex(regexName, RegexOptions.Compiled);
        static Regex unicodeRegex = new Regex(unicodeHexPattern, RegexOptions.Compiled);
        static Regex sizeRegex = new Regex(sizePattern, RegexOptions.Compiled);
        static Regex templateRegex = new Regex(templateTokenPattern, RegexOptions.Compiled);
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private string _hexcode;
        public string CommonName
        {
            get
            {
                var match = regex.Match(Name);
                if (match.Success)
                {
                    return match.Groups["name"].Value;
                }
                return Name;
            }
        }
        public string FontFamilyName
        {
            get => Name.Contains("regular") ? "FluentSystemIcons-Regular" : "FluentSystemIcons-Filled";
        }
        public bool CanCheck
        {
            get => Name.Contains("filled");
        }
        public string StyledName => Name.Contains("filled") ? "Filled" : "Regular";
        public string EnumName
        {
            get
            {
                var f = CommonName.Replace("_", " ").Trim();
                var parts = f.Split(' ');
                StringBuilder sb = new StringBuilder();
                foreach (var part in parts)
                {
                    //if (char.IsDigit(part[0]))
                    //{
                    //    sb.Append("_");
                    //}
                    sb.Append(char.ToUpper(part[0]) + part.Substring(1));
                }
                return sb.ToString();
            }
        }
        public string UnicodeChar
        {
            get
            {
                if (unicodeRegex.IsMatch(Hexcode))
                {
                    var match = unicodeRegex.Match(Hexcode);
                    if (match.Success)
                    {
                        var unicode = match.Groups["code"].Value;
                        if (int.TryParse(unicode, System.Globalization.NumberStyles.HexNumber, null, out int result))
                        {
                            return char.ConvertFromUtf32(result);
                        }
                    }
                }
                //var f = Hexcode.Replace("\"", " ").Trim().Remove(0, 2);
                //if (int.TryParse(f, System.Globalization.NumberStyles.HexNumber, null, out int code))
                //{
                //    return char.ConvertFromUtf32(code);
                //}
                return char.ConvertFromUtf32(0xf3e9);
            }
        }
        public int UnicodeInt
        {
            get
            {
                if (unicodeRegex.IsMatch(Hexcode))
                {
                    var match = unicodeRegex.Match(Hexcode);
                    if (match.Success)
                    {
                        var unicode = match.Groups["code"].Value;
                        if (int.TryParse(unicode, System.Globalization.NumberStyles.HexNumber, null, out int result))
                        {
                            return result;
                        }
                    }
                }
                return -1;
            }
        }
        public Visibility Visible
        {
            get
            {
                var unicodeInt = UnicodeInt;
                if (Name.Contains("regular"))
                {
                    return Singleton<SupportedTables>.Instance.Regular.Contains(unicodeInt) ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (Name.Contains("filled"))
                {
                    return Singleton<SupportedTables>.Instance.Filled.Contains(unicodeInt) ? Visibility.Visible : Visibility.Collapsed;
                }
                return Visibility.Collapsed;
            }

        }
        public string UnicodeXamlString
        {
            get
            {
                if (unicodeRegex.IsMatch(Hexcode))
                {
                    var match = unicodeRegex.Match(Hexcode);
                    if (match.Success)
                    {
                        var unicode = match.Groups["code"].Value;
                        return $"&#x{unicode};";
                    }
                }
                return string.Empty;

            }
        }
        public string HexString
        {
            get
            {
                if (unicodeRegex.IsMatch(Hexcode))
                {
                    var match = unicodeRegex.Match(Hexcode);
                    if (match.Success)
                    {
                        return $"{match.Groups["code"].Value}";
                    }
                }
                return Hexcode;
            }
        }
        public string IconSize
        {
            get
            {
                if (sizeRegex.IsMatch(EnumName))
                {
                    var match = sizeRegex.Match(EnumName);
                    if (match.Success)
                    {
                        return match.Groups["size"].Value;
                    }
                }
                return string.Empty;
            }
        }
        [ObservableProperty]
        private bool _isSelected;
        public string DictionaryEntry(string symbolName)=> $"{{ {symbolName}Symbols.{EnumName} ,char.ConvertFromUtf32( 0x{HexString} ) }}";
        public string GetXamlContent(XamlElementSupported element)=> element switch
        {
            XamlElementSupported.TextBlock => $"<TextBlock Text=\"{UnicodeXamlString}\" Style=\"{{StaticResource FluentTextBlock{(CanCheck ? "Filled" : "Regular")}Glyph}}\"/>",
            XamlElementSupported.Button => $"<Button Content=\"{UnicodeXamlString}\" Style=\"{{StaticResource FluentButton{StyledName}Glyph}}\" Command=\"{{Binding NotImplementedCommand}}\"/>",
            XamlElementSupported.Label => $"<Label Content=\"{UnicodeXamlString}\" Style=\"{{StaticResource FluentLabel{StyledName}Glyph}}\"/>",
            XamlElementSupported.FontSymbolIcon => $"<FontImageSource FontFamily=\"{FontFamilyName}\" Glyph=\"{UnicodeXamlString}\" />",
            XamlElementSupported.ToggleButton => $"<ToggleButton Content=\"{UnicodeXamlString}\" Style=\"{{StaticResource FluentToggleButton{StyledName}Glyph}}\" IsChecked=\"{{Binding ToBeDetermined}}\"/>",
            _ => string.Empty
        };
        public string GetXamlContent(string elementName)
        {
            if (Enum.TryParse<XamlElementSupported>(elementName, out var element))
            {
                return GetXamlContent(element);
            }
            return string.Empty;
        }
        public string ApplyTemplate(string Template)
        {
            var result = Template;
            var matches = templateRegex.Matches(Template);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (SupportedParams.Contains(match.Groups["token"].Value))
                    {
                        result = result.Replace(match.Value, ParamSelector.Select(this, match.Groups["token"].Value).ToString());
                    }
                }
                result = result.Replace("{{", "{").Replace("}}", "}");
                return result;
            }
            return string.Empty;
        }
        internal static Param.Selector<FluentGlyphDefn> ParamSelector { get; } = Param.From<FluentGlyphDefn>();
        internal static string[] SupportedParams => ParamSelector.Keys;
        static FluentGlyphDefn()
        {
            ParamSelector.Add(nameof(CommonName), x => x.CommonName)
                .Add(nameof(UnicodeXamlString), x => x.UnicodeXamlString)
                .Add(nameof(FontFamilyName), x => x.FontFamilyName)
                .Add(nameof(HexString), x => x.HexString)
                .Add(nameof(UnicodeInt), x => x.UnicodeInt)
                .Add(nameof(IconSize), x => x.IconSize)
                .Add(nameof(EnumName), x => x.EnumName)
                .Add(nameof(StyledName), x => x.StyledName);
        }
    }
}
