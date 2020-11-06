namespace GameHost1.Universes.Evance.Milestone2
{
    public class TimeSettings
    {
        public int MaxGeneration { get; set; } = 5000;

        /// <summary>
        /// 普朗克時間，取其為時間的基本單位的意義。
        /// </summary>
        public int PlanckTimeInMillisecond { get; set; } = 200;
    }
}
