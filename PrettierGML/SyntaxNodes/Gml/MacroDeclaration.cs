using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class MacroDeclaration : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public string Body { get; set; }

        public MacroDeclaration(ParserRuleContext context, GmlSyntaxNode id, string body)
            : base(context)
        {
            Id = AsChild(id);
            Body = body;
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Doc.Concat("#macro", " ", Id.Print(ctx), " ", Body);
        }
    }
}
