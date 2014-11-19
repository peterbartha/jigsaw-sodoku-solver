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
        private Panel candidatePanel;

        public CellPanel()
        {
            InitializeCellPanel();
            BuildCandidateView();
        }

        private void InitializeCellPanel() {
            this.Height = 60;
            this.Width = 60;
            this.Background = Brushes.White;
        }

        private void BuildCandidateView()
        {
            candidatePanel = new StackPanel();
            for (int i = 0; i < 9; i++)
            {
                Label temp = new Label();
                temp.Content = i + 1;
                temp.Width = 10;
                temp.Height = 10;
                temp.Foreground = Brushes.Black;
                temp.Background = Brushes.White;


                Thickness margin = temp.Margin;
                margin.Left = (i % 3) * 15;
                margin.Top = Math.Floor(i / 3.0) * 15;
                temp.Margin = margin;

                candidatePanel.Children.Add(temp);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(60, 60);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return new Size(60, 60);
        }
    }
}
