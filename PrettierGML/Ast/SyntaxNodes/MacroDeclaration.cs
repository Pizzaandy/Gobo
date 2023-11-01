using Antlr4.Runtime;
using PrettierGML.Printer;
using PrettierGML.Printer.Docs.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class MacroDeclaration : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public string Body { get; set; }

        public MacroDeclaration(ParserRuleContext context, GmlSyntaxNode id, string body)
            : base(context)
        {
            Id = AsChild(id);
            Body = body;
        }

        public override Doc Print(PrintContext ctx)
        {
            return Doc.Concat("#macro", " ", Id.Print(ctx), " ", Body);
        }
    }
}
