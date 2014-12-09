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
        private int nextId;
        private Table table;

        public SolverController(Table t)
        {
            this.table = t;
            InitializeHeuristics();
            nextId = 0;
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
        }

        public void TakeOneStep()
        {
            if (heuristics.ElementAt(NextId).Enabled)
            {
                if (nextId < heuristics.Count() - 1)
                {
                    if (heuristics.ElementAt(NextId++).Apply())
                    {
                        nextId = 0;
                    }
                }
                else nextId = 0;
            }
            else
            {
                nextId++;
                TakeOneStep();
            }
           
        }

        public void AutoSolve()
        {
            foreach (var heuristic in heuristics)
            {
                nextId = heuristics.IndexOf(heuristic) + 1;
                if (heuristic.Apply())
                {
                    nextId = 0;
                    AutoSolve();
                }
            }
        }

        public int NextId
        {
            get { return nextId; }
            set { nextId = value; }
        }

        public List<Heuristic> Heuristics
        {
            get { return heuristics; }
        }
    }
}
