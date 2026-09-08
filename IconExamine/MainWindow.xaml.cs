using FontExamine;
using FontExamine.Helper;
using FontExamine.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IconExamine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MenuItem lastChecked;
        private bool hasPacket = false;
        private ContextPacketConfig? packetConfig;
        public MainWindow()
        {

            InitializeComponent();
            if (Settings1.Default.LastPage is string str && !string.IsNullOrEmpty(str))
            {
                var target = string.Empty;
                foreach (MenuItem item in ViewMenuItem.Items)
                {
                    if (item.Tag is MenuTagConfig tagConfig && tagConfig.Id == str)
                    {
                        item.IsChecked = true;
                        lastChecked = item;
                        target = tagConfig.Page;
                        hasPacket = tagConfig.HasContextPacket;
                        packetConfig = tagConfig.ContextPacket;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(target))
                {
                    ViewFrame.Navigate(new Uri(target, UriKind.RelativeOrAbsolute));
                }
            }
            this.Icon = CreateIconFromChar('\uE11a', "Segoe Fluent Icons", 32);
        }
        public static ImageSource CreateIconFromChar(char c, string fontFamily, int size = 32)
        {
            var formattedText = new FormattedText(
                c.ToString(),
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily),
                size,
                Brushes.Black,
                1.25);

            var drawing = new DrawingVisual();
            using (var context = drawing.RenderOpen())
            {
                context.DrawText(formattedText, new Point(0, 0));
            }

            var bmp = new RenderTargetBitmap(
                (int)formattedText.Width,
                (int)formattedText.Height,
                96, 96,
                PixelFormats.Pbgra32);

            bmp.Render(drawing);
            return bmp;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menu && menu.Tag is MenuTagConfig tagConfig)
            {
                Settings1.Default.LastPage = tagConfig.Id;
                menu.IsChecked = true;
                if (lastChecked != null && lastChecked != menu)
                    lastChecked.IsChecked = false;
                lastChecked = menu;
                Settings1.Default.Save();
                hasPacket = tagConfig.HasContextPacket;
                packetConfig = tagConfig.ContextPacket;
                ViewFrame.Navigate(new Uri(tagConfig.Page, UriKind.RelativeOrAbsolute));
            }
        }

        private void ViewFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (hasPacket)
            {

                if (ViewFrame.Content is Page page &&  page.DataContext is IContextPacketConsumer consumer && packetConfig != null)
                {
                    if(page is BrowseSymbolIcons browseSymbolIcons)
                    {
                        browseSymbolIcons.SetFontFamily(packetConfig.FontFamily);
                    }
                    consumer.ConsumeContextPacket(packetConfig);
                    hasPacket = false;
                    packetConfig = null;
                }
            }
        }
    }
}