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
        Cell tempCell;

        public TableController()
        {
            InitializeTable();
        }

        private void InitializeTable()
        {
            table = new Table();
            output = new int[27, 27];
            tempCell = new Cell();

            for (int y = 0; y < 9; y++)
            {
                List<Cell> temp = new List<Cell>();
                Box tempbox = new Box();
                for (int x = 0; x < 9; x++)
                {
                    List<int> tempcand = new List<int>();
                    for (int i = 1; i < 10; i++)
                    {
                        tempcand.Add(i);

                            //Console.WriteLine("gechi" + tempcand[i-1]);
                        
                    }
                    
                    temp.Add(new Cell(x, y, (int)((x + 1) * (y + 1)) % 9, tempbox, tempcand));
                }
                table.Cells.Add(temp);
                
            }
            CreateOutput();
        }

        public void CreateOutput()
        {
            int x = -1;
            int y = -1;

            foreach (var celllist  in table.Cells)
            {
                y++;
                foreach (var cell in celllist)
                {
                    x++;
                    for (int c = 1; c < 10 ; c++)
                    {
                      //  int innerrow = c % 3;
                       // int innercolumn = (c - 1) % 3;
                        int innerrow = 0;
                        int innercolumn = 0;

                        if (c == (1 | 2 | 3)) innerrow = 0;
                        if (c == (4 | 5 | 6)) innerrow = 1;
                        if (c == (7 | 8 | 9)) innerrow = 2;
                        if (c == (1 | 4 | 7)) innercolumn = 0;
                        if (c == (2 | 5 | 8)) innercolumn = 1;
                        if (c == (3 | 6 | 9)) innercolumn = 2;
                       
                    //    Console.WriteLine((innerrow + (x * 3)) + " " + (innercolumn + (y * 3)));

                        if (cell.Candidates.Contains(c))
                               output[innerrow + (x * 3), innercolumn + (y * 3)] = c;
                            //output[innerrow + (x * 3), 10] = 36;
                        else
                            output[innerrow + (x * 3), innercolumn + (y * 3)] = c;

                        
                    }
                    Console.WriteLine(x + " " + y);
                }
                x = -1;
            } 
        } 


        public void WriteToConsole()
        {
            /*foreach (var list in table.Cells)
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
             */
  
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
