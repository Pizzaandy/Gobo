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

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Concat(Statement.PrintStatements(ctx, Body), Doc.HardLineIfNoPreviousLine);
        }
    }
}
