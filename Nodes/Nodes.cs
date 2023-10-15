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

    internal class MemberIndexExpression : GmlSyntaxNode, IHasObject
    {
        public GmlSyntaxNode Object { get; set; }
        public GmlSyntaxNode Property { get; set; }
        public string Accessor { get; set; }

        public MemberIndexExpression(
            ParserRuleContext context,
            GmlSyntaxNode @object,
            GmlSyntaxNode property,
            string accessor
        )
            : base(context)
        {
            Object = AsChild(@object);
            Property = AsChild(property);
            Accessor = accessor;
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

    internal class MacroDeclaration : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public string Text { get; set; }

        public MacroDeclaration(ParserRuleContext context, GmlSyntaxNode name, string text)
            : base(context)
        {
            Name = AsChild(name);
            Text = text;
        }
    }

    internal class ConditionalExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Test { get; set; }
        public GmlSyntaxNode WhenTrue { get; set; }
        public GmlSyntaxNode WhenFalse { get; set; }

        public ConditionalExpression(
            ParserRuleContext context,
            GmlSyntaxNode test,
            GmlSyntaxNode whenTrue,
            GmlSyntaxNode whenFalse
        )
            : base(context)
        {
            Test = AsChild(test);
            WhenTrue = AsChild(whenTrue);
            WhenFalse = AsChild(whenFalse);
        }
    }

    internal class UnaryExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Operator { get; set; }
        public GmlSyntaxNode Argument { get; set; }

        public UnaryExpression(
            ParserRuleContext context,
            GmlSyntaxNode @operator,
            GmlSyntaxNode argument
        )
            : base(context)
        {
            Operator = @operator;
            Argument = argument;
        }
    }
}
