using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class ConditionalExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Test { get; set; }
        public GmlSyntaxNode WhenTrue { get; set; }
        public GmlSyntaxNode WhenFalse { get; set; }

        public ConditionalExpression(
            TextSpan span,
            GmlSyntaxNode test,
            GmlSyntaxNode whenTrue,
            GmlSyntaxNode whenFalse
        )
            : base(span)
        {
            Test = AsChild(test);
            WhenTrue = AsChild(whenTrue);
            WhenFalse = AsChild(whenFalse);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            Doc[] innerContents =
            {
                Doc.Line,
                "?",
                " ",
                Doc.Concat(WhenTrue.Print(ctx)),
                Doc.Line,
                ":",
                " ",
                Doc.Concat(WhenFalse.Print(ctx))
            };

            Doc[] outerContents =
            {
                Parent is ConditionalExpression ? Doc.BreakParent : Doc.Null,
                Parent is ReturnStatement && Test is BinaryExpression
                    ? Doc.Indent(Doc.Group(Doc.IfBreak(Doc.SoftLine, Doc.Null), Test.Print(ctx)))
                    : Test.Print(ctx),
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
