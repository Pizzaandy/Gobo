using Antlr4.Runtime;

namespace PrettierGML.Nodes
{
    internal class Comment
    {
        public IToken Token { get; set; }

        public Comment(IToken token)
        {
            Token = token;
        }
    }
}
