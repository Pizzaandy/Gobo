using Antlr4.Runtime;

namespace PrettierGML.Nodes
{
    internal class FunctionDeclaration : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public GmlSyntaxNode Parameters { get; set; }
        public GmlSyntaxNode Body { get; set; }
        public GmlSyntaxNode ConstructorParent { get; set; }

        public FunctionDeclaration(
            ParserRuleContext context,
            GmlSyntaxNode id,
            GmlSyntaxNode parameters,
            GmlSyntaxNode body,
            GmlSyntaxNode parent
        )
            : base(context)
        {
            Id = AsChild(id);
            Parameters = AsChild(parameters);
            Body = AsChild(body);
            ConstructorParent = AsChild(parent);
        }
    }

    internal class ConstructorClause : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public GmlSyntaxNode Parameters { get; set; }

        public ConstructorClause(
            ParserRuleContext context,
            GmlSyntaxNode id,
            GmlSyntaxNode parameters
        )
            : base(context)
        {
            Id = AsChild(id);
            Parameters = AsChild(parameters);
        }
    }

    internal class Parameter : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Initializer { get; set; }

        public Parameter(ParserRuleContext context, GmlSyntaxNode name, GmlSyntaxNode initializer)
            : base(context)
        {
            Name = AsChild(name);
            Initializer = AsChild(initializer);
        }
    }

    internal class StructExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Properties { get; set; }

        public StructExpression(ParserRuleContext context, GmlSyntaxNode properties)
            : base(context)
        {
            Properties = AsChild(properties);
        }
    }

    internal class StructProperty : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Initializer { get; set; }

        public StructProperty(
            ParserRuleContext context,
            GmlSyntaxNode name,
            GmlSyntaxNode initializer
        )
            : base(context)
        {
            Name = AsChild(name);
            Initializer = AsChild(initializer);
        }
    }

    internal class EnumDeclaration : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Members { get; set; }

        public EnumDeclaration(ParserRuleContext context, GmlSyntaxNode name, GmlSyntaxNode members)
            : base(context)
        {
            Name = AsChild(name);
            Members = AsChild(members);
        }
    }

    internal class EnumMember : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Initializer { get; set; }

        public EnumMember(ParserRuleContext context, GmlSyntaxNode name, GmlSyntaxNode initializer)
            : base(context)
        {
            Name = AsChild(name);
            Initializer = AsChild(initializer);
        }
    }
}
