using System;
using System.Collections.Generic;
using System.Text;

namespace GameHost1.Universes.Evance
{
    public class UniverseSettings
    {
        public bool[,] DefaultAliveLivesMatrix { get; set; }

        public int MaxCoordinatesX => DefaultAliveLivesMatrix?.GetLength(0) ?? 0;

        public int MaxCoordinatesY => DefaultAliveLivesMatrix?.GetLength(1) ?? 0;

        public TimeSettings TimeSettings { get; set; } = new TimeSettings();

        public bool EnableAutoMode { get; set; }
    }
}
