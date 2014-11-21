using DesktopApp.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Controller
{
    class MapController
    {
        private TableController tableController;

        public MapController(TableController tableCtrl)
        {
            tableController = tableCtrl;
            GenerateMap();
        }

        private List<List<Cell>> ParseMap(String map, String box)
        {
            List<List<Cell>> table = new List<List<Cell>>();

            for (int y = 0; y < 9; y++)
            {
                List<Cell> row = new List<Cell>();
                for (int x = 0; x < 9; x++)
                {
                    int value;
                    if (int.TryParse(Convert.ToString(map[x + y * 9]), out value))
                    {
                        Cell cell = new Cell(x, y, value);
                        row.Add(cell);
                    }
                }
                table.Add(row);
            }

            return table;
        }

        private List<Box> ParseBox(String box)
        {
            List<Box> boxes = new List<Box>();

            for (int i = 0; i < 9; i++)
            {
                Box tempBox = new Box();
                boxes.Add(tempBox);
            }

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    int boxID;
                    if (int.TryParse(Convert.ToString(box[x + y * 9]), out boxID))
                    {
                        Cell cell = tableController.Table.Cells.ElementAt(y).ElementAt(x);
                        boxes[boxID - 1].Cells.Add(cell);
                        cell.Box = boxes[boxID - 1];
                    }
                }
            }

            return boxes;
        }

        public void GenerateMap()
        {
            String map = "080000090009301000000000000590000037000609000230000061000000000600907208000000040";
            String box = "122223444112223344111223444151133374555663377555669997558669997888864977888869977";
            tableController.Table.Cells = ParseMap(map, box);
            tableController.Table.Boxes = ParseBox(box);
            tableController.RenderTable();
        }
    }
}
