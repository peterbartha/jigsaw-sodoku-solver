using DesktopApp.Controller;
using DesktopApp.Databases;
using DesktopApp.MapEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        SolverController solver;
        public enum GameState { Ended, InGame, Fault }
        int statePointerTop;

        public MainWindow()
        {
            InitializeComponent();
            GenerateNewMap(0);

            TheEnd.Visibility = Visibility.Hidden;
            statePointerTop = 349;
            Canvas.SetTop(StepPointer, statePointerTop);
            
            logoPanel.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseDown);
            logoImg.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseDown);
            titleBar.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseDown);

            exitIcon.MouseLeftButtonDown += new MouseButtonEventHandler(Window_Exit);
            minimalIcon.MouseLeftButtonDown += new MouseButtonEventHandler(Window_Minimal);
        }

        private void GenerateNewMap(int mapIndex)
        {
            tableCtrl = new TableController(this);
            mapCtrl = new MapController(tableCtrl, mapIndex);
            solver = new SolverController(tableCtrl.Table, this);
            tableCtrl.ShowCandidates = true;
        }

        private void Window_Exit(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Window_Minimal(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /**
         * Move window
         */
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_TakeOneStep(object sender, RoutedEventArgs e)
        {
            solver.TakeOneStep();
            Canvas.SetTop(StepPointer, statePointerTop + 30 * solver.Actual);
        }

        private void ChkNakedPair_Checked(object sender, RoutedEventArgs e)
        {
            if  (solver != null)
                solver.Heuristics.ElementAt(2).Enabled = (Boolean)ChkNakedPair.IsChecked;
        }

        private void ChkHiddenPair_Checked(object sender, RoutedEventArgs e)
        {
            if (solver != null)
                solver.Heuristics.ElementAt(3).Enabled = (Boolean)ChkHiddenPair.IsChecked;
        }

        private void ChkPointingPair_Checked(object sender, RoutedEventArgs e)
        {
            if (solver != null)
                solver.Heuristics.ElementAt(4).Enabled = (Boolean)ChkPointingPair.IsChecked;
        }

        private void ChkBoxLineReduuction_Checked(object sender, RoutedEventArgs e)
        {
            if (solver != null)
                solver.Heuristics.ElementAt(5).Enabled = (Boolean)ChkBoxLineReduuction.IsChecked;
        }

        private void ChkXWing_Checked(object sender, RoutedEventArgs e)
        {
            if (solver != null)
                solver.Heuristics.ElementAt(6).Enabled = (Boolean)ChkXWing.IsChecked;
        }
        private void ChkRandomPick_Checked(object sender, RoutedEventArgs e)
        {
            if (solver != null)
                solver.Heuristics.ElementAt(7).Enabled = (Boolean)ChkRandomPick.IsChecked;
        }



        private void AutoSolve_Click(object sender, RoutedEventArgs e)
        {
            if (solver != null)
                solver.AutoSolve();
        }


        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            for (int i = 0; i < mapCtrl.mapData.GetMapCount(); i++)
                data.Add((i+1).ToString());

            var comboBox = sender as ComboBox;
            comboBox.ItemsSource = data;
            comboBox.SelectedIndex = 0;

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            string value = comboBox.SelectedItem as string;
            GenerateNewMap(Convert.ToInt32(value) - 1);
        }

        private void ComboBox_Type_Loaded(object sender, RoutedEventArgs e)
        {
            List<Enum> mapData = new List<Enum>();
            mapData.Add(MapEnums.Easy);
            mapData.Add(MapEnums.Medium);
            mapData.Add(MapEnums.Hard);
            mapData.Add(MapEnums.Expert);
            
            var comboBox = sender as ComboBox;
            comboBox.ItemsSource = mapData;
            comboBox.SelectedIndex = 0;

        }

        private void ComboBox_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            Enum value = comboBox.SelectedItem as Enum;
            if (value.Equals(MapEnums.Easy)) mapCtrl.LoadNewLevel(MapEnums.Easy);
            if (value.Equals(MapEnums.Medium)) mapCtrl.LoadNewLevel(MapEnums.Medium);
            if (value.Equals(MapEnums.Hard)) mapCtrl.LoadNewLevel(MapEnums.Hard);
            if (value.Equals(MapEnums.Expert)) mapCtrl.LoadNewLevel(MapEnums.Expert);
           // mapCtrl.LoadNewLevel((MapEnums)value);

            //ComboBox_Loaded(sender, e);
        }

        private void CheckBox_ShowCandidates(object sender, RoutedEventArgs e)
        {
            if (tableCtrl != null)
                tableCtrl.ShowCandidates = (Boolean)ChkShowCandidates.IsChecked;
        }

        public GameState CheckGameState()
        {
            int empty = 0;
            int fault = 0;

            foreach (var row in tableCtrl.Table.Cells)
            {
                foreach (var cell in row)
                {
                    if (cell.Value == 0 && cell.Candidates.Count() == 0)
                    {
                        fault++;
                    }
                    else if (cell.Value == 0)
                    {
                        empty++;
                    }
                }
            }

            if (fault > 0)
            {
                ShowErrorPanel();
                return GameState.Fault;
            }
            else if (empty > 0) return GameState.InGame;

            ShowSuccessPanel();
            return GameState.Ended;
        }

        private void ShowErrorPanel()
        {
            teTitle.Content = "Error";
            TheEnd.Visibility = Visibility.Visible;
        }

        private void ShowSuccessPanel()
        {
            teTitle.Content = "Success";
            TheEnd.Visibility = Visibility.Visible;
        }

        private void hideTheEnd_Click(object sender, RoutedEventArgs e)
        {
            TheEnd.Visibility = Visibility.Hidden;
        }

    }
}
