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
            //String map = "080000090009301000000000000590000037000609000230000061000000000600907208000000040";
            //String box = "122223444112223344111223444151133374555663377555669997558669997888869977888866977";
            // első jó pálya, quad miatt megakad
            //String map = "400709020000020000090008000104000300700401002002000103000600010000040000010207045";
            //String box = "111222233111222233114452333144455633444555666774556669777856699778888999778888999"; 
            // első jó pálya, quad után
            String map = "400719028000020091290008034104002300700401002042000103020630010000040200310207045";
            String box = "111222233111222233114452333144455633444555666774556669777856699778888999778888999";
            //naked pair tesz
            //String map = "400000038002004100005300240070609004020000070600703090057008300003900400240000009";
            //String box = "111222333111222333111222333444555666444555666444555666777888999777888999777888999";
            //hidden pair teszt 1
            //String map = "720408030080000047401076802810739000000851000000264080209680413340000008168943275";
            //String box = "111222333111222333111222333444555666444555666444555666777888999777888999777888999";
            //hidden pair teszt 2

            //String map = "000000000904607000076804100309701080708000301051308702007502610005403208000000000";
            //String box = "111222333111222333111222333444555666444555666444555666777888999777888999777888999";
            
            //box-line reduction teszt 1
            //String map = "016007803090800000870001260048000300650009082039000650060900020080002936924600510";
            //String box = "111222333111222333111222333444555666444555666444555666777888999777888999777888999";
            //xwing teszt 1
            //String map = "100000569492056108056109240009640801064010000218035604040500016905061402621000005";
            //String box = "111222333111222333111222333444555666444555666444555666777888999777888999777888999";
            // difficult map
            //String map = "000609000090860020000000700008006004000070000700108506000000000010054080000081000";
            //String box = "111122233111222333411222333444555663444555666744555666777888996777888999778889999";
            // standard map
            //String map = "100469008507080046060000100010030000305001680071906002002390060000007400750600329";
            //String box = "112222233111222333411123336441555366444555666447555966477789996777888999778888899";
            //easy map
            //String map = "050079830100500978287060009319485060040700000500013784004100092005927301973006100";
            //String box = "112222233111222333411123336441555366444555666447555966477789996777888999778888899";
            tableController.Table.Cells = ParseMap(map, box);
            tableController.Table.Boxes = ParseBox(box);
            tableController.RenderTable();
        }
    }
}
