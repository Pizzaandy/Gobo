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
            // Macro identifiers can't have leading comments
            Id.PrintOwnComments = false;

            var printed = Doc.Concat("#macro", " ", Id.Print(ctx), " ", Body);

            return Doc.Concat(
                Id.PrintLeadingComments(ctx),
                Id.PrintTrailingComments(ctx, CommentType.Leading),
                Doc.HardLineIfNoPreviousLine,
                printed
            );
        }

        public override Doc PrintWithOwnComments(PrintContext ctx, Doc nodeDoc)
        {
            return Doc.Concat(
                PrintLeadingComments(ctx),
                PrintDanglingComments(ctx, CommentType.Leading),
                nodeDoc,
                Doc.HardLine,
                PrintTrailingComments(ctx, CommentType.Leading)
            );
        }
    }
}
