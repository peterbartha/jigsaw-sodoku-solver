using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Controls;

namespace DesktopApp.Controller
{
    class StaticsController
    {
        private MainWindow mainWindow;
        private Timer timer;
        private int steps;
        private long elapsedMilis;
        private Boolean cheating;
        private Boolean showedCandidates;
        private Boolean timerEnabled;
        
        

        public StaticsController(MainWindow mw)
        {
            mainWindow = mw;
            cheating = showedCandidates = false;
            timerEnabled = true;
            steps = 0;
            elapsedMilis = 0;
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += TimerOnTick;
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            elapsedMilis += 1000;
            var timespan = TimeSpan.FromMilliseconds(elapsedMilis);

            string formatedTime = string.Format("{0:D2} : {1:D2} : {2:D2}", timespan.Hours, timespan.Minutes, timespan.Seconds);

            mainWindow.UpdateTimerLabelContent(formatedTime);
        }


        /**
         * Getter/Setter
         **/
        public Timer Timer
        {
            get { return timer; }
        }

        public int Steps
        {
            get { return steps; }
            set {
                steps = value;
                mainWindow.UpdateStepsCounterContent(value);
            }
        }

        public Boolean Cheating
        {
            get { return cheating; }
            set { 
                cheating = value;
                if (value)
                {
                    timer.Stop();
                    mainWindow.UpdateTimerLabelContent("00 : 00 : 00");
                }
            }
        }

        public Boolean ShowedCandidates
        {
            get { return showedCandidates; }
            set { showedCandidates = value; }
        }

        public Boolean TimerEnabled
        {
            get { return timerEnabled; }
            set { timerEnabled = value; }
        }
    }
}
