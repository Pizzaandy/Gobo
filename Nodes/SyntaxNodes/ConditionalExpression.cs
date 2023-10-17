using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class ConditionalExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Test { get; set; }
        public GmlSyntaxNode WhenTrue { get; set; }
        public GmlSyntaxNode WhenFalse { get; set; }

        public ConditionalExpression(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode test,
            GmlSyntaxNode whenTrue,
            GmlSyntaxNode whenFalse
        )
            : base(context, tokenStream)
        {
            Test = AsChild(test);
            WhenTrue = AsChild(whenTrue);
            WhenFalse = AsChild(whenFalse);
        }

        public override Doc Print()
        {
            Doc[] innerContents =
            {
                Doc.Line,
                "? ",
                Doc.Concat(WhenTrue.Print()),
                Doc.Line,
                ": ",
                Doc.Concat(WhenFalse.Print())
            };

            Doc[] outerContents =
            {
                Parent is ConditionalExpression ? Doc.BreakParent : Doc.Null,
                Parent is ReturnStatement && Test is BinaryExpression
                    ? Doc.Indent(Doc.Group(Doc.IfBreak(Doc.SoftLine, Doc.Null), Test.Print()))
                    : Test.Print(),
                Parent is ConditionalExpression or ReturnStatement
                    ? Doc.Indent(innerContents)
                    : Doc.Indent(innerContents)
            };

            return Parent is ConditionalExpression
                ? Doc.Concat(outerContents)
                : Doc.Group(outerContents);
        }
    }
}
