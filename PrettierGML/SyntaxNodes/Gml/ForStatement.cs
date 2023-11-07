using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class ForStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Init { get; set; }
        public GmlSyntaxNode Test { get; set; }
        public GmlSyntaxNode Update { get; set; }
        public GmlSyntaxNode Body { get; set; }

        public ForStatement(
            ParserRuleContext context,
            GmlSyntaxNode init,
            GmlSyntaxNode test,
            GmlSyntaxNode update,
            GmlSyntaxNode body
        )
            : base(context)
        {
            Init = AsChild(init);
            Test = AsChild(test);
            Update = AsChild(update);
            Body = AsChild(body);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            var items = new Doc[]
            {
                Init.Print(ctx),
                ";",
                Init.IsEmpty ? Doc.Null : Doc.Line,
                Test.Print(ctx),
                ";",
                Test.IsEmpty ? Doc.Null : Doc.Line,
                Update.Print(ctx)
            };

            return Doc.Concat(
                "for",
                " ",
                "(",
                Doc.Group(
                    Doc.Indent(Doc.IfBreak(Doc.Line, Doc.Null), Doc.Concat(items)),
                    Doc.IfBreak(Doc.Line, Doc.Null)
                ),
                ") ",
                Statement.EnsureStatementInBlock(ctx, Body)
            );
        }
    }
}
