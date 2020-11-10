namespace GameHost1.Universes.Evance.Milestone3
{
    /// <summary>
    /// 花瓶 Life
    /// </summary>
    public class LifeWithAppearanceOnly : ILife
    {
        public virtual bool IsAlive { get; }

        public virtual int Generation { get; }

        public LifeWithAppearanceOnly(ILife sourceLife)
        {
            this.IsAlive = sourceLife.IsAlive;
            this.Generation = sourceLife.Generation;
        }
    }
}
