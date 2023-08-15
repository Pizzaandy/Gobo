using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace PrettierGML
{
    public abstract class GMLSyntaxNode
    {
        public int StartIndex { get; set; } = -1;
        public int EndIndex { get; set; } = -1;

        public string Type => GetType().Name;

        public GMLSyntaxNode() { }

        public GMLSyntaxNode(ISyntaxTree context)
        {
            StartIndex = context.SourceInterval.a;
            EndIndex = context.SourceInterval.b;
        }

        //public GMLSyntaxNode(ITerminalNode context)
        //{
        //    StartIndex = context.SourceInterval.a;
        //    EndIndex = context.SourceInterval.b;
        //}

        public static EmptyNode Empty => EmptyNode.Instance;

        public static NodeList List(ISyntaxTree context, IList<GMLSyntaxNode> contents) =>
            new(context, contents);

        internal virtual Doc Print()
        {
            return Doc.Null;
        }
    }

    public interface IHasObject
    {
        public GMLSyntaxNode Object { get; set; }
    }

    public class EmptyNode : GMLSyntaxNode
    {
        public static EmptyNode Instance { get; } = new();

        private EmptyNode() { }
    }

    public class NodeList : GMLSyntaxNode
    {
        public IList<GMLSyntaxNode> Contents { get; set; }

        public NodeList(ISyntaxTree context, IList<GMLSyntaxNode> contents)
            : base(context)
        {
            Contents = contents;
        }
    }

    public class Program : GMLSyntaxNode
    {
        public GMLSyntaxNode Body { get; set; }

        public Program(ParserRuleContext context, GMLSyntaxNode body)
            : base(context)
        {
            Body = body;
        }
    }

    public class Block : GMLSyntaxNode
    {
        public GMLSyntaxNode Body { get; set; }

        public Block(ParserRuleContext context, GMLSyntaxNode body)
            : base(context)
        {
            Body = body;
        }
    }

    public class IfStatement : GMLSyntaxNode
    {
        public GMLSyntaxNode Test { get; set; }
        public GMLSyntaxNode Consequent { get; set; }
        public GMLSyntaxNode Alternate { get; set; }

        public IfStatement(
            ParserRuleContext context,
            GMLSyntaxNode test,
            GMLSyntaxNode consequent,
            GMLSyntaxNode alternate
        )
            : base(context)
        {
            Test = test;
            Consequent = consequent;
            Alternate = alternate;
        }
    }

    public class DoStatement : GMLSyntaxNode
    {
        public GMLSyntaxNode Body { get; set; }
        public GMLSyntaxNode Test { get; set; }

        public DoStatement(ParserRuleContext context, GMLSyntaxNode body, GMLSyntaxNode test)
            : base(context)
        {
            Body = body;
            Test = test;
        }
    }

    public class WhileStatement : GMLSyntaxNode
    {
        public GMLSyntaxNode Test { get; set; }
        public GMLSyntaxNode Body { get; set; }

        public WhileStatement(ParserRuleContext context, GMLSyntaxNode test, GMLSyntaxNode body)
            : base(context)
        {
            Test = test;
            Body = body;
        }
    }

    public class ForStatement : GMLSyntaxNode
    {
        public GMLSyntaxNode Init { get; set; }
        public GMLSyntaxNode Test { get; set; }
        public GMLSyntaxNode Update { get; set; }
        public GMLSyntaxNode Body { get; set; }

        public ForStatement(
            ParserRuleContext context,
            GMLSyntaxNode init,
            GMLSyntaxNode test,
            GMLSyntaxNode update,
            GMLSyntaxNode body
        )
            : base(context)
        {
            Init = init;
            Test = test;
            Update = update;
            Body = body;
        }
    }

    public class RepeatStatement : GMLSyntaxNode
    {
        public GMLSyntaxNode Test { get; set; }
        public GMLSyntaxNode Body { get; set; }

        public RepeatStatement(ParserRuleContext context, GMLSyntaxNode test, GMLSyntaxNode body)
            : base(context)
        {
            Test = test;
            Body = body;
        }
    }

    public class WithStatement : GMLSyntaxNode
    {
        public GMLSyntaxNode Object { get; set; }
        public GMLSyntaxNode Body { get; set; }

        public WithStatement(ParserRuleContext context, GMLSyntaxNode @object, GMLSyntaxNode body)
            : base(context)
        {
            Object = @object;
            Body = body;
        }
    }

    public class SwitchStatement : GMLSyntaxNode
    {
        public GMLSyntaxNode Discriminant { get; set; }
        public GMLSyntaxNode Cases { get; set; }

        public SwitchStatement(
            ParserRuleContext context,
            GMLSyntaxNode discriminant,
            GMLSyntaxNode cases
        )
            : base(context)
        {
            Discriminant = discriminant;
            Cases = cases;
        }
    }

    public class SwitchCase : GMLSyntaxNode
    {
        public GMLSyntaxNode Test { get; set; }
        public GMLSyntaxNode Body { get; set; }

        public SwitchCase(ParserRuleContext context, GMLSyntaxNode test, GMLSyntaxNode body)
            : base(context)
        {
            Test = test;
            Body = body;
        }
    }

    public class ContinueStatement : GMLSyntaxNode
    {
        public ContinueStatement(ParserRuleContext context)
            : base(context) { }
    }

    public class BreakStatement : GMLSyntaxNode
    {
        public BreakStatement(ParserRuleContext context)
            : base(context) { }
    }

    public class ExitStatement : GMLSyntaxNode
    {
        public ExitStatement(ParserRuleContext context)
            : base(context) { }
    }

    public class DefineStatement : GMLSyntaxNode
    {
        public string Name { get; set; }

        public DefineStatement(ParserRuleContext context, string name)
            : base(context)
        {
            Name = name;
        }
    }

    public class RegionStatement : GMLSyntaxNode
    {
        public string Name { get; set; }
        public bool IsEndRegion { get; set; }

        public RegionStatement(ParserRuleContext context, string name, bool isEndRegion)
            : base(context)
        {
            Name = name;
            IsEndRegion = isEndRegion;
        }
    }

    public class AssignmentExpression : GMLSyntaxNode
    {
        public string Operator { get; set; }
        public GMLSyntaxNode Left { get; set; }
        public GMLSyntaxNode Right { get; set; }

        public AssignmentExpression(
            ParserRuleContext context,
            string @operator,
            GMLSyntaxNode left,
            GMLSyntaxNode right
        )
            : base(context)
        {
            Operator = @operator;
            Left = left;
            Right = right;
        }
    }

    public class VariableDeclarationList : GMLSyntaxNode
    {
        public GMLSyntaxNode Declarations { get; set; }
        public string Kind { get; set; }

        public VariableDeclarationList(
            ParserRuleContext context,
            GMLSyntaxNode declarations,
            string kind
        )
            : base(context)
        {
            Declarations = declarations;
            Kind = kind;
        }
    }

    public class VariableDeclarator : GMLSyntaxNode
    {
        public GMLSyntaxNode Id { get; set; }
        public GMLSyntaxNode Initializer { get; set; }

        public VariableDeclarator(
            ParserRuleContext context,
            GMLSyntaxNode id,
            GMLSyntaxNode initializer
        )
            : base(context)
        {
            Id = id;
            Initializer = initializer;
        }
    }

    public class CallExpression : GMLSyntaxNode, IHasObject
    {
        public GMLSyntaxNode Object { get; set; }
        public GMLSyntaxNode Arguments { get; set; }

        public CallExpression(
            ParserRuleContext context,
            GMLSyntaxNode @object,
            GMLSyntaxNode arguments
        )
            : base(context)
        {
            Object = @object;
            Arguments = arguments;
        }
    }

    public class NewExpression : GMLSyntaxNode
    {
        public GMLSyntaxNode Name { get; set; }
        public GMLSyntaxNode Arguments { get; set; }

        public NewExpression(ParserRuleContext context, GMLSyntaxNode name, GMLSyntaxNode arguments)
            : base(context)
        {
            Name = name;
            Arguments = arguments;
        }
    }

    public class FunctionDeclaration : GMLSyntaxNode
    {
        public GMLSyntaxNode Id { get; set; }
        public GMLSyntaxNode Parameters { get; set; }
        public GMLSyntaxNode Body { get; set; }
        public GMLSyntaxNode Parent { get; set; }

        public FunctionDeclaration(
            ParserRuleContext context,
            GMLSyntaxNode id,
            GMLSyntaxNode parameters,
            GMLSyntaxNode body,
            GMLSyntaxNode parent
        )
            : base(context)
        {
            Id = id;
            Parameters = parameters;
            Body = body;
            Parent = parent;
        }
    }

    public class ConstructorClause : GMLSyntaxNode
    {
        public GMLSyntaxNode Id { get; set; }
        public GMLSyntaxNode Parameters { get; set; }

        public ConstructorClause(
            ParserRuleContext context,
            GMLSyntaxNode id,
            GMLSyntaxNode parameters
        )
            : base(context)
        {
            Id = id;
            Parameters = parameters;
        }
    }

    public class Parameter : GMLSyntaxNode
    {
        public GMLSyntaxNode Name { get; set; }
        public GMLSyntaxNode Initializer { get; set; }

        public Parameter(ParserRuleContext context, GMLSyntaxNode name, GMLSyntaxNode initializer)
            : base(context)
        {
            Name = name;
            Initializer = initializer;
        }
    }

    public class MemberIndexExpression : GMLSyntaxNode, IHasObject
    {
        public GMLSyntaxNode Object { get; set; }
        public GMLSyntaxNode Property { get; set; }
        public string Accessor { get; set; }

        public MemberIndexExpression(
            ParserRuleContext context,
            GMLSyntaxNode @object,
            GMLSyntaxNode property,
            string accessor
        )
            : base(context)
        {
            Object = @object;
            Property = property;
            Accessor = accessor;
        }
    }

    public class MemberDotExpression : GMLSyntaxNode, IHasObject
    {
        public GMLSyntaxNode Object { get; set; }
        public GMLSyntaxNode Property { get; set; }

        public MemberDotExpression(
            ParserRuleContext context,
            GMLSyntaxNode @object,
            GMLSyntaxNode property
        )
            : base(context)
        {
            Object = @object;
            Property = property;
        }
    }

    public class UnaryExpression : GMLSyntaxNode
    {
        public GMLSyntaxNode Operator { get; set; }
        public GMLSyntaxNode Argument { get; set; }
        public bool IsPrefix { get; set; }

        public UnaryExpression(
            ParserRuleContext context,
            GMLSyntaxNode @operator,
            GMLSyntaxNode argument,
            bool isPrefix
        )
            : base(context)
        {
            Operator = @operator;
            Argument = argument;
            IsPrefix = isPrefix;
        }
    }

    public class BinaryExpression : GMLSyntaxNode
    {
        public GMLSyntaxNode Operator { get; set; }
        public GMLSyntaxNode Left { get; set; }
        public GMLSyntaxNode Right { get; set; }

        public BinaryExpression(
            ParserRuleContext context,
            GMLSyntaxNode @operator,
            GMLSyntaxNode left,
            GMLSyntaxNode right
        )
            : base(context)
        {
            Operator = @operator;
            Left = left;
            Right = right;
        }
    }

    public class Literal : GMLSyntaxNode
    {
        public string Text { get; set; }

        public Literal(ParserRuleContext context, string text)
            : base(context)
        {
            Text = text;
        }
    }

    public class Identifier : GMLSyntaxNode
    {
        public string Name { get; set; }

        public Identifier(ISyntaxTree context, string name)
            : base(context)
        {
            Name = name;
        }
    }

    public class ArrayExpression : GMLSyntaxNode
    {
        public GMLSyntaxNode Elements { get; set; }

        public ArrayExpression(ParserRuleContext context, GMLSyntaxNode elements)
            : base(context)
        {
            Elements = elements;
        }
    }

    public class StructExpression : GMLSyntaxNode
    {
        public GMLSyntaxNode Properties { get; set; }

        public StructExpression(ParserRuleContext context, GMLSyntaxNode properties)
            : base(context)
        {
            Properties = properties;
        }
    }

    public class StructProperty : GMLSyntaxNode
    {
        public GMLSyntaxNode Name { get; set; }
        public GMLSyntaxNode Initializer { get; set; }

        public StructProperty(
            ParserRuleContext context,
            GMLSyntaxNode name,
            GMLSyntaxNode initializer
        )
            : base(context)
        {
            Name = name;
            Initializer = initializer;
        }
    }

    public class EnumDeclaration : GMLSyntaxNode
    {
        public GMLSyntaxNode Name { get; set; }
        public GMLSyntaxNode Members { get; set; }

        public EnumDeclaration(ParserRuleContext context, GMLSyntaxNode name, GMLSyntaxNode members)
            : base(context)
        {
            Name = name;
            Members = members;
        }
    }

    public class EnumMember : GMLSyntaxNode
    {
        public GMLSyntaxNode Name { get; set; }
        public GMLSyntaxNode Initializer { get; set; }

        public EnumMember(ParserRuleContext context, GMLSyntaxNode name, GMLSyntaxNode initializer)
            : base(context)
        {
            Name = name;
            Initializer = initializer;
        }
    }

    public class MacroDeclaration : GMLSyntaxNode
    {
        public GMLSyntaxNode Name { get; set; }
        public string Text { get; set; }

        public MacroDeclaration(ParserRuleContext context, GMLSyntaxNode name, string text)
            : base(context)
        {
            Name = name;
            Text = text;
        }
    }
}
