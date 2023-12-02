namespace Gobo;

public class SourceFile : SourceText
{
    private string filePath;

    public SourceFile(string filePath)
    {
        this.filePath = filePath;
    }

    public override TextReader GetReader()
    {
        return new StreamReader(filePath, System.Text.Encoding.UTF8);
    }
}
