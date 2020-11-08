using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameHost1.Tests
{
    [TestClass]
    public class Milestone3PatternsTest
    {
        [TestMethod]
        public void Test1()
        {
            Assert.IsTrue(TestRunningWithLogs("running1"));
        }




        public static bool TestRunningWithLogs(string running_name)
        {
            //int count = 0;
            IWorld world = null;


            var setting = LoadSettings($"{running_name}-settings.json");

            int width = setting.InitMap.GetLength(0);
            int depth = setting.InitMap.GetLength(1);

            //bool[,] matrix = new bool[width, depth];
            //foreach(var (x, y) in ArrayHelper.ForEachPos<LifeSnapshot>(setting.InitMap))
            //{
            //    matrix[x, y] = (setting.InitMap[x, y] as ILife).IsAlive;
            //}

            world = Program.CreateWorld(width, depth);
            world.Init(
                setting.InitMap,
                setting.InitFrames,
                setting.InitStarts,
                setting.InitMapFrame);


            var patterns = LoadPatterns($"{running_name}-logs.json").GetEnumerator();
            var runnings = world.Running(TimeSpan.FromDays(1), false).GetEnumerator();

            int count = 0;
            do
            {
                if (patterns.MoveNext() == false) break;
                if (runnings.MoveNext() == false) break;

                foreach (var (x, y) in ArrayHelper.ForEachPos<ILife>(runnings.Current.matrix))
                {
                    Assert.AreEqual(patterns.Current.Maps[x, y].IsAlive, runnings.Current.matrix[x, y].IsAlive);
                    Assert.AreEqual(patterns.Current.Maps[x, y].Generation, runnings.Current.matrix[x, y].Generation);
                }


                count++;
            } while (true);
            return true;
        }

        public static RunningSetting LoadSettings(string setting_file)
        {
            return JsonConvert.DeserializeObject<RunningSetting>(File.ReadAllText(setting_file));
        }


        public static IEnumerable<RunningSnapshot> LoadPatterns(string log_file)
        {
            var json = JsonSerializer.Create();
            var jsonreader = new JsonTextReader(File.OpenText(log_file));
            jsonreader.CloseInput = true;
            jsonreader.SupportMultipleContent = true;

            while (jsonreader.Read())
            {
                yield return json.Deserialize<RunningSnapshot>(jsonreader);
            }

            jsonreader.Close();
            yield break;
        }




    }


    public class RunningSetting
    {
        public int InitMapFrame;
        public bool[,] InitMap;
        public int[,] InitFrames;
        public int[,] InitStarts;

        /*
        

                        File.AppendAllText(
                    "running-settings.json",
                    JsonConvert.SerializeObject(new
                    {
                        InitMapFrame = 50,
                        InitMap = matrix,
                        InitFrames = frames,
                        InitStarts = start_frames
                    }) + "\n");




        */
    }

    public class RunningSnapshot
    {
        public int Time;
        public LifeWrapper[,] Maps;
    }

    public class LifeWrapper
    {
        public bool IsAlive;
        public int Generation;
    }
}
