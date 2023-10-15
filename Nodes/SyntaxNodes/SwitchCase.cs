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

        public override Doc Print()
        {
            var caseText = Test.IsEmpty ? "default" : "case ";
            var parts = new List<Doc>() { Doc.HardLine, caseText, Test.Print(), ":" };
            if (!Body.IsEmpty)
            {
                parts.Add(Doc.Indent(Doc.HardLine, PrintHelper.PrintStatements(Body)));
            }
            return Doc.Concat(parts);
        }
    }
}
