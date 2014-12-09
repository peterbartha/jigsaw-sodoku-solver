using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopApp.Heuristics;
using DesktopApp.Structure;

namespace DesktopApp.Controller
{
    class SolverController
    {
        private List<Heuristic> heuristics;
        private int actualId;
        private Table table;
        

        public SolverController(Table t)
        {
            this.table = t;
            InitializeHeuristics();
            Actual = 0;
        }

        private void InitializeHeuristics() {
            heuristics = new List<Heuristic>();
            heuristics.Add(new NakedSingle(table));
            heuristics.Add(new HiddenSingle(table));
            heuristics.Add(new NakedPair(table));
            heuristics.Add(new HiddenPair(table));
            heuristics.Add(new PointingPair(table));
            heuristics.Add(new BoxLineReduction(table));
            heuristics.Add(new XWing(table));
            heuristics.Add(new RandomPick(table));
        }

        public void TakeOneStep()
        {
            if (heuristics.ElementAt(Actual).Apply())
            {
                Actual = 0;
                return;
            }

            for (int i = Actual + 1; i < heuristics.Count; i++)
            {
                if (heuristics[i].Enabled)
                {
                    Actual = i;
                    return;
                }
            }
            Actual = 0;
        }

        public void AutoSolve()
        {
            foreach (var heuristic in heuristics)
            {
                Actual = heuristics.IndexOf(heuristic) + 1;
                if (heuristic.Apply())
                {
                    Actual = 0;
                    AutoSolve();
                }
            }

        }

        public int Actual
        {
            get { return actualId; }
            set { actualId = value; }
        }

        public List<Heuristic> Heuristics
        {
            get { return heuristics; }
        }
    }
}
