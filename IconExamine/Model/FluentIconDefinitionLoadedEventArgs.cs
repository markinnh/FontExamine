using System;
using System.Collections.Generic;
using System.Text;

namespace FontExamine.Model
{
    internal class FluentIconDefinitionLoadedEventArgs:EventArgs
    {
        //public string FilePath { get; set; }
        public bool Modified { get; set; }
    }
}
