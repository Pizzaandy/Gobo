namespace Gobo;

public class SourceString : SourceText
{
    private string text;

    public SourceString(string text)
    {
        this.text = text;
    }

    public override TextReader GetReader()
    {
        return new StringReader(text);
    }
}
