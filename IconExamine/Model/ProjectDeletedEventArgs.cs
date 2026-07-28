using System;
using System.Collections.Generic;
using System.Text;

namespace FontExamine.Model
{
    internal class ProjectDeletedEventArgs:EventArgs
    {
        public string ProjectName { get; set; }
    }
}
