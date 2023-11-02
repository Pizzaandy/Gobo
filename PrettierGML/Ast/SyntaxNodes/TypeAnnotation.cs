using Antlr4.Runtime;
using PrettierGML.Printer;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class TypeAnnotation : GmlSyntaxNode
    {
        public List<string> Types { get; set; }

        public TypeAnnotation(ParserRuleContext context, List<string> types)
            : base(context)
        {
            Types = types;
        }

        public override Doc Print(PrintContext ctx)
        {
            if (ctx.Options.RemoveSyntaxExtensions)
            {
                return Doc.Null;
            }

            var parts = new List<Doc>();
            foreach (var typeName in Types)
            {
                parts.Add(typeName);
            }
            return Doc.Concat(":", " ", Doc.Join(" | ", parts));
        }
    }
}
