using Antlr4.Runtime;
using PrettierGML.Printer.Document.DocTypes;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class TryStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Body { get; set; }
        public GmlSyntaxNode Catch { get; set; }
        public GmlSyntaxNode Finally { get; set; }

        public TryStatement(
            ParserRuleContext context,
            GmlSyntaxNode body,
            GmlSyntaxNode @catch,
            GmlSyntaxNode alternate
        )
            : base(context)
        {
            Body = AsChild(body);
            Catch = AsChild(@catch);
            Finally = AsChild(alternate);
        }

        public override Doc Print(PrintContext ctx)
        {
            var parts = new List<Doc>
            {
                Doc.Concat("try", " ", Statement.EnsureStatementInBlock(ctx, Body))
            };

            Doc leadingWhitespace =
                ctx.Options.BraceStyle == BraceStyle.NewLine ? Doc.HardLine : " ";

            if (!Catch.IsEmpty)
            {
                parts.Add(Doc.Concat(leadingWhitespace, Catch.Print(ctx)));
            }

            if (!Finally.IsEmpty)
            {
                parts.Add(Doc.Concat(leadingWhitespace, Finally.Print(ctx)));
            }

            return Doc.Concat(parts);
        }
    }
}
