namespace PrettierGML.Tests
{
    public class FormatTestFile
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }

        public FormatTestFile(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}
