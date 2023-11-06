﻿using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class SwitchBlock : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Cases => Children;

        public SwitchBlock(ParserRuleContext context, List<GmlSyntaxNode> cases)
            : base(context)
        {
            AsChildren(cases);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Block.PrintInBlock(ctx, Doc.Join(Doc.HardLine, PrintChildren(ctx)), this);
        }
    }
}
