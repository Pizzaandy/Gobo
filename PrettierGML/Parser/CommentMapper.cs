using Antlr4.Runtime;
using PrettierGML.SyntaxNodes;
using Range = PrettierGML.SyntaxNodes.Range;

namespace PrettierGML.Parser
{
    internal class CommentMapper
    {
        public CommonTokenStream TokenStream { get; set; }

        public List<CommentGroup> CommentGroups { get; set; } = new();

        public CommentMapper(CommonTokenStream tokenStream)
        {
            TokenStream = tokenStream;
        }

        public GmlSyntaxNode AttachComments(GmlSyntaxNode ast)
        {
            var comments = GetAllCommentGroups(TokenStream);
            CommentGroups = comments.Select(comment => DecorateComment(ast, comment)).ToList();

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

        /// <summary>
        /// The tiebreaker for remaining comment groups.
        /// </summary>
        private bool RemainingCommentIsLeading(GmlSyntaxNode followingNode, CommentGroup comment)
        {
            // Ensure that all tokens between the comment and followingNode are whitespace or parentheses
            var tokens = TokenStream
                .GetTokens(comment.TokenRange.Stop, followingNode.TokenRange.Start)
                .Skip(1)
                .SkipLast(1);

            foreach (var token in tokens)
            {
                if (IsWhiteSpace(token) || token.Type == GameMakerLanguageLexer.OpenParen)
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
                var middle = left + right >> 1;
                var child = childNodes[middle];
                var start = child.CharacterRange.Start;
                var stop = child.CharacterRange.Stop;

                if (start <= comment.CharacterRange.Start && comment.CharacterRange.Stop <= stop)
                {
                    return DecorateComment(child, comment, child);
                }

                if (stop <= comment.CharacterRange.Start)
                {
                    // This child node falls completely before the comment.
                    // Because we will never consider this node or any nodes
                    // before it again, this node must be the closest preceding
                    // node we have encountered so far.
                    precedingNode = child;
                    left = middle + 1;
                    continue;
                }

                if (comment.CharacterRange.Stop <= start)
                {
                    // This child node falls completely after the comment.
                    // Because we will never consider this node or any nodes after
                    // it again, this node must be the closest following node we
                    // have encountered so far.
                    followingNode = child;
                    right = middle;
                    continue;
                }

                throw new InvalidOperationException("Comment location overlaps with node location");
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

            result = result.Where(CanAttachComment).ToList();
            result.Sort(new GmlNodeComparer());
            return result;
        }

        /// <summary>
        /// Iterate all tokens and group together comments
        /// </summary>
        private static List<CommentGroup> GetAllCommentGroups(CommonTokenStream tokenStream)
        {
            tokenStream.Fill();

            var tokens = tokenStream.GetTokens(0, tokenStream.Size - 1);
            var groups = new List<CommentGroup>();
            List<IToken>? currentGroup = null;

            foreach (var token in tokens)
            {
                bool breakGroup = false;

                if (IsWhiteSpace(token))
                {
                    // nothing
                }
                else if (IsComment(token))
                {
                    currentGroup ??= new();
                }
                else
                {
                    breakGroup = true;
                }

                if (
                    currentGroup
                        ?.AsEnumerable()
                        .Reverse()
                        .TakeWhile(IsWhiteSpace)
                        .Count(IsLineBreak) >= 2
                )
                {
                    breakGroup = true;
                }

                if (breakGroup && currentGroup?.Count > 0)
                {
                    // Remove whitespace from end of group
                    var trimmedGroup = currentGroup.AsEnumerable();

                    while (IsWhiteSpace(trimmedGroup.Last()))
                    {
                        trimmedGroup = trimmedGroup.SkipLast(1);
                    }

                    var first = trimmedGroup.First();
                    var last = trimmedGroup.Last();

                    groups.Add(
                        new CommentGroup(
                            trimmedGroup.ToList(),
                            new Range(first.StartIndex, last.StopIndex),
                            new Range(first.TokenIndex, last.TokenIndex)
                        )
                    );

                    currentGroup = IsComment(token) ? new() : null;
                }

                currentGroup?.Add(token);
            }

            return groups;
        }

        private static bool CanAttachComment(GmlSyntaxNode node)
        {
            return !(node is EmptyNode or NodeList);
        }

        private bool IsOwnLineComment(CommentGroup comment)
        {
            var leadingTokens = TokenStream.GetHiddenTokensToLeft(comment.TokenRange.Start);
            return leadingTokens is not null
                && leadingTokens.TakeWhile(token => !IsComment(token)).Any(IsLineBreak);
        }

        private bool IsEndOfLineComment(CommentGroup comment)
        {
            var trailingTokens = TokenStream.GetHiddenTokensToRight(comment.TokenRange.Stop);
            return trailingTokens is not null
                && trailingTokens.TakeWhile(token => !IsComment(token)).Any(IsLineBreak);
        }

        private static bool IsComment(IToken token)
        {
            return token.Type == GameMakerLanguageLexer.SingleLineComment
                || token.Type == GameMakerLanguageLexer.MultiLineComment;
        }

        private static bool IsLineBreak(IToken token)
        {
            return token.Type == GameMakerLanguageLexer.LineTerminator;
        }

        private static bool IsWhiteSpace(IToken token)
        {
            return token.Type == GameMakerLanguageLexer.LineTerminator
                || token.Type == GameMakerLanguageLexer.WhiteSpaces;
        }

        private static void AttachCommentGroup(
            GmlSyntaxNode node,
            CommentGroup comment,
            CommentType type
        )
        {
            comment.Type = type;
            node.Comments.Add(comment);
            Console.WriteLine(comment);
        }
    }

    internal class GmlNodeComparer : Comparer<GmlSyntaxNode>
    {
        public override int Compare(GmlSyntaxNode? nodeA, GmlSyntaxNode? nodeB)
        {
            if (nodeA!.CharacterRange.Start == nodeB!.CharacterRange.Start)
            {
                return nodeA.CharacterRange.Stop - nodeB.CharacterRange.Stop;
            }
            else
            {
                return nodeA.CharacterRange.Start - nodeB.CharacterRange.Start;
            }
        }
    }
}
