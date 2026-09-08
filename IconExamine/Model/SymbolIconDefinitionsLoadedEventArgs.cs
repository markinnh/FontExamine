using System;
using System.Collections.Generic;
using System.Text;

namespace FontExamine.Model
{
    internal class SymbolIconDefinitionsLoadedEventArgs:EventArgs
    {
        public bool Modified { get; set; }
    }
}
