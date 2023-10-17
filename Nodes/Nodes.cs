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
            CommonTokenStream tokenStream,
            GmlSyntaxNode id,
            GmlSyntaxNode parameters,
            GmlSyntaxNode body,
            GmlSyntaxNode parent
        )
            : base(context, tokenStream)
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
            CommonTokenStream tokenStream,
            GmlSyntaxNode id,
            GmlSyntaxNode parameters
        )
            : base(context, tokenStream)
        {
            Id = AsChild(id);
            Parameters = AsChild(parameters);
        }
    }

    internal class Parameter : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Initializer { get; set; }

        public Parameter(
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
    }

    internal class StructExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Properties { get; set; }

        public StructExpression(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode properties
        )
            : base(context, tokenStream)
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
            CommonTokenStream tokenStream,
            GmlSyntaxNode name,
            GmlSyntaxNode initializer
        )
            : base(context, tokenStream)
        {
            Name = AsChild(name);
            Initializer = AsChild(initializer);
        }
    }

    internal class EnumDeclaration : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Members { get; set; }

        public EnumDeclaration(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode name,
            GmlSyntaxNode members
        )
            : base(context, tokenStream)
        {
            Name = AsChild(name);
            Members = AsChild(members);
        }
    }

    internal class EnumMember : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Initializer { get; set; }

        public EnumMember(
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
    }

    internal class ParenthesizedExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Expression { get; set; }

        public ParenthesizedExpression(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode expression
        )
            : base(context, tokenStream)
        {
            Expression = AsChild(expression);
        }

        public override Doc Print()
        {
            // remove redundant parens
            while (Expression is ParenthesizedExpression other)
            {
                Expression = other.Expression;
            }
            return Doc.Group("(", Doc.Indent(Doc.SoftLine, Expression.Print()), Doc.SoftLine, ")");
        }
    }
}
