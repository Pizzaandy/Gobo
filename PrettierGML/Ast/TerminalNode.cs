using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.Diagnostics;

namespace PrettierGML.Nodes
{
    internal class TerminalNode : GmlSyntaxNode
    {
        private readonly int tokenId = -1;

        public TerminalNode(ITerminalNode token)
        {
            CharacterRange = new Range(token.SourceInterval.a, token.SourceInterval.b);
            TokenRange = new Range(token.Symbol.StartIndex, token.Symbol.StartIndex);
            tokenId = token.Symbol.Type;
        }

        public TerminalNode(ParserRuleContext context)
            : base(context)
        {
            Debug.Assert(context.Start.TokenIndex == context.Stop.TokenIndex);
            tokenId = context.Start.Type;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Kind);
            hashCode.Add(tokenId);
            return hashCode.ToHashCode();
        }
    }
}
