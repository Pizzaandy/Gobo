﻿using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class CatchProduction : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public GmlSyntaxNode Body { get; set; }

        public CatchProduction(ParserRuleContext context, GmlSyntaxNode id, GmlSyntaxNode body)
            : base(context)
        {
            Id = AsChild(id);
            Body = AsChild(body);
        }

        public override Doc Print(PrintContext ctx)
        {
            if (Id.IsEmpty)
            {
                return Doc.Concat("catch", " ", PrintHelper.EnsureStatementInBlock(ctx, Body));
            }
            else
            {
                return PrintHelper.PrintSingleClauseStatement(ctx, "catch", Id, Body);
            }
        }
    }
}