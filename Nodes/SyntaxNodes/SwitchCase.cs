using Antlr4.Runtime;
using PrettierGML.Nodes.PrintHelpers;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class SwitchCase : GmlSyntaxNode
    {
        public GmlSyntaxNode Test { get; set; }
        public List<GmlSyntaxNode> Body { get; set; }

        public SwitchCase(ParserRuleContext context, GmlSyntaxNode test, List<GmlSyntaxNode> body)
            : base(context)
        {
            Test = AsChild(test);
            Body = AsChildren(body);
        }

        public override Doc Print(PrintContext ctx)
        {
            var caseText = Test.IsEmpty ? "default" : "case" + " ";
            var parts = new List<Doc>() { caseText, Test.Print(ctx), ":" };
            if (Body.Count > 0)
            {
                parts.Add(Doc.Indent(Doc.HardLine, Statement.PrintStatements(ctx, Body)));
            }
            return Doc.Concat(parts);
        }
    }
}
