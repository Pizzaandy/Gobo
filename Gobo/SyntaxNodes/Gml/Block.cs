using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class Block : GmlSyntaxNode
{
    public List<GmlSyntaxNode> Statements => Children;
    public static Doc EmptyBlock => "{}";

    public Block(TextSpan span, List<GmlSyntaxNode> body)
        : base(span)
    {
        AsChildren(body);
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        if (Children.Count == 0)
        {
            return PrintEmptyBlock(ctx, this);
        }

        if (EndsWithGluedStatement(ctx))
        {
            var body = Children.Take(Children.Count - 1).ToList();
            var last = Children[^1];

            Doc leadingWhitespace =
                ctx.Options.BraceStyle is BraceStyle.SameLine
                    ? Doc.Null
                    : Doc.HardLineIfNoPreviousLine;

            return Doc.Concat(
                leadingWhitespace,
                "{",
                body.Count > 0
                    ? Doc.Indent(Doc.HardLine, Statement.PrintStatements(ctx, body))
                    : Doc.Null,
                Doc.HardLine,
                Statement.PrintStatement(ctx, last),
                "}"
            );
        }

        var allowInline =
            Statement.CanInlineBody(ctx, Parent, this, Children) && Comments.Count == 0;

        var bodyDoc = allowInline
            ? Statement.PrintStatementsInline(ctx, Children)
            : Statement.PrintStatements(ctx, Children);

        return WrapInBlock(ctx, bodyDoc, allowInline);
    }

    /// <summary>
    /// True when the last statement shares the closing brace's line, as in '_i += 1;}'.
    /// It is printed at the block's indentation rather than its body's.
    /// </summary>
    private bool EndsWithGluedStatement(PrintContext ctx)
    {
        if (!ctx.Options.PreserveGluedStatements || Children.Count == 0)
        {
            return false;
        }

        var last = Children[^1];

        if (last.Comments.Count > 0 || last.Span.End > Span.End)
        {
            return false;
        }

        // One line in the source, so there is no idiom to preserve.
        if (!ctx.SourceText.ReadSpan(Span.Start, last.Span.Start).Contains('\n'))
        {
            return false;
        }

        return !ctx.SourceText.ReadSpan(last.Span.End, Span.End).Contains('\n');
    }

    /// <summary>
    /// Wraps a doc in brackets and line breaks.
    /// Adds a line break in front of the block depending on brace style.
    /// </summary>
    public static Doc WrapInBlock(PrintContext ctx, Doc bodyDoc, bool allowInline = false)
    {
        if (ctx.Options.BraceStyle is BraceStyle.NewLineIndented)
        {
            return Doc.Indent(
                Doc.HardLineIfNoPreviousLine,
                "{",
                Doc.HardLine,
                bodyDoc,
                Doc.HardLine,
                "}"
            );
        }

        Doc leadingWhitespace =
            ctx.Options.BraceStyle is BraceStyle.NewLine ? Doc.HardLineIfNoPreviousLine : Doc.Null;

        if (allowInline)
        {
            return Doc.Group(
                leadingWhitespace,
                "{",
                Doc.Indent(Doc.Line, bodyDoc),
                Doc.Line,
                "}"
            );
        }

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
