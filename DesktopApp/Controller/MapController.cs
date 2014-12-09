using DesktopApp.Structure;
using DesktopApp.Databases;
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
        public MapData mapData;

        public MapController(TableController tableCtrl, int index)
        {
            tableController = tableCtrl;
            mapData = new MapData();
            GenerateMap(index);
        }

        private List<List<Cell>> ParseMap(String map)
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

        public void GenerateMap(int index)
        {
            tableController.Table.Cells = ParseMap(mapData.GetMapByIndex(index));
            tableController.Table.Boxes = ParseBox(mapData.GetBoxByIndex(index));
            tableController.RenderTable();
        }
    }
}
