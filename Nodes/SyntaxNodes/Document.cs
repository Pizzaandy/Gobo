using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Document : GmlSyntaxNode
    {
        public GmlSyntaxNode Body { get; set; }

        public Document(ParserRuleContext context, GmlSyntaxNode body)
            : base(context)
        {
            Body = AsChild(body);
        }

        public override Doc Print()
        {
            return Doc.Concat(PrintHelper.PrintStatements(Body), Doc.HardLineIfNoPreviousLine);
        }
    }
}
