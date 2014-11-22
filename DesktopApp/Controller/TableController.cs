using DesktopApp.Structure;
using DesktopApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DesktopApp.Controller
{
    class TableController
    {
        MainWindow window;
        Table table;
        int[,] output;
        List<Box> boxes;
        List<List<CellPanel>> cellPanels;

        public TableController(MainWindow mainWindow)
        {
            window = mainWindow;
            InitializeTable();
        }

        private void InitializeTable()
        {
            cellPanels = new List<List<CellPanel>>();
            output = new int[27, 27];
            table = new Table();
            boxes = new List<Box>();
        }

        public void RenderTable()
        {
            MakeCandidatesForTableCells();
            Iteraction();
            CreateCellPanels();
            RenderCellPanels();
        }

        public void CreateCellPanels()
        {
            foreach (var row in table.Cells)
            {
                List<CellPanel> cellPanelRow = new List<CellPanel>();
                foreach (var cell in row)
                {
                    CellPanel cellPanel = new CellPanel(cell, this);
                    cellPanelRow.Add(cellPanel);
                }
                cellPanels.Add(cellPanelRow);
            }
        }

        public void RenderCellPanels()
        {
            ClearTable();
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    CellPanel cellPanel = cellPanels.ElementAt(y).ElementAt(x);
                    Grid.SetColumn(cellPanel, x);
                    Grid.SetRow(cellPanel, y);
                    window.CellGrid.Children.Add(cellPanel);
                }
            }
        }

        public void RefreshCellPanels()
        {
            foreach (var cellPanelRow in cellPanels)
            {
                foreach (var cellPanel in cellPanelRow)
                {
                    cellPanel.Refresh();
                }
            }
        }

        private void ClearTable()
        {
            if (window.CellGrid.Children.Count > 0)
                window.CellGrid.Children.Clear();
        }

        public void Iteraction()
        {
            for (int i = 0; i < 1; i++)
            {
                //Heuristic_NakedSingle();
                /* Heuristic_HiddenSingle();
                 Heuristic_NakedPair();
                 Heuristic_HiddenPair();
                 Heuristic_PointingPair();
                 Heuristic_BoxLineReduction();
                 Heuristic_XWing();
                 refreshNeeded.Clear();*/
                Heuristic_BasicStep();
            } 

        }

        public void Heuristic_Inner_BasicStep(List<Cell> cells, int cand)
        {
            foreach (var cell in cells)
                if (cell.Value != cand)
                {
                    cell.Remove_Secure(cand);
                    cell.Candidates.Sort();
                }
        }

        public void Heuristic_BasicStep()
        {
            foreach (var row in table.Cells)
                foreach (var cell in row)
                    if (cell.Value > 0)
                    {
                        Heuristic_Inner_BasicStep(GetColumnByIndex(cell.Y).ToList(), cell.Value);
                        Heuristic_Inner_BasicStep(GetRowByIndex(cell.X).ToList(), cell.Value);
                        Heuristic_Inner_BasicStep(cell.Box.Cells, cell.Value);
                    }
        }

        public void Heuristic_NakedSingle()
        {
            foreach (var column in table.Cells)
                foreach (var cell in column)
                    if ((cell.Candidates.Count == 1) && (cell.Value == 0))
                    {
                        cell.Value = cell.Candidates.First();
                        //cell.Candidates.Clear();
                    }
            Heuristic_BasicStep();
        }

        public void Heuristic_Inner_HiddenSingle(List<Cell> cells)
        {
            List<Cell> onlyOne_Cell = new List<Cell>();
            for (int i = 0; i < 9; i++) onlyOne_Cell.Add(new Cell());
            List<int> onlyOne_Int = new List<int>();
            List<int> occurred = new List<int>();

            foreach (var cell in cells)
                if (cell.Value == 0)
                    foreach (var cand in cell.Candidates)
                    {
                        if (!occurred.Contains(cand))
                        {
                            onlyOne_Cell[cand - 1] = cell;
                            occurred.Add(cand);
                            onlyOne_Int.Add(cand);
                        }
                        else if(onlyOne_Int.Contains(cand)) onlyOne_Int.Remove(cand);
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
                Heuristic_Inner_HiddenSingle(box.Cells);
        }

        public void Heuristic_Inner_NakedPair(List<Cell> cells)
        {
            int[,] pairs = new int[5, 2];
            List<int> goodNums = new List<int>();
            
            foreach (var cell in cells)
            {
                bool inPairs = false;
                if (cell.Candidates.Count == 2)
                {
                    for (int i = 0; i < pairs.Length; i++)
                        if ((pairs[i, 0] == cell.Candidates.First()) && (pairs[i, 1] == cell.Candidates.Last()))
                        {
                            goodNums.Add(cell.Candidates.First());
                            goodNums.Add(cell.Candidates.Last());
                            inPairs = true;
                        }

                    if (!inPairs)
                    {
                        pairs[pairs.Length, 0] = cell.Candidates.First();
                        pairs[pairs.Length, 1] = cell.Candidates.Last();
                    }
                }
            }

            if (goodNums.Count > 0)
                foreach (var cell in cells)
                    if (cell.Candidates.Count != 2)
                        foreach (var good in goodNums)
                            foreach (var cand in cell.Candidates)
                                if (cand == good)
                                {
                                    cell.Remove_Secure(cand);
                                    cell.Candidates.Sort();
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
                if (cell.Value == 0)
                {
                    if (cell.Candidates.Contains(a)) count_A++;
                    if (cell.Candidates.Contains(b)) count_B++;

                    if (cell.Candidates.Contains(a) && cell.Candidates.Contains(b))
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
                foreach (var cell in column)
                    if (cell.Value == 0)
                        foreach (var cand in cell.Candidates)
                            foreach (var candnew in cell.Candidates)
                                if (cand < candnew)
                                {
                                    Heuristic_Inner_HiddenPair(GetColumnByIndex(cell.Y).ToList(), cand, candnew);
                                    Heuristic_Inner_HiddenPair(GetRowByIndex(cell.X).ToList(), cand, candnew);
                                    Heuristic_Inner_HiddenPair(cell.Box.Cells, cand, candnew);
                                }
        }

        public void Heuristic_Inner_PointingPair(List<Cell> cells, int a, Box except)
        {
            foreach (var cell in cells)
            {
                if (!cell.Box.Equals(except))
                {
                    cell.Remove_Secure(a);
                    cell.Candidates.Sort();
                }
            }
        }

        public void Heuristic_PointingPair()
        {
            foreach (var box in boxes)
                foreach (var cell in box.Cells)
                    if (cell.Value == 0)
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
                                if (nd_Cell.Value == 0)
                                {
                                    if (nd_Cell.Y != cand_Place_Column[cand - 1]) match_Column = false;
                                    else cand_Count_Column[cand - 1]++;

                                    if (nd_Cell.X != cand_Place_Row[cand - 1]) match_Row = false;
                                    else cand_Count_Row[cand - 1]++;
                                }

                            if (match_Column && (cand_Count_Column[cand - 1] > 1))
                                Heuristic_Inner_PointingPair(GetColumnOfCell(cell).ToList(), cand, box);

                            if (match_Row && (cand_Count_Row[cand - 1] > 1))
                                Heuristic_Inner_PointingPair(GetRowOfCell(cell).ToList(), cand, box);
                        }     
                    }
        }

        public void Heuristic_Inner_Core_BoxLineReduction(List<Cell> cells, int a, List<Cell> expect)
        {
            foreach (var cell in cells)
                if (!expect.Contains(cell) && (cell.Value == 0))
                {
                    cell.Remove_Secure(a);
                    cell.Candidates.Sort();
                }
        }

        public void Heuristic_Inner_Shell_BoxLineReduction(List<Cell> cells)
        {
            foreach (var cell in cells)
                if (cell.Value == 0)
                {
                    Box[] cand_Place_Box = new Box[9];
                    int[] cand_Count = new int[9];

                    foreach (var cand in cell.Candidates)
                    {
                        cand_Place_Box[cand - 1] = cell.Box;
                        bool match_Box = true;

                        foreach (var nd_Cell in cells)
                            if (nd_Cell.Value == 0)
                            {
                                if (!nd_Cell.Box.Equals(cand_Place_Box[cand - 1])) match_Box = false;
                                else cand_Count[cand - 1]++;
                            }

                        if (match_Box && (cand_Count[cand - 1] > 1))
                            Heuristic_Inner_Core_BoxLineReduction(cell.Box.Cells, cand, cells);
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

        public List<List<int>> Heuristic_Inner_Only2_XWing(List<Cell> cells)
        {
            // [x,0] cand count [x,1] first column num [x,2] second column num
            int[,] cand_Data = new int[9,3];
            List<List<int>> only2_In_Row = new List<List<int>>();

            foreach (var cell in cells)
                if (cell.Value == 0)
                    foreach (var cand in cell.Candidates)
                    {
                        if (cand_Data[cand, 0] == 0) cand_Data[cand, 1] = cell.Y;
                        if (cand_Data[cand, 0] == 1) cand_Data[cand, 2] = cell.Y;
                        cand_Data[cand, 0]++;
                    }
                

            for (int i = 0; i < 9; i++)
            {
                List<int> cand_Row = new List<int>();
                if (cand_Data[i,0] == 2)
                {
                    cand_Row.Add(i);
                    for (int j = 1; j < 3; j++) cand_Row.Add(cand_Data[i, j]);
                }
                only2_In_Row.Add(cand_Row);
            }
            return only2_In_Row;
        }

        public int get_Indexed(int pos, List<int> cand_Numbers)
        {
            int i = 0;
            foreach (var number in cand_Numbers)
            {
                if (pos == i)  return number;
                i++;
            }
            return -1;
        }

        public void Heuristic_Inner_Core_XWing(List<Cell> cells, int cand, int expect_1, int expect_2)
        {
            foreach (var cell in cells)
                if ((cell.Value == 0) && (cell.Candidates.Contains(cand)) && (cell.X != expect_1) && (cell.X != expect_2))
                    cell.Remove_Secure(cand);
        }
       
        public void Heuristic_XWing()
        {
            List<List<List<int>>> only2_Rows = new List<List<List<int>>>();
            for (int row = 0; row < 9; row++)
            {   
                // [x,0] cand >>>>>VALUE<<<<<< , [x,1] first column num [x,2] second column num
                only2_Rows.Add(Heuristic_Inner_Only2_XWing(GetRowByIndex(row).ToList()));
            }
            // cand_Data: [x.0] cand count, [x.1] first column, [x.2] second column, [x.3] first cand's row number, [x.4] second cand's row number
            int[,] cand_Data = new int[9, 5];
            List<int> cand_Good = new List<int> ();

            int row_I = 0;
            foreach (var row_Good_Cands in only2_Rows)
            {
                foreach (var cand_Numbers in row_Good_Cands)
                {       // cand value
                    int cand_Value = cand_Numbers.First() - 1;
                    // ha előfordul
                    if (cand_Data[cand_Value, 0] == 0)
                    {   //mentem az oszlopértékeket
                        cand_Data[cand_Value, 1] = get_Indexed(1, cand_Numbers);
                        cand_Data[cand_Value, 2] = get_Indexed(2, cand_Numbers);
                        cand_Data[cand_Value, 3] = row_I;
                    }
                    if (cand_Data[cand_Value, 0] == 1)
                    {
                        //ha egy cand második előforduló sorában jó oszlopban vannak az értékek
                        if ((cand_Data[cand_Value, 1] == get_Indexed(1, cand_Numbers)) && (cand_Data[cand_Value, 2] == get_Indexed(2, cand_Numbers)))
                        {
                            cand_Good.Add(cand_Value);
                            cand_Data[cand_Value, 4] = row_I;
                        }
                    }
                    if ((cand_Data[cand_Value, 0] > 1) && (cand_Good.Contains(cand_Value))) cand_Good.Remove(cand_Value);

                    cand_Data[cand_Value, 0]++;
                }  
                row_I++;
            }

            for (int i = 0; i < 9; i++)
            {   // cand_Data: [x.0] cand count, [x.1] first column, [x.2] second column, [x.3] first cand's row number, [x.4] second cand's row number
                if (cand_Good.Contains(i + 1))
                {
                    Heuristic_Inner_Core_XWing(GetColumnByIndex(cand_Data[i, 1]).ToList(), i + 1, cand_Data[i, 3], cand_Data[i, 4]);
                    Heuristic_Inner_Core_XWing(GetColumnByIndex(cand_Data[i, 2]).ToList(), i + 1, cand_Data[i, 3], cand_Data[i, 4]);
                }
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
            if (cell.Value == 0) return true;
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
            if (cell.Candidates.Count > 0) cell.Candidates.Clear();
            if (cell.Value == 0)
            {
                for (int value = 1; value <= 9; value++)
                {
                    cell.Value = value; // testing
                    if (CheckCellValidity(cell)) cell.Candidates.Add(value);
                }

                if (cell.Candidates.Count() == 1) cell.Value = cell.Candidates[0];
                else cell.Value = 0;
                cell.Candidates.Sort();
                if (cellPanels.Count > 0) cellPanels.ElementAt(cell.Y).ElementAt(cell.X).Refresh();
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


        /**
         * Getter/Setter
         */
        public Table Table
        {
            get { return table; }
            set { table = value; }
        }
    }
}
