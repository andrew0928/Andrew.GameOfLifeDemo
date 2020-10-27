using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameHost1.Tests
{
    //[TestClass]
    public class TimePassRuleTest
    {
        /*
        
        Rule(s):
        1. 每個細胞有兩種狀態 - 存活或死亡，每個細胞與以自身為中心的周圍八格細胞產生互動（如圖，黑色為存活，白色為死亡）
        2. 當前細胞為存活狀態時，當周圍的存活細胞低於2個時（不包含2個），該細胞變成死亡狀態。（模擬生命數量稀少）
        3. 當前細胞為存活狀態時，當周圍有2個或3個存活細胞時，該細胞保持原樣。
        4. 當前細胞為存活狀態時，當周圍有超過3個存活細胞時，該細胞變成死亡狀態。（模擬生命數量過多）
        5. 當前細胞為死亡狀態時，當周圍有3個存活細胞時，該細胞變成存活狀態。（模擬繁殖）
        
        */

        [TestMethod("規則測試2: 當前細胞為存活狀態時，當周圍的存活細胞低於2個時（不包含2個），該細胞變成死亡狀態。")]
        public void Rule2_Test()
        {
            // lives: 0
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { false, false, false },
                { false, true, false },
                { false, false, false}
            }));

            // lives: 1
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, false, false },
                { false, true, false },
                { false, false, false}
            }));

            // lives: 1
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { false, true, false },
                { false, true, false },
                { false, false, false}
            }));

            // lives: 1
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { false, false, true },
                { false, true, false },
                { false, false, false}
            }));

            // lives: 1
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { false, false, false },
                { false, true, true },
                { false, false, false}
            }));

            // lives: 1
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { false, false, false },
                { false, true, false },
                { false, false, true}
            }));

            // lives: 1
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { false, false, false },
                { false, true, false },
                { false, true, false}
            }));

            // lives: 1
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { false, false, false },
                { false, true, false },
                { true, false, false}
            }));

            // lives: 1
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { false, false, false },
                { true, true, false },
                { false, false, false}
            }));

        }

        [TestMethod("規則測試3: 當前細胞為存活狀態時，當周圍有2個或3個存活細胞時，該細胞保持原樣")]
        public void Rule3_Test()
        {
            // lives: 2
            Assert.IsTrue(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, true, false },
                { false, true, false },
                { false, false, false}
            }));

            // lives: 3
            Assert.IsTrue(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, true, true},
                { false, true, false },
                { false, false, false}
            }));
        }

        [TestMethod("規則測試4: 當前細胞為存活狀態時，當周圍有超過3個存活細胞時，該細胞變成死亡狀態。（模擬生命數量過多）")]
        public void Rule4_Test()
        {
            // lives: 4
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, true, true },
                { true, true, false },
                { false, false, false}
            }));

            // lives: 5
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, true, true },
                { true, true, true },
                { false, false, false}
            }));

            // lives: 6
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, true, true },
                { true, true, true },
                { true, false, false}
            }));

            // lives: 7
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, true, true },
                { true, true, true },
                { true, true, false}
            }));

            // lives: 8
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, true, true },
                { true, true, true },
                { true, true, true}
            }));


        }

        [TestMethod("規則測試5: 當前細胞為死亡狀態時，當周圍有3個存活細胞時，該細胞變成存活狀態。（模擬繁殖）")]
        public void Rule5_Test()
        {
            // lives: 0
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { false, false, false },
                { false, false, false },
                { false, false, false}
            }));

            // lives: 1
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, false, false },
                { false, false, false },
                { false, false, false}
            }));

            // lives: 2
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, true, false },
                { false, false, false },
                { false, false, false}
            }));

            // lives: 3
            Assert.IsTrue(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, true, true },
                { false, false, false },
                { false, false, false}
            }));

            // lives: 4
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, true, true },
                { true, false, false },
                { false, false, false}
            }));

            // lives: 5
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, true, true },
                { true, false, true },
                { false, false, false}
            }));

            // lives: 6
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, true, true },
                { true, false, true },
                { true, false, false}
            }));

            // lives: 7
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, true, true },
                { true, false, true },
                { true, true, false}
            }));

            // lives: 8
            Assert.IsFalse(GameHost1.Program.TimePassRule(new bool[3, 3]
            {
                { true, true, true },
                { true, false, true },
                { true, true, true}
            }));


        }

    }
}
