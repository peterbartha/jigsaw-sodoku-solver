﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Databases
{
    class MapData
    {
        private int mapCount;
        private List<String> maps;
        private List<String> boxes;

        public MapData()
        {
            maps = new List<String>();
            boxes = new List<String>();
            Initialize();
            mapCount = maps.Count;
        }

        private void Initialize()
        {
            //NEM xwing teszt 1 - nem tesztelt
            maps.Add("100000569492056108056109240009640801064010000218035604040500016905061402621000005");
            boxes.Add("111222333111222333111222333444555666444555666444555666777888999777888999777888999");
            //NEM difficult map - tippel
            maps.Add("000609000090860020000000700008006004000070000700108506000000000010054080000081000");
            boxes.Add("111122233111222333411222333444555663444555666744555666777888996777888999778889999");
            //NEM durva - tippel
            maps.Add("000300900050000009000090000000003090300524007060800000000040000100000070001005000");
            boxes.Add("123333344123333444122224444112255566112555766115557766888877776888999976889999976");
            //EASY
            maps.Add("400719028000020091290008034104002300700401002042000103020630010000040200310207045");
            boxes.Add("111222233111222233114452333144455633444555666774556669777856699778888999778888999");
            //EASY
            maps.Add("100469008507080046060000100010030000305001680071906002002390060000007400750600329");
            boxes.Add("112222233111222333411123336441555366444555666447555966477789996777888999778888899");
            //EASY amazon easy 1
            maps.Add("300004609007509102804000000240310000010020090000031046000000704901602800706400001");
            boxes.Add("111122222111333332113343222555543666554444466555746666888747799877777999888889999");
            //EASY amazon easy 2
            maps.Add("300692007005308600000000000089145230000000000042739850000000000008904300800561004");
            boxes.Add("111222333411522633411522633411522633444555666774885996774885996774885996777888999");
            //EASY amazon easy 3
            maps.Add("040106080061000240000040000036000890004697100072000450000060000097000310010905030");
            boxes.Add("111122233112222333111223333455556666444456666444455556777788999777888899778889999");
            //EASY amazon easy 4
            maps.Add("000120000300280000050600800500902018120000049760401002005006070000014005000078000");
            boxes.Add("112233333411222223411125333411525666444555666444575886999578886977777886999997788");
            //EASY krazydad inter vol1 1
            maps.Add("000890030000034000030000006900000400004106700007000002500000080000210000080052000");
            boxes.Add("111222222311144222311144445366644545333666555366677575388877775388877999888999999");
            //EASY krazydad inter vol1 2
            maps.Add("004970000200006000043000001900008000010000020000200007100000950000500006000059300");
            boxes.Add("122233333114222333114252636114552666114555677444955677484959677888999677888889997");
            //EASY krazydad inter vol1 8
            maps.Add("000000200000250800000040000000102460600000007095803000000060000008034000001000000");
            boxes.Add("111112222111332222133344255333445555636645575666644777668447779888877999888899999");
            //EASY krazydad inter vol2 book4 1
            maps.Add("300400010000805000000000602000600104600000008209007000704000000000308000090004003");
            boxes.Add("112234444112234445112234555111234555662237755666837999666837799688837799888837799");
            //EASY krazydad inter vol2 book25 1
            maps.Add("000010000000031008000040002700000120000409000036000009400020000900270000000080000");
            boxes.Add("111112333112222233114422333444525553446555577446665777888667797888866697889999999");
           
            //MEDIUM krazydad chal vol1 book9 1
            maps.Add("900040700000009000406003078000001000200090003000700000570900401000600000009080007");
            boxes.Add("112222233111222333111123333456666678455567778445666788455597778445999788449999988");
            //MEDIUM amazon2 
            maps.Add("000704000002060800600000001030105020000000000060809050200000005004050600000402000");
            boxes.Add("111222222312244424311144444311355556333356666355556776888887776898889976999999777");
           
            //HARD krazydad inter vol2 book41 1
            maps.Add("067000000580003001000560000000400680000000000031006000000048000700300094000000860");
            boxes.Add("111222222111324442113323344153333554655555574666888874666888874669899777999999777");
            //HARD krazydad inter vol2 book48 1
            maps.Add("800400000000200000090000030900300750700060002065004007040000010000009000000003001");
            boxes.Add("111123333111123333122222223445555566447555866447758866477999886479999986477798886");
            //HARD krazydad inter vol2 book48 2
            maps.Add("900850030000000000000380100460000000090030020000000043002016000000000000010093007");
            boxes.Add("111111122314441422334444422333333552665555522655777777668888877668988897669999999");
            //HARD krazydad chal vol1 book9 2
            maps.Add("406000030000300002000402000000000000050000060000000000000508000700001000010000709");
            boxes.Add("112222233411123335441222355441666355441163355744666558777696888799999998777798888");
            
            //EXPERT krazydad inter vol2 book100 1
            maps.Add("070003000000020008002500000000040700000279000004010000000005300600080000000700010");
            boxes.Add("111223333111222223111444223555664333555664444557666648597777688997977888999997888");
            //EXPERT krazydad chal vol1 book37 2
            maps.Add("600000000100900080000340060900700040000000000070001008040086000010002005000000009");
            boxes.Add("122333445123333345122234445122666445112666455117666755817777759888879999888879999");
        
        }

        public String GetMapByIndex(int index)
        {
            return maps.ElementAt(index);
        }
        public String GetBoxByIndex(int index)
        {
            return boxes.ElementAt(index);
        }

        public int GetMapCount()
        {
            return mapCount;
        }
    }
}
