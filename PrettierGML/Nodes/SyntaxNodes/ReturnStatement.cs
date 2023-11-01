using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ReturnStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Argument { get; set; }

        public ReturnStatement(ParserRuleContext context, GmlSyntaxNode argument)
            : base(context)
        {
            Argument = AsChild(argument);
        }

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Concat("return", Argument.IsEmpty ? "" : " ", Argument.Print(ctx));
        }
    }
}
