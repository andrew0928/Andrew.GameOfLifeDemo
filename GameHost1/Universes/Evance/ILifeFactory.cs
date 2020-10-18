namespace GameHost1.Universes.Evance
{
    public interface ILifeFactory
    {
        public ILife Generate(LifeSettings lifeSettings);
    }
}
