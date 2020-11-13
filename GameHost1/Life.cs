#define ENABLE_RUNNING_RECORDING

namespace GameHost1
{
    public class Life : ILife
    {
        public int Generation { get; set; }
        public bool IsAlive { get; set; }
    }
}
