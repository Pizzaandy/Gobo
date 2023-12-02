using Gobo.SyntaxNodes;

namespace Gobo.Parser;

internal class CommentMapper
{
    public SourceText SourceText { get; set; }

    public List<CommentGroup> CommentGroups { get; set; } = new();

    public CommentMapper(SourceText sourceText, List<Token[]> triviaGroups)
    {
        SourceText = sourceText;
        foreach (var triviaGroup in triviaGroups)
        {
            if (
                triviaGroup.Any(
                    t => t.Kind is TokenKind.SingleLineComment or TokenKind.MultiLineComment
                )
            )
            {
                CommentGroups.AddRange(CreateCommentGroups(triviaGroup));
            }
        }
    }

    public GmlSyntaxNode AttachComments(GmlSyntaxNode ast)
    {
        CommentGroups.ForEach(comment => DecorateComment(ast, comment));

        foreach (var comment in CommentGroups)
        {
            var followingNode = comment.FollowingNode;
            var precedingNode = comment.PrecedingNode;
            var enclosingNode = comment.EnclosingNode;

            if (IsOwnLineComment(comment))
            {
                comment.Placement = CommentPlacement.OwnLine;

                if (followingNode is not null)
                {
                    AttachCommentGroup(followingNode, comment, CommentType.Leading);
                }
                else if (precedingNode is not null)
                {
                    AttachCommentGroup(precedingNode, comment, CommentType.Trailing);
                }
                else if (enclosingNode is not null)
                {
                    AttachCommentGroup(enclosingNode, comment, CommentType.Dangling);
                }
                else
                {
                    AttachCommentGroup(ast, comment, CommentType.Dangling);
                }
            }
            else if (IsEndOfLineComment(comment))
            {
                comment.Placement = CommentPlacement.EndOfLine;

                if (precedingNode is not null)
                {
                    AttachCommentGroup(precedingNode, comment, CommentType.Trailing);
                }
                else if (followingNode is not null)
                {
                    AttachCommentGroup(followingNode, comment, CommentType.Leading);
                }
                else if (enclosingNode is not null)
                {
                    AttachCommentGroup(enclosingNode, comment, CommentType.Dangling);
                }
                else
                {
                    AttachCommentGroup(ast, comment, CommentType.Dangling);
                }
            }
            else
            {
                comment.Placement = CommentPlacement.Remaining;

                if (precedingNode is not null && followingNode is not null)
                {
                    if (RemainingCommentIsLeading(followingNode, comment))
                    {
                        AttachCommentGroup(followingNode, comment, CommentType.Leading);
                    }
                    else
                    {
                        AttachCommentGroup(precedingNode, comment, CommentType.Trailing);
                    }
                }
                else if (precedingNode is not null)
                {
                    AttachCommentGroup(precedingNode, comment, CommentType.Trailing);
                }
                else if (followingNode is not null)
                {
                    AttachCommentGroup(followingNode, comment, CommentType.Leading);
                }
                else if (enclosingNode is not null)
                {
                    AttachCommentGroup(enclosingNode, comment, CommentType.Dangling);
                }
                else
                {
                    AttachCommentGroup(ast, comment, CommentType.Dangling);
                }
            }
        }

        return ast;
    }

    private static List<CommentGroup> CreateCommentGroups(Token[] triviaTokens)
    {
        var result = new List<CommentGroup>();
        var currentGroup = new List<Token>();
        int lineBreakCount = 0;
        CommentGroup? lastGroup = null;

        void AcceptCommentGroup()
        {
            if (currentGroup.Count == 0)
            {
                return;
            }

            var newGroup = new CommentGroup(currentGroup.ToArray());
            result.Add(newGroup);

            currentGroup.Clear();
            lastGroup = newGroup;
            lineBreakCount = 0;
        }

        foreach (var token in triviaTokens)
        {
            if (token.Kind is TokenKind.LineBreak)
            {
                AcceptCommentGroup();
                lineBreakCount++;
            }
            else if (token.Kind is TokenKind.SingleLineComment or TokenKind.MultiLineComment)
            {
                currentGroup.Add(token);
            }
        }

        AcceptCommentGroup();

        return result;
    }

    /// <summary>
    /// Determine whether a remaining comment group should be attached to the preceding or following node.
    /// </summary>
    private bool RemainingCommentIsLeading(GmlSyntaxNode followingNode, CommentGroup comment)
    {
        // Check if all tokens between the comment and followingNode are whitespace or parentheses

        var whitespaceBetween = SourceText.ReadSpan(comment.Span.End, followingNode.Span.Start);

        foreach (var character in whitespaceBetween)
        {
            if (char.IsWhiteSpace(character) || character == '(')
            {
                continue;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    private static CommentGroup DecorateComment(
        GmlSyntaxNode node,
        CommentGroup comment,
        GmlSyntaxNode? enclosingNode = null
    )
    {
        var childNodes = GetSortedChildNodes(node);

        var left = 0;
        var right = childNodes.Count;
        GmlSyntaxNode? precedingNode = null;
        GmlSyntaxNode? followingNode = null;

        while (left < right)
        {
            var middle = (left + right) >> 1;
            var child = childNodes[middle];

            var start = child.Span.Start;
            var end = child.Span.End;
            var commentStart = comment.Span.Start;
            var commentEnd = comment.Span.End;

            // The comment is completely contained by this child node
            // Abandon the binary search at this level.
            if (start <= commentStart && commentEnd <= end)
            {
                return DecorateComment(child, comment, child);
            }

            if (end <= commentStart)
            {
                // This child node falls completely before the comment.
                // Because we will never consider this node or any nodes
                // before it again, this node must be the closest preceding
                // node we have encountered so far.
                precedingNode = child;
                left = middle + 1;
                continue;
            }

            if (commentEnd <= start)
            {
                // This child node falls completely after the comment.
                // Because we will never consider this node or any nodes after
                // it again, this node must be the closest following node we
                // have encountered so far.
                followingNode = child;
                right = middle;
                continue;
            }

            throw new Exception("Comment location overlaps with node location");
        }

        comment.EnclosingNode = enclosingNode;
        comment.PrecedingNode = precedingNode;
        comment.FollowingNode = followingNode;

        return comment;
    }

    private static List<GmlSyntaxNode> GetSortedChildNodes(GmlSyntaxNode node)
    {
        var result = new List<GmlSyntaxNode>();

        foreach (var child in node.Children)
        {
            if (CanAttachComment(child))
            {
                result.Add(child);
            }
            else
            {
                result.AddRange(GetSortedChildNodes(child));
            }
        }

        //result = result.Where(CanAttachComment).ToList();
        result.Sort(new GmlNodeComparer());
        return result;
    }

    private static bool CanAttachComment(GmlSyntaxNode node)
    {
        return !(node is EmptyNode or NodeList);
    }

    private bool IsOwnLineComment(CommentGroup comment)
    {
        bool firstInLine =
            SourceText.GetLineBreaksToLeft(comment.Span) > 0 || comment.Span.Start == 0;

        bool lastInLine =
            SourceText.GetLineBreaksToRight(comment.Span) > 0
            || comment.Span.End == SourceText.Length;

        return firstInLine && lastInLine;
    }

    private bool IsEndOfLineComment(CommentGroup comment)
    {
        bool noLeadingLineBreaks = SourceText.GetLineBreaksToLeft(comment.Span) == 0;

        bool lastInLine =
            SourceText.GetLineBreaksToRight(comment.Span) > 0
            || comment.Span.End == SourceText.Length;

        return noLeadingLineBreaks && lastInLine;
    }

    private static void AttachCommentGroup(
        GmlSyntaxNode node,
        CommentGroup comment,
        CommentType type
    )
    {
        comment.Type = type;
        node.Comments.Add(comment);
    }
}

internal class GmlNodeComparer : Comparer<GmlSyntaxNode>
{
    public override int Compare(GmlSyntaxNode? nodeA, GmlSyntaxNode? nodeB)
    {
        if (nodeA!.Span.Start == nodeB!.Span.Start)
        {
            return nodeA.Span.End - nodeB.Span.End;
        }
        else
        {
            return nodeA.Span.Start - nodeB.Span.Start;
        }
    }
}
