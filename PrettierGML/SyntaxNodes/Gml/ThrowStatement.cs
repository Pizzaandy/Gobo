﻿using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class ThrowStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Expression { get; set; }

        public ThrowStatement(ParserRuleContext context, GmlSyntaxNode expression)
            : base(context)
        {
            Expression = AsChild(expression);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Doc.Concat("throw", " ", Expression.Print(ctx));
        }
    }
}
