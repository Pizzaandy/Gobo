﻿using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class TryStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Body { get; set; }
        public GmlSyntaxNode Catch { get; set; }
        public GmlSyntaxNode Finally { get; set; }

        public TryStatement(
            ParserRuleContext context,
            GmlSyntaxNode body,
            GmlSyntaxNode @catch,
            GmlSyntaxNode alternate
        )
            : base(context)
        {
            Body = AsChild(body);
            Catch = AsChild(@catch);
            Finally = AsChild(alternate);
        }

        public override Doc Print(PrintContext ctx)
        {
            var parts = new List<Doc>
            {
                Doc.Concat("try", " ", PrintHelper.EnsureStatementInBlock(ctx, Body))
            };

            if (!Catch.IsEmpty)
            {
                parts.Add(Doc.Concat(" ", Catch.Print(ctx)));
            }

            if (!Finally.IsEmpty)
            {
                parts.Add(Doc.Concat(" ", Finally.Print(ctx)));
            }

            return Doc.Concat(parts);
        }
    }
}