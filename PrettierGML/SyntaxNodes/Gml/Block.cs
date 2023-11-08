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
                return WrapInBlock(Statement.PrintStatements(ctx, Children));
            }
        }

        /// <summary>
        /// Wraps a doc in brackets and line breaks.
        /// </summary>
        public static Doc WrapInBlock(Doc bodyDoc)
        {
            return Doc.Concat("{", Doc.Indent(Doc.HardLine, bodyDoc), Doc.HardLine, "}");
        }

        /// <summary>
        /// If any dangling comments exist on danglingCommentSource, they are printed inside the block.
        /// </summary>
        public static Doc PrintEmptyBlock(
            PrintContext ctx,
            GmlSyntaxNode? danglingCommentSource = null
        )
        {
            if (danglingCommentSource is null || danglingCommentSource.Comments.Count == 0)
            {
                return EmptyBlock;
            }
            else
            {
                return Doc.Group(
                    "{",
                    Doc.Indent(Doc.SoftLine, danglingCommentSource.PrintDanglingComments(ctx)),
                    Doc.SoftLine,
                    "}"
                );
            }
        }

        public static Doc EmptyBlock => "{}";

        public override Doc PrintWithOwnComments(PrintContext ctx, Doc nodeDoc)
        {
            // Print dangling comments as leading
            return Doc.Concat(
                PrintLeadingComments(ctx),
                PrintDanglingComments(ctx, CommentType.Leading),
                nodeDoc,
                PrintTrailingComments(ctx)
            );
        }

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
