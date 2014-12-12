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
using System.Threading.Tasks;
using System.Windows.Threading;
using DesktopApp.Structure;

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
        StaticsController stats;
        LoggedInUser user;
        public enum GameState { Ended, InGame, Fault }
        int statePointerTop;

        public MainWindow(LoggedInUser lu)
        {
            InitializeComponent();
            GenerateNewMap();

            user = lu;
            SetUpUserDetails();

            TheEnd.Visibility = Visibility.Hidden;
            statePointerTop = 349;
            Canvas.SetTop(StepPointer, statePointerTop);
            
            logoPanel.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseDown);
            logoImg.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseDown);
            titleBar.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseDown);

            exitIcon.MouseLeftButtonDown += new MouseButtonEventHandler(Window_Exit);
            minimalIcon.MouseLeftButtonDown += new MouseButtonEventHandler(Window_Minimal);
        }

        private void GenerateNewMap()
        {
            stats = new StaticsController(this);
            tableCtrl = new TableController(this, stats);
            mapCtrl = new MapController(tableCtrl, this, stats);
            solver = new SolverController(tableCtrl.Table, this);

            tableCtrl.ShowCandidates = false;
        }

        private void SetUpUserDetails()
        {
            lblUsername.Content = user.Username == "" ? user.Email : user.Username;
        }

        public void SetActualMapName(string name)
        {
            lblActualMap.Content = "Actual map: " + name;
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

        private void Button_TakeOneStep(object sender, RoutedEventArgs e)
        {
            stats.Cheating = true;
            var emptyBefore = CountEmptyCells();
            solver.TakeOneStep();
            var emptyAfter = CountEmptyCells();
            stats.Steps += emptyBefore - emptyAfter;
            Canvas.SetTop(StepPointer, statePointerTop + 30 * solver.Actual);
        }

        private void AutoSolve_Click(object sender, RoutedEventArgs e)
        {
            stats.Cheating = true;
            if (solver != null)
            {
                var emptyBefore = CountEmptyCells();
                solver.AutoSolve();
                var emptyAfter = CountEmptyCells();
                stats.Steps += emptyBefore - emptyAfter;
            }
        }

        private int CountEmptyCells()
        {
            if (tableCtrl == null) return 0;

            int count = 0;
            foreach (var row in tableCtrl.Table.Cells)
            {
                foreach (var cell in row)
                {
                    if (cell.Value == 0) count++;
                }
            }
            return count;
        }

        public void UpdateTimerLabelContent(string content)
        {
            this.Dispatcher.Invoke((Action)(() => { lblTimer.Content = content; }));
        }

        public void UpdateStepsCounterContent(int steps)
        {
            lblSteps.Content = "Steps: " + Convert.ToString(steps);
        }

        private void CheckBox_ShowCandidates(object sender, RoutedEventArgs e)
        {
            if (tableCtrl != null && stats != null)
            {
                tableCtrl.ShowCandidates = (Boolean)ChkShowCandidates.IsChecked;
                stats.ShowedCandidates = (Boolean)ChkShowCandidates.IsChecked ? (Boolean)ChkShowCandidates.IsChecked : stats.ShowedCandidates;
            }
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
            lblDesc.Text = "Wrong solution.\r\nPlease check the numbers.";
            TheEnd.Visibility = Visibility.Visible;
        }

        private void ShowSuccessPanel()
        {
            teTitle.Content = "Success";
            lblDesc.Text = "Steps: " + stats.Steps.ToString() + "\r\nCanidates showed: " + stats.ShowedCandidates.ToString() + "\r\nTime: " + lblTimer.Content + "\r\nPoints: \r\nSolver used: " + stats.Cheating.ToString() + "\r\n";
            TheEnd.Visibility = Visibility.Visible;
        }

        private void hideTheEnd_Click(object sender, RoutedEventArgs e)
        {
            TheEnd.Visibility = Visibility.Hidden;
        }

        private void ChkStopWatch_Checked(object sender, RoutedEventArgs e)
        {
            lblTimer.Visibility = (Boolean)ChkStopWatch.IsChecked ? Visibility.Visible : Visibility.Hidden;
            if (stats.Timer.Enabled) stats.Timer.Stop();
            stats.TimerEnabled = (Boolean)ChkStopWatch.IsChecked;
        }

        private void btnOpenMap_Click(object sender, RoutedEventArgs e)
        {
            var mapWindow = new OpenMap(mapCtrl);
            mapWindow.Show();
        }

    }
}
