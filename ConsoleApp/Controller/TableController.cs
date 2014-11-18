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

        public void WriteToConsole()
        {
            foreach (var list in table.Cells)
            {
                Console.WriteLine(" ------------------------------------- ");
                foreach (var cell in list)
                {
                    Console.Write(" | " + cell.Value + ' ' + cell.Value + ' ' + cell.Value);
                    if (cell.X == 8) Console.Write(" | ");
                }
                Console.WriteLine();
            }
            Console.WriteLine(" ------------------------------------- ");
            Console.WriteLine();
        }
    }
}
