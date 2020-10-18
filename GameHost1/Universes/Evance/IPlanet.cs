namespace GameHost1.Universes.Evance
{
    public interface IPlanet : IPlanetReadOnly
    {
        public bool TryPutLife(ILife life);
    }
}
