using ConsoleApp.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Controller
{
    class TableController
    {
        Table table;

        public TableController()
        {
            InitializeTable();
        }

        private void InitializeTable()
        {
            table = new Table();
            for (int y = 0; y < 9; y++)
            {
                List<Cell> temp = new List<Cell>();
                Box tempbox = new Box();
                for (int x = 0; x < 9; x++)
                {
                    temp.Add(new Cell(x, y, (int)(x * y) % 9 + 1, tempbox));
                }
                table.Cells.Add(temp);
            }
        }

        public bool CheckTable()
        {
            bool valid = false;
            // Cell by cell
            foreach (var row in table.Cells)
            {
                foreach (var cell in row)
                {
                    valid = CheckCellValidity(cell);
                }
            }
            return valid;
        }

        public bool CheckCellValidity(Cell cell)
        {
            int count = 0;

            // Row
            foreach (var c in GetRowOfCell(cell))
            {
                if (c.Value == cell.Value) count++;
                if (count > 1) return false;
            }

            // Column
            count = 0;
            foreach (var c in GetColumnOfCell(cell))
            {
                if (c.Value == cell.Value) count++;
                if (count > 1) return false;
            }

            // Box
            count = 0;
            foreach (var c in cell.Box.Cells)
            {
                if (c.Value == cell.Value) count++;
                if (count > 1) return false;
            }

            return true;
        }

        public void MakeCandidatesForCell(Cell cell)
        {
            for (int value = 1; value <= 9; value++)
            {
                cell.Value = value; // testing
                if (CheckCellValidity(cell)) cell.Candidates.Add(value);
            }

            if (cell.Candidates.Count() == 1)
            {
                cell.Value = cell.Candidates[0];
                cell.Candidates.Sort();
            }
            else
            {
                cell.Value = 0;
            }
        }

        public void MakeCandidatesForTableCells()
        {
            foreach (var row in table.Cells)
            {
                foreach (var cell in row)
                {
                    MakeCandidatesForCell(cell);
                }
            }
        }

        public void WriteToConsole()
        {
            foreach (var list in table.Cells)
            {
                Console.WriteLine(" ------------------------------------- ");
                foreach (var cell in list)
                {
                    Console.Write(" | " + cell.Value);
                    if (cell.X == 8) Console.Write(" | ");
                }
                Console.WriteLine();
            }
            Console.WriteLine(" ------------------------------------- ");
            Console.WriteLine();
        }


        /**
         * Utility methods
         */
        private IEnumerable<Cell> GetRowOfCell(Cell cell)
        {
            return GetRowByIndex(cell.Y);
        }

        private IEnumerable<Cell> GetColumnOfCell(Cell cell)
        {
            return GetColumnByIndex(cell.X);
        }

        private IEnumerable<Cell> GetColumnByIndex(int column)
        {
            return table.Cells.Select(innerList => innerList[column]);
        }

        private IEnumerable<Cell> GetRowByIndex(int row)
        {
            return table.Cells.ElementAt(row);
        }
    }
}
