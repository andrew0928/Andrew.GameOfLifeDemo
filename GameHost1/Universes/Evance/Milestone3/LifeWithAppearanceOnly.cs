namespace GameHost1.Universes.Evance.Milestone3
{
    /// <summary>
    /// 花瓶 Life
    /// </summary>
    public class LifeWithAppearanceOnly : ILife
    {
        public virtual bool IsAlive { get; }

        public LifeWithAppearanceOnly(bool isAlive)
        {
            this.IsAlive = isAlive;
        }

        public LifeWithAppearanceOnly(ILife sourceLife)
        {
            this.IsAlive = sourceLife.IsAlive;
        }

        public static LifeWithAppearanceOnly Transform(ILife sourceLife)
        {
            return new LifeWithAppearanceOnly(sourceLife);
        }
    }
}
