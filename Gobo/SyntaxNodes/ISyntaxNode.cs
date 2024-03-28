namespace Gobo.SyntaxNodes;

internal interface ISyntaxNode<T>
    where T : ISyntaxNode<T>
{
    public abstract TextSpan Span { get; set; }

    public abstract T? Parent { get; protected set; }

    public abstract T[] Children { get; protected init; }

    /// <summary>
    /// Returns true if a difference is found
    /// </summary>
    public static bool TryFindDifference(T? a, T? b, out (T?, T?) difference)
    {
        difference = (a, b);

        if (a is null || b is null)
        {
            return true;
        }

        // Compare hash codes of terminal nodes
        if (a.GetHashCode() != b.GetHashCode())
        {
            if (a.Children.Length == 0 && b.Children.Length == 0)
            {
                return true;
            }

            foreach (var (first, second) in a.Children.Zip(b.Children))
            {
                if (TryFindDifference(first, second, out difference))
                {
                    if (first is null && second is null)
                    {
                        continue;
                    }
                    difference = (first, second);
                    return true;
                }
            }
        }

        difference = (default(T), default(T));
        return false;
    }
}
