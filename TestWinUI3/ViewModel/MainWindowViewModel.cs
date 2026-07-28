using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWinUI3.ViewModel
{
    public class ClickNotification
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ClickNotification> clickNotifications = new ObservableCollection<ClickNotification>();
        [RelayCommand]
        private void ButtonClicked(object value)
        {
            if (value is string p && !string.IsNullOrEmpty(p))
            {
                ClickNotifications.Add(new ClickNotification() { Message = p, Timestamp = DateTime.Now });
            }
        }
    }
}
