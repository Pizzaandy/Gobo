namespace PrettierGML.Tests;

public class TestFile
{
    public string FilePath;
    public string Name;

    public TestFile(string filePath)
    {
        FilePath = filePath;
        Name = Path.GetFileName(filePath);
    }

    public override string ToString()
    {
        return Name;
    }
}
