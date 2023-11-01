using Antlr4.Runtime;
using PrettierGML.Printer.Document.DocTypes;
using PrettierGML.Nodes.PrintHelpers;
using PrettierGML.Printer;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ForStatement : GmlSyntaxNode
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

        public override Doc Print(PrintContext ctx)
        {
            var separator = Doc.Concat(";", Doc.Line);
            var items = new Doc[] { Init.Print(ctx), Test.Print(ctx), Update.Print(ctx) };
            return Doc.Concat(
                "for (",
                Doc.Group(
                    Doc.Indent(Doc.IfBreak(Doc.Line, Doc.Null), Doc.Join(separator, items)),
                    Doc.IfBreak(Doc.Line, Doc.Null)
                ),
                ") ",
                Statement.EnsureStatementInBlock(ctx, Body)
            );
        }
    }
}
