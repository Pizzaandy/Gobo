namespace Gobo.Text;

public class StringText : SourceText
{
    public string Source;

    public override int Length => Source.Length;

    public override char this[int position] => Source[position];

    public StringText(string code)
    {
        Source = code;
    }

    public override string ReadSpan(TextSpan span)
    {
        return Source.Substring(span.Start, span.Length);
    }

    public override void CopyTo(
        int sourceIndex,
        char[] destination,
        int destinationIndex,
        int count
    )
    {
        Source.CopyTo(sourceIndex, destination, destinationIndex, count);
    }
}
