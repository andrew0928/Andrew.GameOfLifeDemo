using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

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
            bool[,] input_matrix = _Transform(input);
            bool[,] expected_matrix = _Transform(expected_result);
            bool[,] actual_matrix = GameHost1.Program.GetNextGenMatrix(input_matrix);

            try
            {
                CompareMatrix(expected_matrix, actual_matrix);
            }
            catch
            {
                Dump("expected_matrix", expected_matrix);
                Dump("actual_matrix", actual_matrix);

                return false;
            }

            return true;
        }

        private void Dump(string message, bool[,] matrix)
        {
            Console.WriteLine(message);
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    var c = matrix[x, y];
                    Console.Write(c ? '★' : '☆');
                }
                Console.WriteLine();
            }
        }



        private void CompareMatrix(bool[,] source, bool[,] target)
        {
            if (source == null) throw new ArgumentNullException();
            if (target == null) throw new ArgumentNullException();
            if (source.GetLength(0) != target.GetLength(0)) throw new ArgumentOutOfRangeException();
            if (source.GetLength(1) != target.GetLength(1)) throw new ArgumentOutOfRangeException();

            for (int y = 0; y < source.GetLength(1); y++)
            {
                for (int x = 0; x < source.GetLength(0); x++)
                {
                    if (source[x, y] != target[x, y]) throw new ArgumentException();
                }
            }

            return;
        }

        private bool[,] _Transform(string[] map)
        {
            bool[,] matrix = new bool[map[0].Length, map.Length];

            int x = 0;
            int y = 0;
            foreach (var line in map)
            {
                foreach (var c in line)
                {
                    matrix[x, y] = (c == '1');
                    x++;
                }
                x = 0;
                y++;
            }

            return matrix;
        }

    }
}
