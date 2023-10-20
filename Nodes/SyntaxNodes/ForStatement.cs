using Antlr4.Runtime;

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
            CommonTokenStream tokenStream,
            GmlSyntaxNode init,
            GmlSyntaxNode test,
            GmlSyntaxNode update,
            GmlSyntaxNode body
        )
            : base(context, tokenStream)
        {
            Init = AsChild(init);
            Test = AsChild(test);
            Update = AsChild(update);
            Body = AsChild(body);
        }

        public override Doc Print()
        {
            var separator = Doc.Concat(";", Doc.Line);
            var items = new Doc[] { Init.Print(), Test.Print(), Update.Print() };
            return Doc.Concat(
                "for (",
                Doc.Group(
                    Doc.Indent(Doc.IfBreak(Doc.Line, Doc.Null), Doc.Join(separator, items)),
                    Doc.Line
                ),
                ") ",
                PrintHelper.EnsureStatementInBlock(Body)
            );
        }
    }
}
