using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Structure
{
    class Cell
    {
        private Box box;
        private List<int> candidates;
        private int val;
        private int x;
        private int y;

        public Cell() { }

        public Cell(int x, int y, int value, Box box, List<int> candidate)
        {
            X = x;
            Y = y;
            Value = value;
            Box = box;
            candidates = candidate;
        }



        /**
         * Setter / Getters
         */
        public Box Box
        {
            get { return box; }
            set { box = value; }
        }

        public List<int> Candidates
        {
            get { return candidates; }
            set { candidates = value; }
        }


        public int Value
        {
            get { return val; }
            set { val = value; }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        
    }
}
