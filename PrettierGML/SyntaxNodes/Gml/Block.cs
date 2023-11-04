using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class Block : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Statements => Children;

        public Block(ParserRuleContext context, List<GmlSyntaxNode> body)
            : base(context)
        {
            AsChildren(body);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return PrintInBlock(ctx, Statement.PrintStatements(ctx, Children));
        }

        public static Doc PrintInBlock(PrintContext ctx, Doc bodyDoc)
        {
            if (bodyDoc == Doc.Null)
            {
                return EmptyBlock;
            }

            Doc leadingLine =
                ctx.Options.BraceStyle == BraceStyle.NewLine ? Doc.HardLine : Doc.Null;

            return Doc.Concat(
                leadingLine,
                "{",
                Doc.Indent(Doc.HardLine, bodyDoc),
                Doc.HardLine,
                "}"
            );
        }

        public static Doc EmptyBlock => "{}";

        public override int GetHashCode()
        {
            if (Children.Count == 1)
            {
                return Children.First().GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }
    }
}
