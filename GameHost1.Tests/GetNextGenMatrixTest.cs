using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GameHost1.Tests
{
    [TestClass]
    public class GetNextGenMatrixPatternsTest
    {
        [TestMethod("穩定狀態1: 板凳(1)")]
        public void StaticPattern1Test()
        {
            Assert.IsTrue(PatternTest(
                new string[]
                    {
                        "11",
                        "11",
                    },
                new string[]
                    {
                        "11",
                        "11",
                    }
            ));
        }

        [TestMethod("穩定狀態2: 麵包(1)")]
        public void StaticPattern2Test()
        {
            Assert.IsTrue(PatternTest(
                new string[]
                    {
                        "000000",
                        "001100",
                        "010010",
                        "001010",
                        "000100",
                        "000000",
                    },
                new string[]
                    {
                        "000000",
                        "001100",
                        "010010",
                        "001010",
                        "000100",
                        "000000",
                    }
            ));
        }

        [TestMethod("穩定狀態3: 蜂巢(1)")]
        public void StaticPattern3Test()
        {
            Assert.IsTrue(PatternTest(
                new string[]
                    {
                        "000000",
                        "001100",
                        "010010",
                        "001100",
                        "000000",
                    },
                new string[]
                    {
                        "000000",
                        "001100",
                        "010010",
                        "001100",
                        "000000",
                    }
            ));
        }

        [TestMethod("震盪狀態1: 信號燈(2)")]
        public void LoopPattern1Test()
        {
            Assert.IsTrue(PatternTest(
                new string[] {
                    "00000",
                    "00000",
                    "01110",
                    "00000",
                    "00000"
                },
                new string[] {
                    "00000",
                    "00100",
                    "00100",
                    "00100",
                    "00000"
                }
            )); 
            
            Assert.IsTrue(PatternTest(
                new string[] {
                    "00000",
                    "00100",
                    "00100",
                    "00100",
                    "00000"
                },
                new string[] {
                    "00000",
                    "00000",
                    "01110",
                    "00000",
                    "00000"
                }
            ));
        }

        [TestMethod("震盪狀態2: 蟾蜍(2)")]
        public void LoopPattern2Test()
        {
            Assert.IsTrue(PatternTest(
                new string[] {
                    "000000",
                    "000000",
                    "001110",
                    "011100",
                    "000000",
                    "000000",
                },
                new string[] {
                    "000000",
                    "000100",
                    "010010",
                    "010010",
                    "001000",
                    "000000",
                }
            ));
            Assert.IsTrue(PatternTest(
                new string[] {
                    "000000",
                    "000100",
                    "010010",
                    "010010",
                    "001000",
                    "000000",
                },
                new string[] {
                    "000000",
                    "000000",
                    "001110",
                    "011100",
                    "000000",
                    "000000",
                }
            ));
        }
        [TestMethod("震盪狀態3: 烽火(2)")]
        public void LoopPattern3Test()
        {
            Assert.IsTrue(PatternTest(
                new string[] {
                        "000000",
                        "011000",
                        "010000",
                        "000010",
                        "000110",
                        "000000",
                },
                new string[] {
                        "000000",
                        "011000",
                        "011000",
                        "000110",
                        "000110",
                        "000000",
                }
            ));
            Assert.IsTrue(PatternTest(
                new string[] {
                        "000000",
                        "011000",
                        "011000",
                        "000110",
                        "000110",
                        "000000",
                },
                new string[] {
                        "000000",
                        "011000",
                        "010000",
                        "000010",
                        "000110",
                        "000000",
                }
            ));
        }
        [TestMethod("會移動的振盪狀態1: 滑翔機 (4)")]
        public void DynamicLoopPattern1Test()
        {
            Assert.IsTrue(PatternTest(
                new string[] {
                        "000000",
                        "010000",
                        "001100",
                        "011000",
                        "000000",
                        "000000",
                },
                new string[] {
                        "000000",
                        "001000",
                        "000100",
                        "011100",
                        "000000",
                        "000000",
                }
            ));
            Assert.IsTrue(PatternTest(
                new string[] {
                        "000000",
                        "001000",
                        "000100",
                        "011100",
                        "000000",
                        "000000",
                },
                new string[] {
                        "000000",
                        "000000",
                        "010100",
                        "001100",
                        "001000",
                        "000000",
                }
            ));
            Assert.IsTrue(PatternTest(
                new string[] {
                        "000000",
                        "000000",
                        "010100",
                        "001100",
                        "001000",
                        "000000",
                },
                new string[] {
                        "000000",
                        "000000",
                        "000100",
                        "010100",
                        "001100",
                        "000000",
                }
            ));
            Assert.IsTrue(PatternTest(
                new string[] {
                        "000000",
                        "000000",
                        "000100",
                        "010100",
                        "001100",
                        "000000",
                },
                new string[] {
                        "000000",
                        "000000",
                        "001000",
                        "000110",
                        "001100",
                        "000000",
                }
            ));
        }

        private bool PatternTest(string[] input, string[] expected_result)
        {
            var input_map = new Map(_Transform(input));
            var expected_map = new Map(_Transform(expected_result));
            var actual_map = input_map.GetNextGeneration();

            try
            {
                CompareMap(expected_map, actual_map);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void CompareMap(Map source, Map target)
        {
            if (source == null) throw new ArgumentNullException();
            if (target == null) throw new ArgumentNullException();
            if (source.Width != target.Width) throw new ArgumentOutOfRangeException();
            if (source.Height != target.Height) throw new ArgumentOutOfRangeException();

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    if (source.Matrix[x, y].Status != target.Matrix[x, y].Status) throw new ArgumentException();
                }
            }

            return;
        }

        private Cell[,] _Transform(string[] map)
        {
            Cell[,] matrix = new Cell[map[0].Length, map.Length];

            int x = 0;
            int y = 0;
            foreach (var line in map)
            {
                foreach (var c in line)
                {
                    matrix[x, y] = new Cell
                    {
                        Status = (c == '1')
                    };
                    x++;
                }
                x = 0;
                y++;
            }

            return matrix;
        }

    }
}
