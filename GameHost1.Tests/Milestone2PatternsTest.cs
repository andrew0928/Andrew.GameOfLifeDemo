using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Tests
{
    [TestClass]
    public class Milestone2PatternsTest
    {
        [TestMethod("穩定狀態1: 板凳(1)")]
        public void StaticPattern1Test()
        {
            Assert.IsTrue(BasicPatternTest(
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
            Assert.IsTrue(BasicPatternTest(
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
            Assert.IsTrue(BasicPatternTest(
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
            Assert.IsTrue(BasicPatternTest(
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
            Assert.IsTrue(BasicPatternTest(
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
            Assert.IsTrue(BasicPatternTest(
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
            Assert.IsTrue(BasicPatternTest(
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
                },
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


        private bool BasicPatternTest(string[] input, params string[][] expected_results)
        {
            bool[,] input_matrix = _Transform(input);

            int width = input_matrix.GetLength(0);
            int depth = input_matrix.GetLength(1);

            var world = Program.CreateWorld(width, depth);

            int[,] frames = new int[width, depth];
            int[,] start_frames = new int[width, depth];
            int frame = 10;
            foreach(var (x, y) in ArrayHelper.ForEachPos<bool>(input_matrix))
            {
                frames[x, y] = frame;
            }

            world.Init(input_matrix, frames, start_frames, 10);

            int count = 0;
            foreach(var lifes in world.Running(TimeSpan.MaxValue))
            {
                if (count >= expected_results.GetLength(0)) break;
                bool[,] expected_matrix = _Transform(expected_results[count++]);
                
                CompareMatrix(expected_matrix, lifes.matrix);
            }
            return true;
        }


        private void CompareMatrix(bool[,] source, ILife[,] target)
        {
            if (source == null) throw new ArgumentNullException();
            if (target == null) throw new ArgumentNullException();
            if (source.GetLength(0) != target.GetLength(0)) throw new ArgumentOutOfRangeException();
            if (source.GetLength(1) != target.GetLength(1)) throw new ArgumentOutOfRangeException();

            foreach(var (x, y) in ArrayHelper.ForEachPos<bool>(source))
            {
                if (source[x, y] != target[x, y].IsAlive) throw new ArgumentException();
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
