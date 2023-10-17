using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class WithStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Object { get; set; }
        public GmlSyntaxNode Body { get; set; }

        public WithStatement(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode @object,
            GmlSyntaxNode body
        )
            : base(context, tokenStream)
        {
            Object = AsChild(@object);
            Body = AsChild(body);
        }

        public override Doc Print()
        {
            return PrintHelper.PrintSingleClauseStatement("repeat", Object, Body);
        }
    }
}
