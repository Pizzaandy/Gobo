namespace Gobo.SyntaxNodes;

internal interface ISyntaxNode<T>
    where T : ISyntaxNode<T>
{
    public abstract TextSpan Span { get; set; }

    public abstract T? Parent { get; protected set; }

    public abstract List<T> Children { get; protected set; }

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
        if (a.Children.Count == 0 && b.Children.Count == 0)
        {
            if (a.GetHashCode() != b.GetHashCode())
            {
                return true;
            }
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

        difference = (default(T), default(T));
        return false;
    }
}
