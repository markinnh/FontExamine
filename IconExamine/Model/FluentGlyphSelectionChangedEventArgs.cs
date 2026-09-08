using System;
using System.Collections.Generic;
using System.Text;

namespace FontExamine.Model
{
    internal class FluentGlyphSelectionChangedEventArgs:EventArgs
    {
        public FluentGlyphSelectionChangedEventArgs(FluentGlyphDefn glyph, bool isSelected)
        {
            Glyph = glyph;
            IsSelected = isSelected;
        }

        public FluentGlyphDefn Glyph { get; }
        public bool IsSelected { get; }
    }
}
