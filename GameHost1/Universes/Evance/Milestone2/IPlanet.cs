namespace GameHost1.Universes.Evance.Milestone2
{
    public interface IPlanet : IPlanetReadOnly
    {
        public bool TryPutLife(ILife life);
    }
}
