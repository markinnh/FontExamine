using System.ComponentModel;

namespace FontExamine.Model;

public enum GlyphUsedFor
{
    Segoe,
    [Description("Fluent Filled")]
    FluentFilled,
    [Description("Fluent Regular")]
    FluentRegular,
    [Description("Segoe Fluent")]
    SegoeFluent
}
public enum DevelopementLayer
{
    [Description("WPF (Win 7+)")]
    WPF,
    [Description("WinUI (Win 10+)")]
    WinUI,
    
    MAUI
}
public static class EnumNameProvider 
{
    public static Array GlyphUsedFor => Enum.GetValues(typeof(GlyphUsedFor));
    public static Array DevelopementLayer => Enum.GetValues(typeof(DevelopementLayer));
}

/// <summary>
/// Keeps track of which development layers are supported for each GlyphUsedFor value. This dictionary is used to determine the compatibility of glyphs with different development frameworks.
/// </summary>
public static class GlyphSupportDictionary
{
    public static readonly Dictionary<GlyphUsedFor, List<DevelopementLayer>> SupportDictionary = new()
    {
        { GlyphUsedFor.Segoe, new List<DevelopementLayer> { DevelopementLayer.WPF, DevelopementLayer.WinUI } },
        { GlyphUsedFor.FluentFilled, new List<DevelopementLayer> {DevelopementLayer.WPF, DevelopementLayer.WinUI, DevelopementLayer.MAUI } },
        { GlyphUsedFor.FluentRegular, new List<DevelopementLayer> {DevelopementLayer.WPF, DevelopementLayer.WinUI, DevelopementLayer.MAUI } },
        { GlyphUsedFor.SegoeFluent, new List<DevelopementLayer> { DevelopementLayer.WPF, DevelopementLayer.WinUI } }
    };
}