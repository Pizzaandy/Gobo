using Antlr4.Runtime;
using System.Diagnostics;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Block : GmlSyntaxNode
    {
        public GmlSyntaxNode Body { get; set; }

        public Block(ParserRuleContext context, GmlSyntaxNode body)
            : base(context)
        {
            Body = AsChild(body);
            Debug.Assert(Body is NodeList or EmptyNode);
        }

        public override Doc Print(PrintContext ctx)
        {
            return PrintInBlock(ctx, Statement.PrintStatements(ctx, Body));
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
            if (!Body.IsEmpty && Body.Children.Count == 1)
            {
                return Body.Children.First().GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }
    }
}
