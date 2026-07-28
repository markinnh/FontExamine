using System;
using System.Collections.Generic;
using System.Text;

namespace FontExamine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Diagnostics;

    public sealed class TokenAdorner : Adorner
    {
        private readonly TextBox _textBox;
        private readonly Func<IReadOnlyList<string>> _getTokens;
        internal  bool Rendering{ get; set; }
        public TokenAdorner(TextBox adornedElement, Func<IReadOnlyList<string>> getTokens,bool rendering=true)
            : base(adornedElement)
        {
            _textBox = adornedElement ?? throw new ArgumentNullException(nameof(adornedElement));
            _getTokens = getTokens ?? throw new ArgumentNullException(nameof(getTokens));
            Rendering = rendering;
            IsHitTestVisible = false;

            //_textBox.LayoutUpdated += (_, __) => InvalidateVisual();
            _textBox.TextChanged += (_, __) => InvalidateVisual();
            _textBox.SizeChanged += (_, __) => InvalidateVisual();
            //_textBox.ScrollChanged += (_, __) => InvalidateVisual(); // if using a ScrollViewer wrapper
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var tokens = _getTokens() ?? Array.Empty<string>();
            Debug.WriteLine($"In OnRender, token count = {tokens.Count}, rendering = {Rendering}");
            if (tokens.Count == 0 || string.IsNullOrEmpty(_textBox.Text))
                return;
            
            if (!Rendering)
                return;

            var text = _textBox.Text;

            foreach (var token in tokens.Distinct())
            {
                int startIndex = 0;
                while (startIndex < text.Length)
                {
                    int index = text.IndexOf(token, startIndex, StringComparison.Ordinal);
                    if (index < 0)
                        break;

                    DrawToken(drawingContext, token, index);
                    startIndex = index + token.Length;
                }
            }
        }

        private void DrawToken(DrawingContext dc, string token, int startIndex)
        {
            // Get the rect for the first character of the token
            _textBox.Focus();
            Rect startRect = _textBox.GetRectFromCharacterIndex(startIndex, true);
            if (startRect.IsEmpty)
                return;

            // Get the rect for the last character of the token
            int endIndex = startIndex + token.Length - 1;
            Rect endRect = _textBox.GetRectFromCharacterIndex(endIndex, true);
            if (endRect.IsEmpty)
                return;
            //Point screenPos = _textBox.PointToScreen(new Point(0, 0));
            //Point clientPos = _textBox.PointToScreen(screenPos);
            // Single-line token: simple rect
            if (Math.Abs(startRect.Y - endRect.Y) < 0.1)
            {
                var rect = new Rect(
                    startRect.X,
                    startRect.Y,
                    (endRect.X + endRect.Width) - startRect.X,
                    startRect.Height);

                DrawTokenChip(dc, rect, token);
            }
            else
            {
                // Multi-line token: draw per line (simple version: just first line)
                var rect = new Rect(
                    startRect.X,
                    startRect.Y,
                    _textBox.ActualWidth - startRect.X,
                    startRect.Height);

                DrawTokenChip(dc, rect, token);
            }
        }

        private void DrawTokenChip(DrawingContext dc, Rect rect, string token)
        {
            const double paddingX = 3;
            const double paddingY = 1;
            const double radius = 4;

            var background = new SolidColorBrush(Color.FromRgb(0xCC, 0xE5, 0xFF));
            var border = new Pen(new SolidColorBrush(Color.FromRgb(0x55, 0x99, 0xFF)), 1);
            var foreground = new SolidColorBrush(Color.FromRgb(0x8e, 0x90, 0x89));

            var chipRect = new Rect(
                rect.X - paddingX,
                rect.Y + paddingY,
                rect.Width + paddingX * 2,
                rect.Height - paddingY * 2);

            dc.DrawRoundedRectangle(background, border, chipRect, radius, radius);

            var ft = new FormattedText(
                token,
                System.Globalization.CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(_textBox.FontFamily, _textBox.FontStyle, _textBox.FontWeight, _textBox.FontStretch),
                _textBox.FontSize,
                foreground,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            var textPoint = new Point(chipRect.X + paddingX, chipRect.Y + (chipRect.Height - ft.Height) / 2);
            dc.DrawText(ft, textPoint);
        }
    }
}
