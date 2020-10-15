using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Universes.Evance
{
    public static class Life
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="area">must be bool[3, 3]</param>
        /// <returns></returns>
        public static bool LifeV1(bool[,] area)
        {
            var currentCellStatus = area[1, 1];
            ushort totalLiveCells = 0;

            for (int y = 0; y < area.GetLength(0); y++)
            {
                for (int x = 0; x < area.GetLength(1); x++)
                {
                    if (area[x, y])
                    {
                        totalLiveCells++;
                    }
                }
            }

            if (currentCellStatus)
            {
                totalLiveCells--;

                if (totalLiveCells < 2 || totalLiveCells > 3)
                {
                    currentCellStatus = false;
                }
            }
            else
            {
                if (totalLiveCells == 3)
                {
                    currentCellStatus = true;
                }
            }

            return currentCellStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="area">must be bool[3, 3]</param>
        /// <returns></returns>
        public static bool LifeV2(bool[,] area)
        {
            // 自身狀態
            // 感知周遭
            // 演化結果

            //var cell = new Cell(area);
            //cell.Evolve();

            //return cell.IsLive;

            return new Cell(area).Evolve();
        }
    }
}
