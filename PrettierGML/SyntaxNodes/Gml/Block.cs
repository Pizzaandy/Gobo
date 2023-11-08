using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class Block : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Statements => Children;

        public Block(ParserRuleContext context, List<GmlSyntaxNode> body)
            : base(context)
        {
            AsChildren(body);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            if (Children.Count == 0)
            {
                return PrintEmptyBlock(ctx, this);
            }
            else
            {
                return WrapInBlock(ctx, Statement.PrintStatements(ctx, Children));
            }
        }

        /// <summary>
        /// Wraps a doc in brackets and line breaks.
        /// Adds whitespace in front of the block depending on brace style.
        /// </summary>
        public static Doc WrapInBlock(PrintContext ctx, Doc bodyDoc)
        {
            Doc leadingWhitespace =
                ctx.Options.BraceStyle is BraceStyle.NewLine
                    ? Doc.HardLineIfNoPreviousLine
                    : Doc.Null;

            return Doc.Concat(
                leadingWhitespace,
                "{",
                Doc.Indent(Doc.HardLine, bodyDoc),
                Doc.HardLine,
                "}"
            );
        }

        /// <summary>
        /// If any dangling comments exist on danglingCommentSource, they are printed inside the block.
        /// </summary>
        public static Doc PrintEmptyBlock(
            PrintContext ctx,
            GmlSyntaxNode? danglingCommentSource = null
        )
        {
            if (danglingCommentSource is null || danglingCommentSource.DanglingComments.Count == 0)
            {
                return EmptyBlock;
            }
            else
            {
                return WrapInBlock(ctx, danglingCommentSource.PrintDanglingComments(ctx));
            }
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
