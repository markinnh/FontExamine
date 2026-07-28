using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace FontExamine.Model
{
    public partial class CheckableString:ObservableObject
    {
        [ObservableProperty]
        private bool _checked;
        [ObservableProperty]
        private string _name;
        internal CheckableString(string name,bool isChecked=false)
        {
            Name = name;
            Checked = isChecked;
        }
        internal CheckableString() { }
    }
}
