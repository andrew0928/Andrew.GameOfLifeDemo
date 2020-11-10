using System;
using System.IO;

namespace GameHost1.Tests
{
    class SnapshotHelper
    {
        public static bool Snapshot(string running_name)
        {
            var setting = Milestone3PatternsTest.LoadSettings($"{running_name}-settings.json");

            int width = setting.InitMap.GetLength(0);
            int depth = setting.InitMap.GetLength(1);

            IWorld world = Program.CreateWorld(width, depth);
            world.Init(
                setting.InitMap,
                setting.InitFrames,
                setting.InitStarts,
                setting.InitMapFrame);


            var patterns = Milestone3PatternsTest.LoadPatterns($"{running_name}-logs.json").GetEnumerator();
            var runnings = world.Running(TimeSpan.FromDays(1), false).GetEnumerator();

            int count = 0;

            StreamWriter andrew = new StreamWriter(ConfigProvider.TestSnapshotTxtPath);
            StreamWriter fion = new StreamWriter(ConfigProvider.FionSnapshotTxtPath);

            do
            {
                if (patterns.MoveNext() == false) break;
                if (runnings.MoveNext() == false) break;

                int tmp = 0;

                foreach (var (x, y) in ArrayHelper.ForEachPos<ILife>(runnings.Current.matrix))
                {
                    andrew.Write(patterns.Current.Maps[x, y].IsAlive == true ? "1" : "0");
                    fion.Write(runnings.Current.matrix[x, y].IsAlive == true ? "1" : "0");

                    tmp++;

                    if (tmp % 5 == 0)
                    {
                        andrew.WriteLine();
                        fion.WriteLine();
                    }

                    if (tmp % 25 == 0)
                    {
                        andrew.WriteLine("==>" + patterns.Current.Time);
                        andrew.WriteLine();

                        fion.WriteLine("==>" + runnings.Current.time);
                        fion.WriteLine();
                    }
                }
                andrew.Flush();
                fion.Flush();

                count++;
            } while (true);
            return true;
        }
    }
}
