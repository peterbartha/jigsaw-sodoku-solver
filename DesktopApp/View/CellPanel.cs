using DesktopApp.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DesktopApp
{
    class CellPanel : Panel
    {
        private Canvas candidateCanavas, resultCanavas;
        private Cell cell;

        public CellPanel(Cell c)
        {
            cell = c;
            InitializeCellPanel();
            BuildCandidateView();   // This is the default view
        }

        private void InitializeCellPanel() {
            this.Height = 60;
            this.Width = 60;
            this.Background = Brushes.White;
        }

        private void BuildCandidateView()
        {
            candidateCanavas = new Canvas();
            candidateCanavas.Width = 60;
            candidateCanavas.Height = 60;
            candidateCanavas.Background = Brushes.White;
            for (int i = 0; i < 9; i++)
            {
                Border border = new Border();
                border.Width = border.Height = 18;
                border.BorderBrush = null;

                Thickness margin = border.Margin;
                if (i % 3 == 0) margin.Left = 3;
                else margin.Left = 3 + (i % 3) * 18;
                if (Math.Floor(i / 3.0) == 0) margin.Top = 3;
                else margin.Top = 3 + Math.Floor(i / 3.0) * 18;
                border.Margin = margin;

                TextBlock number = new TextBlock();
                number.Text = Convert.ToString(i + 1);
                number.Foreground = Brushes.Black;
                number.FontFamily = new FontFamily("Freestyle Script Regular");
                number.FontSize = 20;
                number.HorizontalAlignment = HorizontalAlignment.Center;
                number.VerticalAlignment = VerticalAlignment.Center;
                number.Padding = new Thickness(0);

                border.Child = number;
                candidateCanavas.Children.Add(border);
            }
            this.Children.Add(candidateCanavas);
        }

        private void BuildResultView()
        {
            resultCanavas = new Canvas();
            resultCanavas.Width = 60;
            resultCanavas.Height = 60;
            resultCanavas.Background = Brushes.White;

            Border border = new Border();
            border.Width = border.Height = 60;
            border.BorderBrush = null;

            TextBlock number = new TextBlock();
            number.Text = Convert.ToString(0);
            number.Foreground = Brushes.Black;
            number.FontFamily = new FontFamily("Freestyle Script Regular");
            number.FontSize = 30;
            number.HorizontalAlignment = HorizontalAlignment.Center;
            number.VerticalAlignment = VerticalAlignment.Center;
            number.Padding = new Thickness(0);
            
            border.Child = number;
            resultCanavas.Children.Add(border);
            this.Children.Add(resultCanavas);
        }

        public void SetCellBackgroundColor(Brush brush)
        {
            candidateCanavas.Background = resultCanavas.Background = brush;
        }

        private void SwitchViewToCandidates()
        {
            if (candidateCanavas.Visibility == Visibility.Hidden)
            {
                candidateCanavas.Visibility = Visibility.Visible;
                resultCanavas.Visibility = Visibility.Hidden;
            }
        }

        private void SwitchViewToResult()
        {
            if (resultCanavas.Visibility == Visibility.Hidden)
            {
                resultCanavas.Visibility = Visibility.Visible;
                candidateCanavas.Visibility = Visibility.Hidden;
            }
        }

        public void Refresh()
        {
            if (cell.Value > 0) SwitchViewToResult();
            else SwitchViewToCandidates();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double sumX = 0.0;
            double maxY = 0.0;
            foreach (UIElement child in this.Children)
            {
                child.Measure(new Size(Math.Max(availableSize.Width - sumX, 0.0), availableSize.Height));
                sumX += child.DesiredSize.Width;
                maxY = Math.Max(maxY, child.DesiredSize.Height);
            }
            return new Size(sumX, maxY);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0.0;
            for (int i = 0; i < this.Children.Count - 1; i++)
            {
                UIElement child = this.Children[i];
                child.Arrange(new Rect(x, 0.0, child.DesiredSize.Width, child.DesiredSize.Height));
                x += child.DesiredSize.Width;
            }
            if (this.Children.Count > 0)
            {
                UIElement lastChild = this.Children[this.Children.Count - 1];
                lastChild.Arrange(new Rect(x, 0.0, Math.Max(finalSize.Width - x, 0.0), lastChild.DesiredSize.Height));
            }
            return finalSize;
        }
    }
}
