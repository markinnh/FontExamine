using FontExamine;
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
        public MainWindow()
        {
            InitializeComponent();
            if(Settings1.Default.LastPage is string str && !string.IsNullOrEmpty(str))
            {
                ViewFrame.Navigate(new Uri(str, UriKind.RelativeOrAbsolute));
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
            if (sender is MenuItem menu && menu.Tag is string str) { 
                Settings1.Default.LastPage = str;
                Settings1.Default.Save();
                ViewFrame.Navigate(new Uri(str,UriKind.RelativeOrAbsolute));
            }
        }
    }
}