namespace GameHost1.Universes.Evance.Milestone2
{
    public class Planet : PlanetBase
    {
        public Planet(int x, int y) : base(x, y)
        {
        }

        public Planet(ILife[,] lives) : base(lives)
        {
        }
    }
}
