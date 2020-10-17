using Xunit;
using FluentAssertions;
using System.Collections.Generic;

namespace GameHost1.Tests
{
    public class TimePassRule_Tests
    {
        [Fact(DisplayName = "1. Any live cell with two or three live neighbours survives.")]
        public void Rule1()
        {
            // 跑出所有 2 個 true 的組合
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i == j) continue;
                    var list = new List<bool>() { false, false, false, false, false, false, false, false };
                    list[i] = true;
                    list[j] = true;
                    var area = new bool[,] {
                        {list[0], list[1], list[2]},
                        {list[3], true, list[4]},
                        {list[5], list[6], list[7]},
                    };
                    Program.TimePassRule(area).Should().BeTrue();
                }
            }

            // 跑出所有 3 個 true 的組合
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        if (i == j || j == k || i == k) continue;
                        var list = new List<bool>() { false, false, false, false, false, false, false, false };
                        list[i] = true;
                        list[j] = true;
                        list[k] = true;
                        var area = new bool[,] {
                            {list[0], list[1], list[2]},
                            {list[3], true, list[4]},
                            {list[5], list[6], list[7]},
                        };
                        Program.TimePassRule(area).Should().BeTrue();
                    }
                }
            }
        }

        [Fact(DisplayName = "2. Any dead cell with three live neighbours becomes a live cell.")]
        public void Rule2()
        {
            // 跑出所有 3 個 true 的組合
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        if (i == j || j == k || i == k) continue;
                        var list = new List<bool>() { false, false, false, false, false, false, false, false };
                        list[i] = true;
                        list[j] = true;
                        list[k] = true;
                        var area = new bool[,] {
                            {list[0], list[1], list[2]},
                            {list[3], false, list[4]},
                            {list[5], list[6], list[7]},
                        };
                        Program.TimePassRule(area).Should().BeTrue();
                    }
                }
            }
        }

        [Fact(DisplayName = "3. All other live cells die in the next generation. Similarly, all other dead cells stay dead.")]
        public void Rule3()
        {
            for (int i = 0; i <= 8; i++) // 幾個 true: 可以有 0~8 個
            {
                // 只測從前面開始放 true 的情境: 10000000, 11000000, 11100000, ..., 11111111
                var list = new List<bool>() { false, false, false, false, false, false, false, false };
                for (int j = 0; j < i; j++) list[j] = true;

                // 測試中間是死掉的細胞
                if (i == 3) continue;
                var areaForDeadCell = new bool[,] {
                    {list[0], list[1], list[2]},
                    {list[3], false, list[4]},
                    {list[5], list[6], list[7]},
                };
                Program.TimePassRule(areaForDeadCell).Should().BeFalse();

                if (i == 2) continue;
                // 測試中間是活著的細胞
                var areaForLiveCell = new bool[,] {
                    {list[0], list[1], list[2]},
                    {list[3], true, list[4]},
                    {list[5], list[6], list[7]},
                };
                Program.TimePassRule(areaForLiveCell).Should().BeFalse();
            }
        }
    }
}
