using Antlr4.Runtime;
using PrettierGML.Nodes;

namespace PrettierGML
{
    internal class CommentMapper
    {
        public GmlSyntaxNode Ast { get; set; }
        public CommonTokenStream TokenStream { get; set; }

        public CommentMapper(GmlSyntaxNode ast, CommonTokenStream tokenStream)
        {
            Ast = ast;
            TokenStream = tokenStream;
        }

        public GmlSyntaxNode AttachComments()
        {
            return Ast;
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

        public List<IToken> GetAllComments(CommonTokenStream tokenStream)
        {
            tokenStream.Fill();
            var tokens = tokenStream.GetTokens(0, tokenStream.Size - 1);
            var comments = new List<IToken>();

            foreach (var token in tokens)
            {
                if (
                    token.Type == GameMakerLanguageLexer.SingleLineComment
                    || token.Type == GameMakerLanguageLexer.MultiLineComment
                )
                {
                    comments.Add(token);
                }
            }

            return comments;
        }

        private static bool CanAttachComment(GmlSyntaxNode node)
        {
            return node is not NodeList;
        }
    }
}
