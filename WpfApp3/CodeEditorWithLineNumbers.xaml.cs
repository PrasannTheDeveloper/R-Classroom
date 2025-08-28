using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace WpfApp3
{
    public partial class CodeEditorWithLineNumbers : UserControl
    {
        public TextBox CodeTextBox => codeTextBox;
        private ScrollViewer _codeScrollViewer;
        private ScrollViewer _lineNumbersScrollViewer;

        public CodeEditorWithLineNumbers()
        {
            InitializeComponent();
            codeTextBox.TextChanged += CodeTextBox_TextChanged;
            codeTextBox.Loaded += CodeTextBox_Loaded;
            lineNumbersTextBox.Loaded += LineNumbersTextBox_Loaded;
            UpdateLineNumbers();
        }

        private void CodeTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            // Get the ScrollViewer from the TextBox's template
            _codeScrollViewer = GetScrollViewer(codeTextBox);
            if (_codeScrollViewer != null)
            {
                _codeScrollViewer.ScrollChanged += CodeScrollViewer_ScrollChanged;
            }
        }

        private void LineNumbersTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            // Get the ScrollViewer from the line numbers TextBox's template
            _lineNumbersScrollViewer = GetScrollViewer(lineNumbersTextBox);
        }

        private ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer scrollViewer)
                return scrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                var result = GetScrollViewer(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void CodeScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Sync the line numbers scroll position with the code editor
            if (_lineNumbersScrollViewer != null)
            {
                _lineNumbersScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
                _lineNumbersScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
            }

            // Adjust the height of the line numbers to match the code editor
            lineNumbersTextBox.Height = codeTextBox.ActualHeight;
        }

        private void CodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateLineNumbers();
        }

        private void UpdateLineNumbers()
        {
            int lineCount = codeTextBox.LineCount;
            var lineNumbers = new StringBuilder();
            for (int i = 1; i <= lineCount; i++)
            {
                lineNumbers.AppendLine(i.ToString());
            }
            lineNumbersTextBox.Text = lineNumbers.ToString();

            // Adjust width of line numbers based on the number of digits
            int maxDigits = lineCount.ToString().Length;
            double width = maxDigits * 7 + 20; // 20 for padding
            lineNumbersTextBox.Width = width;
        }

        public new string Text
        {
            get => codeTextBox.Text;
            set => codeTextBox.Text = value;
        }

        public int CaretIndex
        {
            get => codeTextBox.CaretIndex;
            set => codeTextBox.CaretIndex = value;
        }

        public void HighlightErrorLines(int[] errorLines)
        {
            // Reset all line colors
            lineNumbersTextBox.Foreground = new SolidColorBrush(Colors.Black);

            if (errorLines == null || errorLines.Length == 0)
                return;

            // Create a formatted text with error lines highlighted
            var lineNumbers = new StringBuilder();
            int lineCount = codeTextBox.LineCount;

            for (int i = 1; i <= lineCount; i++)
            {
                if (errorLines.Contains(i))
                {
                    lineNumbers.AppendLine($"⚠️ {i}");
                }
                else
                {
                    lineNumbers.AppendLine(i.ToString());
                }
            }

            lineNumbersTextBox.Text = lineNumbers.ToString();
        }

        public void SetTransparency(double opacity)
        {
            // Set background opacity for both codeTextBox and lineNumbersTextBox
            var editorBrush = new SolidColorBrush(Colors.White) { Opacity = opacity };
            codeTextBox.Background = editorBrush;
            lineNumbersTextBox.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238)) { Opacity = opacity };
            this.Background = Brushes.Transparent;
        }
    }
}