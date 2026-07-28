using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace FontExamine.Markup
{
    public class SegoeSymbolMarkupExtension : MarkupExtension
    {
        public SegoeIconTable.SegoeGlyph Symbol { get; set; } = SegoeIconTable.SegoeGlyph.Search;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return SegoeIconTable.GlyphToUnicodeMap[Symbol];
        }
    }
}
