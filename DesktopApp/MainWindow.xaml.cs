using DesktopApp.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TableController tableCtrl;
        MapController mapCtrl;

        public MainWindow()
        {
            InitializeComponent();
            tableCtrl = new TableController(this);
            mapCtrl = new MapController(tableCtrl);
        }

    }
}
