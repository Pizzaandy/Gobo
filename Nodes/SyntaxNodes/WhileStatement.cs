﻿using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class WhileStatement : GmlSyntaxNode
    {
        public GmlSyntaxNode Test { get; set; }
        public GmlSyntaxNode Body { get; set; }

        public WhileStatement(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode test,
            GmlSyntaxNode body
        )
            : base(context, tokenStream)
        {
            Test = AsChild(test);
            Body = AsChild(body);
        }

        public override Doc Print()
        {
            return PrintHelper.PrintSingleClauseStatement("while", Test, Body);
        }
    }
}