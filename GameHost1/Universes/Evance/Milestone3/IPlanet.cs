namespace GameHost1.Universes.Evance.Milestone3
{
    public interface IPlanet : IPlanetReadOnly
    {
        public bool TryPutLife(ILife life);
    }
}
