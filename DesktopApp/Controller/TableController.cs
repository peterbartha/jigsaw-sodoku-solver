using DesktopApp.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Controller
{
    class TableController
    {
        Table table;
        int[,] output;
        List<Box> boxes;

        public TableController()
        {
            InitializeTable();
        }

        private void InitializeTable()
        {
            output = new int[27, 27];
            table = new Table();
            boxes = new List<Box>();      

            for (int y = 0; y < 9; y++)
            {
                List<Cell> row = new List<Cell>();
                Box tempBox = new Box();
                for (int x = 0; x < 9; x++)
                {
                    Cell cell = new Cell(x, y, (int)((x + 1) * (y + 1)) % 9);
                    row.Add(cell);                     
                    tempBox.Cells.Add(cell);
                }

                foreach (var cell in row)
                {
                    cell.Box = tempBox;
                }

                table.Cells.Add(row);
                boxes.Add(tempBox);
            }
            MakeCandidatesForTableCells();

            CreateOutput();
            Iteraction();
            CreateOutput();
        }

        public void Iteraction()
        {
            List<Cell> refreshNeeded = new List<Cell>();
            //Simulate BasicStep
            table.Cells[3][4].Value = 8;
            refreshNeeded.Add(table.Cells[3][4]);
             
            Heuristic_BasicStep(refreshNeeded);
            Heuristic_NakedSingle();
            Heuristic_HiddenSingle();
            Heuristic_NakedPair();
            Heuristic_HiddenPair();
            Heuristic_PointingPair();
            Heuristic_BoxLineReduction();
            refreshNeeded.Clear();
        }

        public void Heuristic_Inner_BasicStep(List<Cell> cells, Cell source)
        {
            foreach (var cell in cells)
            {
                //TODO: mi van ha benne sincs?
                cell.Candidates.Remove(source.Value);
                cell.Candidates.Sort();
            }
        }

        public void Heuristic_BasicStep(List<Cell> doRefresh)
        {
            foreach (var cell in doRefresh)
            {
                Heuristic_Inner_BasicStep(GetColumnByIndex(cell.Y).ToList(), cell);
                Heuristic_Inner_BasicStep(GetRowByIndex(cell.X).ToList(), cell);
                Heuristic_Inner_BasicStep(cell.Box.Cells, cell);
            }
        }

        public void Heuristic_NakedSingle()
        {
            foreach (var column in table.Cells)
            {
                foreach (var cell in column)
                {
                    if (cell.Candidates.Count == 1)
                    {
                        List<Cell> doBasic = new List<Cell>();
                        doBasic.Add(cell);
                        cell.Value = cell.Candidates[0];
                        cell.Candidates.Clear();
                        Heuristic_BasicStep(doBasic);
                    }
                }
            }
        }

        public void Heuristic_Inner_HiddenSingle(List<Cell> cells)
        {
            List<Cell> onlyOne_Cell = new List<Cell>();
            for (int i = 0; i < 9; i++) onlyOne_Cell.Add(new Cell());
            List<int> onlyOne_Int = new List<int>();
            List<int> occurred = new List<int>();

            foreach (var cell in cells)
            {
                if (cell.Value == 0)
                {
                    foreach (var cand in cell.Candidates)
                    {
                        if (!occurred.Contains(cand))
                        {
                            onlyOne_Cell[cand - 1] = cell;
                            occurred.Add(cand);
                            onlyOne_Int.Add(cand);
                        }
                        else
                        {
                            //TODO: mi van ha benne sincs?
                            onlyOne_Int.Remove(cand);
                        }
                    }
                } 
            }

            foreach (var cand in onlyOne_Int)
            {
                Cell fresh = onlyOne_Cell[cand - 1];
                table.Cells[fresh.X][fresh.Y].Candidates.Clear();
                table.Cells[fresh.X][fresh.Y].Candidates.Add(cand);
                table.Cells[fresh.X][fresh.Y].Candidates.Sort();
            }
        }

        public void Heuristic_HiddenSingle()
        {
            for (int num = 0; num < 9; num++)
            {
                Heuristic_Inner_HiddenSingle(GetColumnByIndex(num).ToList());
                Heuristic_Inner_HiddenSingle(GetRowByIndex(num).ToList());
            }

            foreach (var box in boxes)
            {
                Heuristic_Inner_HiddenSingle(box.Cells);
            }
        }

        public void Heuristic_Inner_NakedPair(List<Cell> cells)
        {
            int[,] pairs = new int[5, 2];
            List<int> goodNums = new List<int>();
            bool inPairs = false;

            foreach (var cell in cells)
            {
                if (cell.Candidates.Count == 2)
                {
                    for (int i = 0; i < pairs.Length; i++)
                    {
                        if ((pairs[i, 0] == cell.Candidates.First()) && (pairs[i, 1] == cell.Candidates.Last()))
                        {
                            goodNums.Add(cell.Candidates.First());
                            goodNums.Add(cell.Candidates.Last());
                            inPairs = true;
                        }
                    }
                    if (!inPairs)
                    {
                        int length = pairs.Length;
                        pairs[length, 0] = cell.Candidates.First();
                        pairs[length, 1] = cell.Candidates.Last();
                    }
                }
            }

            if (goodNums.Count > 0)
            {
                foreach (var cell in cells)
                {
                    if (cell.Candidates.Count != 2)
                    {
                        foreach (var good in goodNums)
                        {
                            foreach (var cand in cell.Candidates)
                            {
                                if (cand == good)
                                {
                                    cell.Candidates.Remove(cand);
                                    cell.Candidates.Sort();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Heuristic_NakedPair()
        {
            for (int i = 0; i < 9; i++)
            {
                Heuristic_Inner_NakedPair(GetColumnByIndex(i).ToList());
                Heuristic_Inner_NakedPair(GetRowByIndex(i).ToList());
            }
            foreach (var box in boxes)
            {
                Heuristic_Inner_NakedPair(box.Cells);
            }
        }

        public void Heuristic_Inner_HiddenPair(List<Cell> cells, int a, int b)
        {
            int count_A = 0;
            int count_B = 0;
            List<Cell> goodCells = new List<Cell>();

            foreach (var cell in cells)
            {
                if (cell.Candidates.Contains(a)) count_A++;
                if (cell.Candidates.Contains(b)) count_B++;

                if (cell.Candidates.Contains(a) && cell.Candidates.Contains(b))
                {
                    goodCells.Add(cell);                       
                }               
            }

            if((goodCells.Count == 2) && (count_A == 2) && (count_B == 2))
            {
                foreach (var goodCell in goodCells)
                {
                    goodCell.Candidates.Clear();
                    goodCell.Candidates.Add(a);
                    goodCell.Candidates.Add(b);
                    goodCell.Candidates.Sort();
                }
            }
        }

        public void Heuristic_HiddenPair()
        {
            foreach (var column in table.Cells)
            {
                foreach (var cell in column)
                {
                    if (cell.Value == 0)
                    {
                        foreach (var cand in cell.Candidates)
                        {
                            foreach (var candnew in cell.Candidates)
                            {
                                if (cand < candnew)
                                {
                                    Heuristic_Inner_HiddenPair(GetColumnByIndex(cell.Y).ToList(), cand, candnew);
                                    Heuristic_Inner_HiddenPair(GetRowByIndex(cell.X).ToList(), cand, candnew);
                                    Heuristic_Inner_HiddenPair(cell.Box.Cells, cand, candnew);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Heuristic_Inner_PointingPair(List<Cell> cells, int a, Box except)
        {
            foreach (var cell in cells)
            {
                if (!cell.Box.Equals(except))
                {
                    cell.Candidates.Remove(a);
                    cell.Candidates.Sort();
                }
            }
        }

        public void Heuristic_PointingPair()
        {
            foreach (var box in boxes)
            {
                foreach (var cell in box.Cells)
                {
                    int[] cand_Place_Column = new int[9];
                    int[] cand_Place_Row = new int[9];
                    int[] cand_Count_Column = new int[9];
                    int[] cand_Count_Row = new int[9];

                    foreach (var cand in cell.Candidates)
                    {
                        cand_Place_Column[cand - 1] = cell.Y;
                        cand_Place_Row[cand - 1] = cell.X;
                        bool match_Column = true;
                        bool match_Row = true;

                        foreach (var nd_Cell in box.Cells)
                        {
                            if (nd_Cell.Y != cand_Place_Column[cand - 1])
                            {
                                match_Column = false;
                            }
                            else
                            {
                                cand_Count_Column[cand - 1]++;
                            }
                            if (nd_Cell.X != cand_Place_Row[cand - 1])
                            {
                                match_Row = false;
                            }
                            else
                            {
                                cand_Count_Row[cand - 1]++;
                            }
                        }
                        if (match_Column && (cand_Count_Column[cand - 1] > 1))
                        {
                            Heuristic_Inner_PointingPair(GetColumnOfCell(cell).ToList(), cand, box);
                        }
                        if (match_Row && (cand_Count_Row[cand - 1] > 1))
                        {
                            Heuristic_Inner_PointingPair(GetRowOfCell(cell).ToList(), cand, box);
                        }
                    }                  
                }
            }
        }

        public void Heuristic_Inner_Core_BoxLineReduction(List<Cell> cells, int a, List<Cell> expect)
        {
            foreach (var cell in cells)
            {
                if (!expect.Contains(cell))
                {
                    cell.Candidates.Remove(a);
                    cell.Candidates.Sort();
                }
            }
        }

        public void Heuristic_Inner_Shell_BoxLineReduction(List<Cell> cells)
        {
            foreach (var cell in cells)
            {
                Box[] cand_Place_Box = new Box[9];
                int[] cand_Count = new int[9];

                foreach (var cand in cell.Candidates)
                {
                    cand_Place_Box[cand - 1] = cell.Box;
                    bool match_Box = true;

                    foreach (var nd_Cell in cells)
                    {
                        if (!nd_Cell.Box.Equals(cand_Place_Box[cand - 1]))
                        {
                            match_Box = false;
                        }
                        else
                        {
                            cand_Count[cand - 1]++;
                        }
                    }
                    if (match_Box && (cand_Count[cand - 1] > 1))
                    {
                        Heuristic_Inner_Core_BoxLineReduction(cell.Box.Cells, cand, cells);
                    }
                }
            }
        }

        public void Heuristic_BoxLineReduction()
        {
            for (int i = 0; i < 9; i++)
            {
                Heuristic_Inner_Shell_BoxLineReduction(GetColumnByIndex(i).ToList());
                Heuristic_Inner_Shell_BoxLineReduction(GetRowByIndex(i).ToList());
            }
        }
       
        /* Fejlesztes alatt!!!
        public void Heuristic_XWing(){
            List<List<int>> only2 = new List<List<int>>();
            
            for (int row = 0; row < 9; row++)
            {
                List<int> only2_Row = new List<int>();
                int[] cand_Count_Row = new int[9];

                foreach (var cell in GetRowByIndex(row).ToList())
                {
                    foreach (var cand in cell.Candidates)
                    {
                        cand_Count_Row[cand - 1]++;
                    }
                }
                foreach (var cand in cand_Count_Row)
                {
                    if (cand_Count_Row[cand - 1] == 2)
                    {
                        only2_Row.Add(cand);
                    }
                }
                only2.Add(only2_Row);
            }
        }*/

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

        public void CreateOutput()
        {
            int x = 0;
            int y = 0;

            foreach (var celllist in table.Cells)
            {                
                foreach (var cell in celllist)
                {    
                    for (int c = 0; c < 9; c++)
                    {
                        int innerrow = c / 3;
                        int innercolumn = c % 3;

                        if (cell.Candidates.Contains(c+1))
                            output[innerrow + (x * 3), innercolumn + (y * 3)] = c+1;
                        else
                            output[innerrow + (x * 3), innercolumn + (y * 3)] = 0;
                    }
                    x++;
                }
                y++;
                x = 0;
            }
        } 


        public void WriteToConsole()
        {
            for (int column = 0; column < 27; column++)
            {
                if ((column) % 9 == 0) Console.WriteLine("-------------------------------------------------------------------");
                else
                    if ((column) % 3 == 0) Console.WriteLine();

                for (int row = 0; row < 27; row++)
                {
                    if (row == 0) Console.Write("|");
                    Console.Write(" " + output[column, row]);
                    if (row == 26) Console.Write(" |");
                    else
                        if ((row + 1) % 9 == 0) Console.Write(" |");
                        else
                            if (((row + 1) % 3 == 0) || (row == 26)) Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("-------------------------------------------------------------------");
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
