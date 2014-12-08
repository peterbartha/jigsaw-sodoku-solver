﻿using DesktopApp.Heuristics;
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
        private MainWindow window;
        private Table table;
        private List<List<CellPanel>> cellPanels;
        private Boolean showCandidates;
        

        public TableController(MainWindow mainWindow)
        {
            window = mainWindow;
            InitializeTable();
        }

        private void InitializeTable()
        {
            cellPanels = new List<List<CellPanel>>();
            table = new Table();
        }

        public void RenderTable()
        {
            CreateCellPanels();
            RenderCellPanels();
            MakeCandidatesForTableCells();
        }

        public void CreateCellPanels()
        {
            foreach (var row in table.Cells)
            {
                List<CellPanel> cellPanelRow = new List<CellPanel>();
                foreach (var cell in row)
                {
                    CellPanel cellPanel = new CellPanel(cell, this);
                    cell.Panel = cellPanel;
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
        private void ClearTable()
        {
            if (window.CellGrid.Children.Count > 0)
                window.CellGrid.Children.Clear();
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
            foreach (var c in Utility.GetRowOfCell(cell, table.Cells))
            {
                if (c.Value == cell.Value) count++;
                if (count > 1) return false;
            }

            // Column
            count = 0;
            foreach (var c in Utility.GetColumnOfCell(cell, table.Cells))
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

                if (cell.Candidates.Count() == 1)
                {
                    cell.Panel.ChangeCellValue(cell.Candidates.First());
                }
                else
                {
                    cell.Value = 0;
                    cell.Candidates.Sort();
                }
                if (cellPanels.Count > 0) cell.Panel.Refresh();
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

        /**
         * Getter/Setter
         */

        public Table Table
        {
            get { return table; }
            set { table = value; }
        }

        public Boolean ShowCandidates
        {
            get { return showCandidates; }
            set { 
                showCandidates = value;
                foreach (var row in cellPanels)
                {
                    foreach (var cellPanel in row)
                    {
                        cellPanel.RefreshView();
                    }
                }
            }
        }


    }
}
