using FontExamine.Services;

namespace FontExamine.Helper;

public static class CurrentMenuTagConfig
{
    public static MenuTagConfig FluentMenuTag { get; set; } = new MenuTagConfig() { Id = "FluentMenuTag", Page = "Views\\Projects.xaml", HasContextPacket = false };
    public static MenuTagConfig SegoeUISymbol { get; set; } = new MenuTagConfig()
    {
        Id = "SegoeUISymbol",
        Page = "Views\\BrowseSymbolIcons.xaml",
        HasContextPacket = true,
        ContextPacket = new ContextPacketConfig()
        {
            FileName = "SymbolDefinitionsProject.json",
            FontFamily = "Segoe UI Symbol",
            GlyphIds = Singleton<SupportedTables>.Instance.SegoeUISymbol
        }
    };
    public static MenuTagConfig SegoeUIFluent { get; set; } = new MenuTagConfig()
    {
        Id = "SegoeFluent",
        Page = "Views\\BrowseSymbolIcons.xaml",
        HasContextPacket = true,
        ContextPacket = new ContextPacketConfig()
        {
            FileName = "SegoeFluentDefinitionsProject.json",
            FontFamily = "Segoe Fluent Icons",
            GlyphIds = Singleton<SupportedTables>.Instance.SegoeUIFluent
        }
    };
}