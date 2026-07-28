using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace FontExamine
{
    public class OldTokenizedTextBox : Control
    {
        TextBox? _textBox;
        ItemsControl? _itemsControl;
        bool _firstVisit = true;
        static OldTokenizedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(OldTokenizedTextBox),
                new FrameworkPropertyMetadata(typeof(OldTokenizedTextBox)));
        }
        public string DisplayText
        {
            get => (string)GetValue(DisplayTextProperty);
            set => SetValue(DisplayTextProperty, value);
        }

        public static readonly DependencyProperty DisplayTextProperty =
            DependencyProperty.Register(nameof(DisplayText), typeof(string),
                typeof(OldTokenizedTextBox), new PropertyMetadata(string.Empty));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string),
                typeof(OldTokenizedTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        public ObservableCollection<string> Tokens
        {
            get => (ObservableCollection<string>)GetValue(TokensProperty);
            set => SetValue(TokensProperty, value);
        }

        public static readonly DependencyProperty TokensProperty =
            DependencyProperty.Register(nameof(Tokens), typeof(ObservableCollection<string>),
                typeof(OldTokenizedTextBox), new PropertyMetadata(new ObservableCollection<string>()));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (OldTokenizedTextBox)d;
            control.ParseTokens();
            control.UpdateDisplayText();
        }
        private void UpdateDisplayText()
        {
            if (string.IsNullOrEmpty(Text))
            {
                DisplayText = "";
                return;
            }

            string result = Text;

            foreach (var token in Tokens)
            {
                string spaces = new string(' ', token.Length);
                result = result.Replace(token, spaces);
            }

            DisplayText = result;
        }
        private void ParseTokens()
        {
            Tokens.Clear();

            if (string.IsNullOrEmpty(Text))
                return;

            var matches = Regex.Matches(Text, @"\{([^}]+)\}");

            foreach (Match m in matches)
                Tokens.Add(m.Value);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBox = GetTemplateChild("PART_TextBox") as TextBox;
            _itemsControl = GetTemplateChild("PART_ItemsControl") as ItemsControl;

            if (_firstVisit)
            {
                _textBox?.LayoutUpdated += (s, e) => UpdateTokenPositions();
                _firstVisit = false;
            }
        }

        private void UpdateTokenPositions()
        {
            if (_textBox == null || _itemsControl == null)
                return;
            int index = 0;
            var lastIndex = 0;
            var localText = Text;
            Debug.WriteLine($"Token count = {Tokens.Count}");
            for (int i = 0; i < Tokens.Count; i++)
            {
                string token = Tokens[i];
                index = lastIndex == 0 ? localText.IndexOf(token) : localText.IndexOf(token, lastIndex);

                if (index > 0)
                {

                    Rect rect = _textBox.GetRectFromCharacterIndex(index);
                    //Rect endRect = _textBox.GetRectFromCharacterIndex(index + token.Length);

                    if (_itemsControl.ItemContainerGenerator.ContainerFromIndex(i) is FrameworkElement container)
                    {
                        Canvas.SetLeft(container, rect.X);
                        Canvas.SetTop(container, rect.Y);
                        //Canvas.SetRight(container, endRect.X+endRect.Width);
                        //Canvas.SetBottom(container, endRect.Y+endRect.Height);
                    }
                    lastIndex = ++index;
                }

            }
        }
    }
}
