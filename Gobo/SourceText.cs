namespace Gobo;

public abstract class SourceText
{
    public readonly string? FilePath;

    public abstract TextReader GetReader();

    public string ReadAllText()
    {
        var reader = GetReader();
        return reader.ReadToEnd();
    }

    public string ReadSpan(TextSpan span)
    {
        int index = 0;
        var reader = GetReader();

        while (index < span.Start)
        {
            reader.Read();
            index++;
        }

        var buffer = new char[span.Length];
        reader.ReadBlock(buffer, 0, span.Length);

        return new string(buffer);
    }

    public string ReadSpan(int start, int end)
    {
        return ReadSpan(new TextSpan(start, end));
    }
}
