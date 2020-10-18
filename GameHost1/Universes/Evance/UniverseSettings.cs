namespace GameHost1.Universes.Evance
{
    public class UniverseSettings
    {
        public bool[,] DefaultAliveLivesMatrix { get; set; }

        public int MaxCoordinatesX => DefaultAliveLivesMatrix?.GetLength(0) ?? 0;

        public int MaxCoordinatesY => DefaultAliveLivesMatrix?.GetLength(1) ?? 0;

        public TimeSettings TimeSettings { get; set; } = new TimeSettings();

        public ILifeFactory LifeFactory { get; set; } = new LifeFactory();

        public bool EnableAutoMode { get; set; }
    }
}
