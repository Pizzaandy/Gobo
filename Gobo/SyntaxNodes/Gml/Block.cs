using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class Block : GmlSyntaxNode
{
    public GmlSyntaxNode[] Statements => Children;
    public static Doc EmptyBlock => "{}";

    public Block(TextSpan span, GmlSyntaxNode[] body)
        : base(span)
    {
        Children = body;
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        if (Children.Length == 0)
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
    /// Adds a line break in front of the block depending on brace style.
    /// </summary>
    public static Doc WrapInBlock(PrintContext ctx, Doc bodyDoc)
    {
        Doc leadingWhitespace =
            ctx.Options.BraceStyle is BraceStyle.NewLine ? Doc.HardLineIfNoPreviousLine : Doc.Null;

        return Doc.Concat(
            leadingWhitespace,
            "{",
            Doc.Indent(Doc.HardLine, bodyDoc),
            Doc.HardLine,
            "}"
        );
    }

    /// <summary>
    /// Print an empty block statement.
    /// If any dangling comments exist on danglingCommentSource, they are printed inside the block.
    /// </summary>
    public static Doc PrintEmptyBlock(PrintContext ctx, GmlSyntaxNode? danglingCommentSource = null)
    {
        if (danglingCommentSource is null || !danglingCommentSource.DanglingComments.Any())
        {
            return EmptyBlock;
        }
        else
        {
            return WrapInBlock(ctx, danglingCommentSource.PrintDanglingComments(ctx));
        }
    }

    public override int GetHashCode()
    {
        if (Children.Length == 1)
        {
            return Children.First().GetHashCode();
        }
        else
        {
            return base.GetHashCode();
        }
    }
}
