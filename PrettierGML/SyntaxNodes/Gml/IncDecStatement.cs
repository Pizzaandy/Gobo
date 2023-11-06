﻿using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class IncDecStatement : GmlSyntaxNode
    {
        public string Operator { get; set; }
        public GmlSyntaxNode Argument { get; set; }
        public bool IsPrefix { get; set; }

        public IncDecStatement(
            ParserRuleContext context,
            string @operator,
            GmlSyntaxNode argument,
            bool isPrefix
        )
            : base(context)
        {
            Operator = @operator;
            Argument = AsChild(argument);
            IsPrefix = isPrefix;
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            if (IsPrefix)
            {
                return Doc.Concat(Operator, Argument.Print(ctx));
            }
            else
            {
                return Doc.Concat(Argument.Print(ctx), Operator);
            }
        }
    }
}
