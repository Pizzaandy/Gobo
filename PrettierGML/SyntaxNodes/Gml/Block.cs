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
            return PrintInBlock(ctx, Statement.PrintStatements(ctx, Children), this);
        }

        /// <summary>
        /// Wraps a doc in brackets and line breaks.
        /// If any dangling comments exist on danglingCommentSource, they are printed inside the block.
        /// </summary>
        public static Doc PrintInBlock(
            PrintContext ctx,
            Doc bodyDoc,
            GmlSyntaxNode? danglingCommentSource = null
        )
        {
            var hasDanglingComments =
                danglingCommentSource is not null && danglingCommentSource.DanglingComments.Any();

            if (bodyDoc == Doc.Null && !hasDanglingComments)
            {
                return EmptyBlock;
            }

            Doc printedComments = Doc.Null;

            if (hasDanglingComments)
            {
                danglingCommentSource!.PrintOwnComments = false;
                printedComments = danglingCommentSource.PrintDanglingComments(ctx);
            }

            return Doc.Concat(
                "{",
                Doc.Indent(Doc.HardLine, printedComments, bodyDoc),
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
