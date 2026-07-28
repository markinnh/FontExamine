using FontExamine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FontExamine
{
    public class TokenizedTextBox : Control
    {
        static TokenizedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TokenizedTextBox),
                new FrameworkPropertyMetadata(typeof(TokenizedTextBox)));
        }

        private TextBox? _textBox;
        private Popup? _popup;
        private ListBox? _listBox;

        private AdornerLayer? _adornerLayer;
        private TokenAdorner? _adorner;
        public List<string> AvailableParameters { get; set; } = FluentGlyphDefn.SupportedParams.ToList();
        public ObservableCollection<string> Tokens
        {
            get => (ObservableCollection<string>)GetValue(TokensProperty);
            set => SetValue(TokensProperty, value);
        }

        public static readonly DependencyProperty TokensProperty =
            DependencyProperty.Register(nameof(Tokens), typeof(ObservableCollection<string>),
                typeof(TokenizedTextBox), new PropertyMetadata(new ObservableCollection<string>()));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string),
                typeof(TokenizedTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));



        public bool ShowBubbles
        {
            get { return (bool)GetValue(ShowBubblesProperty); }
            set
            {
                SetValue(ShowBubblesProperty, value);
                _adornerLayer?.InvalidateVisual();
                Debug.WriteLine($"ShowBubbles changed to {value}");
            }
        }

        // Using a DependencyProperty as the backing store for ShowBubbles.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowBubblesProperty =
            DependencyProperty.Register(nameof(ShowBubbles), typeof(bool), typeof(TokenizedTextBox), new PropertyMetadata(true, OnShowBubblesChanged));

        private static void OnShowBubblesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TokenizedTextBox)d;

            Debug.WriteLine($"ShowBubbles changed to {e.NewValue}");
            UpdateTokenAdorner(control, (bool)e.NewValue);
            //Debug.WriteLine("applied template {0}",control.ApplyTemplate());
            control.InvalidateVisual();
            //control.OnApplyTemplate();
        }
        private static void UpdateTokenAdorner(TokenizedTextBox control, bool showBubbles)
        {
            if (control._textBox == null)
                return;
            var layer = AdornerLayer.GetAdornerLayer(control._textBox);
            if (layer != null)
            {
                if (control._adorner == null)
                {
                    control._adorner = new TokenAdorner(control._textBox, () => control.Tokens, showBubbles);
                    layer.Add(control._adorner);
                }
                if (control._adorner != null)
                {
                    control._adorner.Rendering = showBubbles;
                }
                control.Dispatcher.BeginInvoke(
                    new Action(() => control._adorner?.InvalidateVisual()),
    DispatcherPriority.Background);
                ;
            }
            //control._adorner?.InvalidateVisual();

        }
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TokenizedTextBox)d;
            control.ParseTokens();
        }

        private void ParseTokens()
        {
            //Debug.WriteLine("In ParseTokens");
            Tokens.Clear();

            if (string.IsNullOrEmpty(Text))
                return;

            var matches = System.Text.RegularExpressions.Regex.Matches(Text, @"\{[^}^{]+\}");
            foreach (System.Text.RegularExpressions.Match m in matches)
                Tokens.Add(m.Value);
            Debug.WriteLine($"In parse tokens, examining {Text}, tokens found: {{{string.Join(',', Tokens)}}}");
        }
        private void ShowSuggestions()
        {
            if (_popup == null || _listBox == null || _textBox == null)
                return;

            // Determine the current fragment inside { ... }
            string? fragment = GetCurrentTokenFragment();
            if (fragment == null)
            {
                _popup.IsOpen = false;
                return;
            }

            // Filter available parameters
            var filtered = AvailableParameters
                .Where(p => p.StartsWith(fragment, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filtered.Count == 0)
            {
                _popup.IsOpen = false;
                return;
            }

            // Populate the list
            _listBox.ItemsSource = filtered;
            _listBox.SelectedIndex = 0;

            // Position the popup at the caret
            PositionPopup();

            // Show it
            _popup.IsOpen = true;
        }
        private void UpdateSuggestions()
        {
            if (_popup != null && _listBox != null)
            {
                string? fragment = GetCurrentTokenFragment();
                if (fragment == null)
                {
                    _popup.IsOpen = false;
                    return;
                }

                var filtered = AvailableParameters
                    .Where(p => p.StartsWith(fragment, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (filtered.Count == 0)
                {
                    _popup.IsOpen = false;
                    return;
                }

                _listBox.ItemsSource = filtered;
                PositionPopup();
                _popup.IsOpen = true;
            }
        }
        private string? GetCurrentTokenFragment()
        {
            int pos = _textBox?.CaretIndex ?? 0;
            string text = _textBox?.Text ?? string.Empty;

            int start = pos == 0 ? text.LastIndexOf("{", 0) : text.LastIndexOf('{', pos - 1);
            if (start < 0) return null;

            int end = text.IndexOf('}', start + 1);
            if (end >= 0 && end < pos) return null;

            return text.Substring(start + 1, pos - (start + 1));
        }
        private void PositionPopup()
        {
            if (_textBox != null && _popup != null)
            {
                int index = _textBox?.CaretIndex ?? 0;
                Rect rect = _textBox?.GetRectFromCharacterIndex(index, true) ?? Rect.Empty;

                _popup.HorizontalOffset = rect.X;
                _popup.VerticalOffset = rect.Y + rect.Height;
            }
        }
        private void CommitSelection()
        {
            if (_listBox?.SelectedItem is not string token)
                return;

            int pos = _textBox?.CaretIndex ?? 0;
            string text = _textBox?.Text ?? string.Empty;

            int start = text.LastIndexOf('{', pos - 1);
            if (start < 0) return;

            string replacement = "{" + token + "}";

            _textBox?.Text = text.Substring(0, start) + replacement + text.Substring(pos);
            _textBox?.CaretIndex = start + replacement.Length;

            _popup.IsOpen = false;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            System.Diagnostics.Debug.WriteLine("In Apply Template");
            _textBox = GetTemplateChild("PART_TextBox") as TextBox;
            _popup = GetTemplateChild("PART_AutoCompletePopup") as Popup;
            _listBox = GetTemplateChild("PART_AutoCompleteList") as ListBox;

            if (_listBox != null)
            {
                _listBox.MouseDoubleClick += (s, e) => CommitSelection();
                _listBox.PreviewKeyDown += (s, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        CommitSelection();
                        e.Handled = true;
                    }
                };
            }
            if (_textBox != null && AdornerLayer.GetAdornerLayer(_textBox) is AdornerLayer layer)
            {
                _adorner = new TokenAdorner(_textBox, () => Tokens, ShowBubbles);
                var _otherAdorners = layer.GetAdorners(_textBox);
                var _oldAdorner = _otherAdorners?.FirstOrDefault(a => a is TokenAdorner);

                if (_oldAdorner != null && _oldAdorner is TokenAdorner token)
                {
                    token.Rendering = ShowBubbles;
                }
                else
                    layer.Add(_adorner);
                _textBox.PreviewTextInput += (s, e) =>
                {
                    if (e.Text == "{")
                        ShowSuggestions();
                };
                _textBox.TextChanged += (s, e) => UpdateSuggestions();
                _textBox.TextChanged += (_, __) =>
                {
                    _textBox.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _adorner?.InvalidateVisual();
                    }), DispatcherPriority.Background);
                };
                _textBox.SelectionChanged += (s, e) => UpdateSuggestions();
                _textBox.SelectionChanged += (_, __) => layer.InvalidateVisual();
                _textBox.AddHandler(ScrollViewer.ScrollChangedEvent,
                    new RoutedEventHandler((s, e) => layer.InvalidateVisual()));
                _textBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _adorner?.InvalidateVisual();
                }), DispatcherPriority.Background);
            }
        }
    }
}
