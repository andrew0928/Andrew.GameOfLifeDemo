using System.IO;

namespace GameHost1.Tests
{
    public static class ConfigProvider
    {
        public static readonly string ProjectPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));

        public static readonly string TestFolderPath = "Fion.Tests";

        public static readonly string FionSnapshotTxtPath = Path.Combine(ProjectPath, TestFolderPath, "Fion.txt");

        public static readonly string TestSnapshotTxtPath = Path.Combine(ProjectPath, TestFolderPath, "Test.txt");
    }
}
