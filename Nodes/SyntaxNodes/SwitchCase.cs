using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class SwitchCase : GmlSyntaxNode
    {
        public GmlSyntaxNode Test { get; set; }
        public GmlSyntaxNode Body { get; set; }

        public SwitchCase(ParserRuleContext context, GmlSyntaxNode test, GmlSyntaxNode body)
            : base(context)
        {
            Test = AsChild(test);
            Body = AsChild(body);
        }

        public override Doc Print(PrintContext ctx)
        {
            var caseText = Test.IsEmpty ? "default" : $"{"case"} ";
            return Doc.Concat(
                caseText,
                Test.Print(ctx),
                ":",
                Body.IsEmpty ? Doc.Null : Doc.Indent(Doc.HardLine, Body.Print(ctx))
            );
        }
    }
}
