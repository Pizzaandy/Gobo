using Antlr4.Runtime;
using PrettierGML.Nodes;
using PrettierGML.Nodes.SyntaxNodes;
using System.Diagnostics;

namespace PrettierGML
{
    internal class CommentMapper
    {
        public CommonTokenStream TokenStream { get; set; }

        public CommentMapper(CommonTokenStream tokenStream)
        {
            TokenStream = tokenStream;
        }

        public GmlSyntaxNode AttachComments(GmlSyntaxNode ast)
        {
            Debug.Assert(ast is Document);

            var comments = GetAllComments(TokenStream);
            var nodes = GetSortedChildNodes(ast);

            var decoratedComments = comments.Select(comment => DecorateComment(ast, comment));

            return ast;
        }

        private List<GmlSyntaxNode> GetSortedChildNodes(GmlSyntaxNode node)
        {
            var result = new List<GmlSyntaxNode>();

            foreach (var child in node.Children)
            {
                if (child is EmptyNode)
                {
                    continue;
                }
                result.Add(child);
                result.AddRange(GetSortedChildNodes(child));
            }

            result.Sort(new GmlNodeComparer());
            return result;
        }

        private static List<Comment> GetAllComments(CommonTokenStream tokenStream)
        {
            tokenStream.Fill();
            var tokens = tokenStream.GetTokens(0, tokenStream.Size - 1);
            var comments = new List<Comment>();

            foreach (var token in tokens)
            {
                if (
                    token.Type == GameMakerLanguageLexer.SingleLineComment
                    || token.Type == GameMakerLanguageLexer.MultiLineComment
                )
                {
                    comments.Add(new Comment(token.Text));
                }
            }

            return comments;
        }

        private static Comment DecorateComment(
            GmlSyntaxNode node,
            Comment comment,
            GmlSyntaxNode? enclosingNode = null
        )
        {
            return comment;
        }
    }

    internal class GmlNodeComparer : Comparer<GmlSyntaxNode>
    {
        public override int Compare(GmlSyntaxNode? nodeA, GmlSyntaxNode? nodeB)
        {
            if (nodeA!.SourceInterval.a == nodeB!.SourceInterval.a)
            {
                return nodeA.SourceInterval.b - nodeB.SourceInterval.b;
            }
            else
            {
                return nodeA.SourceInterval.a - nodeB.SourceInterval.a;
            }
        }
    }
}
