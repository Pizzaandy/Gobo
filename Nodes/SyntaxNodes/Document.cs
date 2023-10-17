using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Document : GmlSyntaxNode
    {
        public GmlSyntaxNode Body { get; set; }

        public Document(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode body
        )
            : base(context, tokenStream)
        {
            Body = AsChild(body);
        }

        public override Doc Print()
        {
            return Doc.Concat(PrintHelper.PrintStatements(Body), Doc.HardLineIfNoPreviousLine);
        }
    }
}
