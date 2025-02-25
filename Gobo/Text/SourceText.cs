using System.Diagnostics;

namespace Gobo.Text;

public abstract class SourceText
{
    public abstract char this[int position] { get; }
    public abstract int Length { get; }

    private const int CharBufferSize = 32 * 1024;
    private const int CharBufferCount = 5;

    private static readonly ObjectPool<char[]> s_charArrayPool =
        new(() => new char[CharBufferSize], CharBufferCount);

    public abstract string ReadSpan(TextSpan span);

    public abstract void CopyTo(
        int sourceIndex,
        char[] destination,
        int destinationIndex,
        int count
    );

    public string ReadSpan(int start, int end)
    {
        return ReadSpan(new TextSpan(start, end));
    }

    public static SourceText From(string text)
    {
        return new StringText(text);
    }

    /// <summary>
    /// Implements equality comparison of the content of two different instances of <see cref="SourceText"/>.
    /// </summary>
    public virtual bool ContentEquals(SourceText other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (this.Length != other.Length)
        {
            return false;
        }

        var buffer1 = s_charArrayPool.Allocate();
        var buffer2 = s_charArrayPool.Allocate();
        Debug.Assert(buffer1.Length == buffer2.Length);
        Debug.Assert(buffer1.Length == CharBufferSize);

        try
        {
            for (
                int position = 0, length = this.Length;
                position < length;
                position += CharBufferSize
            )
            {
                var count = Math.Min(this.Length - position, CharBufferSize);
                this.CopyTo(sourceIndex: position, buffer1, destinationIndex: 0, count);
                other.CopyTo(sourceIndex: position, buffer2, destinationIndex: 0, count);

                if (!buffer1.AsSpan(0, count).SequenceEqual(buffer2.AsSpan(0, count)))
                {
                    return false;
                }
            }

            return true;
        }
        finally
        {
            s_charArrayPool.Free(buffer2);
            s_charArrayPool.Free(buffer1);
        }
    }

    public virtual int GetLineBreaksToLeft(TextSpan span)
    {
        var start = span.Start - 1;

        if (start <= 0)
        {
            return 0;
        }

        var lineBreakCount = 0;

        for (var index = start; index >= 0; index--)
        {
            var character = this[index];
            if (character == '\n')
            {
                lineBreakCount++;
            }
            else if (!char.IsWhiteSpace(character))
            {
                break;
            }
        }

        return lineBreakCount;
    }

    public virtual int GetLineBreaksToRight(TextSpan span)
    {
        var end = span.End;

        if (end >= Length - 1)
        {
            return 0;
        }
        var lineBreakCount = 0;

        for (var index = end; index < Length; index++)
        {
            var character = this[index];
            if (character == '\n')
            {
                lineBreakCount++;
            }
            else if (!char.IsWhiteSpace(character))
            {
                break;
            }
        }

        return lineBreakCount;
    }
}
