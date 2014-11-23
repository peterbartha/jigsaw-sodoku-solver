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
        //List<Box> boxes;
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
            //boxes = new List<Box>();
        }

        public void RenderTable()
        {
            CreateCellPanels();
            RenderCellPanels();
            MakeCandidatesForTableCells();
            //Iteraction();
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
                HeuristicNakedSingle();
                HeuristicHiddenSingle();
                HeuristicNakedPair();
                HeuristicHiddenPair();
                HeuristicPointingPair();
                HeuristicBoxLineReduction();
                //HeuristicXWing();
        }

        public void HeuristicInnerBasicStep(List<Cell> cells, int cand)
        {
            foreach (var cell in cells)
                if (cell.Value != cand)
                {
                    cell.RemoveSecure(cand);
                    cell.Candidates.Sort();
                    cellPanels.ElementAt(cell.Y).ElementAt(cell.X).Refresh();
                }
                else
                {
                    cell.Candidates.Clear();
                    cellPanels.ElementAt(cell.Y).ElementAt(cell.X).Refresh();
                }
        }
        
        /**
         * Erase the new cell value from the same Column, Row and Box
         */
        public void HeuristicBasicStep(Cell cell)
        {
            HeuristicInnerBasicStep(GetColumnByIndex(cell.X).ToList(), cell.Value);
            HeuristicInnerBasicStep(GetRowByIndex(cell.Y).ToList(), cell.Value);
            HeuristicInnerBasicStep(cell.Box.Cells, cell.Value);
        }

        /**
         * If only one candidate exist, then make it for the cell value, and call BasicStep
         */
        public void HeuristicNakedSingle()
        {
            foreach (var column in table.Cells)
                foreach (var cell in column)
                    if ((cell.Candidates.Count == 1) && (cell.Value == 0))
                    {
                        cell.Value = cell.Candidates.First();
                        cellPanels.ElementAt(cell.Y).ElementAt(cell.X).ChangeCellValue_ByHeuristic(cell.Candidates.First());
                        cell.Candidates.Clear();
                        HeuristicBasicStep(cell);
                    }            
        }

        public void HeuristicInnerHiddenSingle(List<Cell> cells)
        {
            List<Cell> onlyOneCell = new List<Cell>();
            for (int i = 0; i < 9; i++) onlyOneCell.Add(new Cell());

            int[] occurred = new int[9];

            foreach (var cell in cells)
                if (cell.Value == 0)
                    foreach (var cand in cell.Candidates)
                    {
                        if (occurred[cand - 1] == 0) onlyOneCell[cand - 1] = cell;
                        occurred[cand - 1]++;
                    }

            for (int i = 0; i < 9; i++)
                if (occurred[i] == 1)
                {
                    int x = onlyOneCell[i].X;
                    int y = onlyOneCell[i].Y;
                    table.Cells[y][x].Candidates.Clear();
                    table.Cells[y][x].Value = i + 1;
                    HeuristicBasicStep(table.Cells[y][x]);
                    cellPanels.ElementAt(y).ElementAt(x).ChangeCellValue_ByHeuristic(i + 1);
                }
        }

        public void HeuristicHiddenSingle()
        {
            for (int num = 0; num < 9; num++)
            {
                HeuristicInnerHiddenSingle(GetColumnByIndex(num).ToList());
                HeuristicInnerHiddenSingle(GetRowByIndex(num).ToList());
            }

            foreach (var box in table.Boxes)
                HeuristicInnerHiddenSingle(box.Cells);
        }

        /**
         * Eliminating candidates
         */

        public void HeuristicInnerNakedPair(List<Cell> cells)
        {
            int[,] pairs = new int[10, 2];
            int pairCount = 0;
            List<Cell> goodCells = new List<Cell>();
            
            foreach (var cell in cells)
            {
                bool inPairs = false;
                if (cell.Candidates.Count == 2)
                {
                    for (int i = 0; i < pairCount; i++)
                        if ((pairs[i, 0] == cell.Candidates.First()) && (pairs[i, 1] == cell.Candidates.Last()))
                        {
                            goodCells.Add(cell);
                            inPairs = true;
                        }

                    if (!inPairs)
                    {
                        pairs[pairCount, 0] = cell.Candidates.First();
                        pairs[pairCount, 1] = cell.Candidates.Last();
                        pairCount++;
                    }
                }
            }

            foreach (var goodCell in goodCells)
                foreach (var cell in cells)
                    if ((cell.Candidates.Count == 2) && 
                        (cell.Candidates.Contains(goodCell.Candidates.First()) &&
                         cell.Candidates.Contains(goodCell.Candidates.Last())))
                    {
                        foreach (var neighCell in cells)
                            if (neighCell.Candidates.Count > 2)
                                foreach (var good in goodCell.Candidates)
                                    if (neighCell.Candidates.Contains(good))
                                    {
                                        neighCell.RemoveSecure(good);
                                        neighCell.Candidates.Sort();
                                        cellPanels.ElementAt(neighCell.Y).ElementAt(neighCell.X).Refresh();
                                    }
                    }
        }

        public void HeuristicNakedPair()
        {
            for (int i = 0; i < 9; i++)
            {
                HeuristicInnerNakedPair(GetColumnByIndex(i).ToList());
                HeuristicInnerNakedPair(GetRowByIndex(i).ToList());
            }
            foreach (var box in table.Boxes)
            {
                HeuristicInnerNakedPair(box.Cells);
            }
                
        }

        public void HeuristicInnerHiddenPair(List<Cell> cells, int a, int b)
        {
            int countA = 0;
            int countB = 0;
            List<Cell> goodCells = new List<Cell>();

            foreach (var cell in cells)
                if (cell.Value == 0)
                {
                    if (cell.Candidates.Contains(a)) countA++;
                    if (cell.Candidates.Contains(b)) countB++;
                    if (cell.Candidates.Contains(a) && cell.Candidates.Contains(b))
                        goodCells.Add(cell);    
                }

            if((goodCells.Count == 2) && (countA == 2) && (countB == 2))
                foreach (var cell in cells)
                    if (goodCells.Contains(cell))
                    {
                        cell.Candidates.Clear();
                        cell.Candidates.Add(a);
                        cell.Candidates.Add(b);
                        cell.Candidates.Sort();
                        cellPanels.ElementAt(cell.Y).ElementAt(cell.X).Refresh();

                    }
        }

        public void HeuristicHiddenPair()
        {
            foreach (var column in table.Cells)
                foreach (var cell in column)
                    if ((cell.Value == 0) && (cell.Candidates.Count > 1))
                        for (int i = 1; i < 9; i++)
                            for (int j = i+1; j < 10; j++)
                                if (cell.Candidates.Contains(i) && cell.Candidates.Contains(j))
                                {
                                    HeuristicInnerHiddenPair(GetColumnByIndex(cell.Y).ToList(), i, j);
                                    HeuristicInnerHiddenPair(GetRowByIndex(cell.X).ToList(), i, j);
                                    HeuristicInnerHiddenPair(cell.Box.Cells, i, j);
                                }
        }

        public void HeuristicInnerPointingPair(List<Cell> cells, int a, Box except)
        {
            foreach (var cell in cells)
                if (!cell.Box.Equals(except))
                {
                    cell.RemoveSecure(a);
                    cell.Candidates.Sort();
                    cellPanels.ElementAt(cell.Y).ElementAt(cell.X).Refresh();
                }
        }

        public void HeuristicPointingPair()
        {
             foreach (var box in table.Boxes)
             {
                 bool[] matchBoolColumn = new bool[9];
                 bool[] matchBoolRow = new bool[9];
                 for (int i = 0; i < 9; i++)
                 {
                     matchBoolColumn[i] = true;
                     matchBoolRow[i] = true;
                 }
                 int[] candPlaceColumn = new int[9];
                 int[] candPlaceRow = new int[9];
                 int[] candCount = new int[9];

                 foreach (var cell in box.Cells)
                     if (cell.Value == 0)
                         foreach (var cand in cell.Candidates)
                         {
                             if (candCount[cand-1] == 0)
                             {
                                 candPlaceColumn[cand - 1] = cell.Y;
                                 candPlaceRow[cand - 1] = cell.X;
                             }
                             else
                             {
                                 if (candPlaceColumn[cand - 1] != cell.Y) matchBoolColumn[cand - 1] = false;
                                 if (candPlaceRow[cand - 1] != cell.X) matchBoolRow[cand - 1] = false;
                             }
                             candCount[cand - 1]++;
                         }

                 for (int i = 0; i < 9; i++)
                     if (candCount[i] < 2)
                     {
                         matchBoolColumn[i] = false;
                         matchBoolRow[i] = false;
                     }

                 for (int i = 0; i < 9; i++)
                 {
                     if (matchBoolColumn[i])
                         HeuristicInnerPointingPair(GetRowByIndex(candPlaceColumn[i]).ToList(), i + 1, box);
                     if (matchBoolRow[i])
                         HeuristicInnerPointingPair(GetColumnByIndex(candPlaceRow[i]).ToList(), i + 1, box);
                 }
             }
        }

        public void HeuristicInnerCoreBoxLineReduction(List<Cell> cells, int a, List<Cell> expect)
        {
            foreach (var cell in cells)
                if (!expect.Contains(cell) && (cell.Value == 0))
                {
                    cell.RemoveSecure(a);
                    cell.Candidates.Sort();
                    cellPanels.ElementAt(cell.Y).ElementAt(cell.X).Refresh();
                }
        }

        public void HeuristicInnerShellBoxLineReduction(List<Cell> cells)
        {
            bool[] matchBool = new bool[9];
            for (int i = 0; i < 9; i++) matchBool[i] = true;
            List<Box> candPlace = new List<Box>();
            for (int i = 0; i < 9; i++)
            {
                Box temp = new Box();
                candPlace.Add(temp);
            }
            int[] candCount = new int[9];

            foreach (var cell in cells)
                if (cell.Value == 0)
                    foreach (var cand in cell.Candidates)
                    {
                        if (candCount[cand - 1] == 0)
                        {
                            candPlace[cand - 1] = cell.Box;
                        }
                        else
                        {
                            if (!candPlace[cand - 1].Equals(cell.Box)) matchBool[cand - 1] = false;
                        }
                        candCount[cand - 1]++;
                    }

            for (int i = 0; i < 9; i++)
                if (candCount[i] < 2) matchBool[i] = false;

            for (int i = 0; i < 9; i++)
                if (matchBool[i])
                    HeuristicInnerCoreBoxLineReduction(candPlace[i].Cells, i + 1, cells);
        }

        public void HeuristicBoxLineReduction()
        {
            for (int i = 0; i < 9; i++)
            {
                HeuristicInnerShellBoxLineReduction(GetColumnByIndex(i).ToList());
                HeuristicInnerShellBoxLineReduction(GetRowByIndex(i).ToList());
            }
        }

        public List<List<int>> HeuristicInnerOnly2XWing(List<Cell> cells)
        {
            // [x,0] cand count [x,1] first column num [x,2] second column num
            int[,] candData = new int[9,3];
            List<List<int>> only2InRow = new List<List<int>>();

            foreach (var cell in cells)
                if (cell.Value == 0)
                    foreach (var cand in cell.Candidates)
                    {
                        if (candData[cand - 1, 0] == 0) candData[cand - 1, 1] = cell.Y;
                        if (candData[cand - 1, 0] == 1) candData[cand - 1, 2] = cell.Y;
                        candData[cand - 1, 0]++;
                    }
                
            for (int i = 0; i < 9; i++)
            {
                List<int> candRow = new List<int>();
                if (candData[i,0] == 2)
                {
                    candRow.Add(i);
                    for (int j = 1; j < 3; j++) candRow.Add(candData[i, j]);
                }
                only2InRow.Add(candRow);
            }
            return only2InRow;
        }

        public void HeuristicInnerCoreXWing(List<Cell> cells, int cand, int expect1, int expect2)
        {
            foreach (var cell in cells)
                if ((cell.Value == 0) && (cell.Candidates.Contains(cand)) && (cell.X != expect1) && (cell.X != expect2))
                {
                    cell.RemoveSecure(cand);
                    cell.Candidates.Sort();
                    cellPanels.ElementAt(cell.Y).ElementAt(cell.X).Refresh();
                }
                    
        }
       
        public void HeuristicXWing()
        {
            List<List<List<int>>> only2Rows = new List<List<List<int>>>();
            for (int row = 0; row < 9; row++)
            {   
                // [x,0] cand >>>>>VALUE<<<<<< , [x,1] first column num [x,2] second column num
                only2Rows.Add(HeuristicInnerOnly2XWing(GetRowByIndex(row).ToList()));
            }
            // cand_Data: [x.0] cand count, [x.1] first column, [x.2] second column, [x.3] first cand's row number, [x.4] second cand's row number
            int[,] candData = new int[9, 5];
            List<int> candGood = new List<int> ();

            int rowI = 0;
            foreach (var rowGoodCands in only2Rows)
            {
                foreach (var candNumbers in rowGoodCands)
                {
                    if (candNumbers.Count > 0)
                    {
                        int candValue = candNumbers.ElementAt(0);
                        // ha (először) előfordul
                        if (candData[candValue, 0] == 0)
                        {   //mentem az oszlop & sor értékeket
                            candData[candValue, 1] = candNumbers.ElementAt(1);
                            candData[candValue, 2] = candNumbers.ElementAt(2);
                            candData[candValue, 3] = rowI;
                        }
                        //ha másodszor is
                        if (candData[candValue, 0] == 1)
                        {
                            //ha jó oszlopban vannak az értékek
                            if ((candData[candValue, 1] == candNumbers.ElementAt(1)) && (candData[candValue, 2] == candNumbers.ElementAt(2)))
                            {
                                candGood.Add(candValue);
                                candData[candValue, 4] = rowI;
                            }
                        }
                        candData[candValue, 0]++;
                    }
                }
                rowI++;
            }

            for (int i = 0; i < 9; i++)
                if ((candData[i, 0] > 1) && (candGood.Contains(i + 1))) candGood.Remove(i + 1);

            for (int i = 0; i < 9; i++)
            {   // cand_Data: [x.0] cand count, [x.1] first column, [x.2] second column, [x.3] first cand's row number, [x.4] second cand's row number
                if (candGood.Contains(i + 1))
                {
                    HeuristicInnerCoreXWing(GetColumnByIndex(candData[i, 1]).ToList(), i + 1, candData[i, 3], candData[i, 4]);
                    HeuristicInnerCoreXWing(GetColumnByIndex(candData[i, 2]).ToList(), i + 1, candData[i, 3], candData[i, 4]);
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
