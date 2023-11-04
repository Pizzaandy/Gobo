using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class TemplateLiteral : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Parts => Children;

        public TemplateLiteral(ParserRuleContext context, List<GmlSyntaxNode> atoms)
            : base(context)
        {
            Children = AsChildren(atoms);
        }

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Concat("$\"", Doc.Concat(PrintChildren(ctx)), "\"");
        }
    }
}
