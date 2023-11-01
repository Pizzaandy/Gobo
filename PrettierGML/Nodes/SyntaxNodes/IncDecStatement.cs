using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class IncDecStatement : GmlSyntaxNode
    {
        public string Operator { get; set; }
        public GmlSyntaxNode Argument { get; set; }
        public bool IsPrefix { get; set; }

        public IncDecStatement(
            ParserRuleContext context,
            string @operator,
            GmlSyntaxNode argument,
            bool isPrefix
        )
            : base(context)
        {
            Operator = @operator;
            Argument = AsChild(argument);
            IsPrefix = isPrefix;
        }

        public override Doc Print(PrintContext ctx)
        {
            if (IsPrefix)
            {
                return Doc.Concat(Operator, Argument.Print(ctx));
            }
            else
            {
                return Doc.Concat(Argument.Print(ctx), Operator);
            }
        }
    }
}
