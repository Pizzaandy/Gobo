﻿using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class Argument : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Initializer { get; set; }

        public Argument(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode name,
            GmlSyntaxNode initializer
        )
            : base(context, tokenStream)
        {
            Name = AsChild(name);
            Initializer = AsChild(initializer);
        }

        public override Doc Print()
        {
            if (Initializer.IsEmpty)
            {
                return Name.Print();
            }
            else
            {
                return Doc.Concat(Name.Print(), " = ", Initializer.Print());
            }
        }
    }
}