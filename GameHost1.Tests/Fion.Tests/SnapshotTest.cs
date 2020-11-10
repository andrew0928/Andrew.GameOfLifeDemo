using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameHost1.Tests
{
    [TestClass]
    public class SnapshotTest
    {
        [TestMethod]
        public void Test()
        {
            SnapshotHelper.Snapshot("running1");
        }
    }
}
