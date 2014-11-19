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
        int[,] output;

        public TableController()
        {
            InitializeTable();
        }

        private void InitializeTable()
        {
            output = new int[27, 27];
            table = new Table();

            for (int y = 0; y < 9; y++)
            {
                List<Cell> row = new List<Cell>();
                Box tempbox = new Box();

                for (int x = 0; x < 9; x++)
                {
                    List<int> candidate = new List<int>();
                    Cell cell = new Cell(x, y, (int)((x + 1) * (y + 1)) % 9, tempbox);

                    for (int i = 1; i < 10; i++)   candidate.Add(i);

                    cell.Candidates = candidate;
                    row.Add(cell);
                }
                table.Cells.Add(row);
            }
            CreateOutput();
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
    }
}
