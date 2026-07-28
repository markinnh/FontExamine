using System;
using System.Collections.Generic;
using System.Text;

namespace FontExamine.Model
{
    internal class ProjectRenamedEventArgs: EventArgs
    {
        public string OldName { get; set; }=string.Empty; 
        public string NewName { get; set; } = string.Empty;
    }
}
