namespace PrettierGML
{
    public abstract class GMLSyntaxNode
    {
        //public int StartIndex { get; set; }
        //public int EndIndex { get; set; }

        public string Type => GetType().Name;

        public static EmptyNode Empty => EmptyNode.Instance;

        public static NodeList List(params GMLSyntaxNode[] contents) => new NodeList(contents);

        public static NodeList List(IList<GMLSyntaxNode> contents) => new NodeList(contents);
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

        public NodeList(IList<GMLSyntaxNode> contents)
        {
            Contents = contents;
        }
    }

    public class Block : GMLSyntaxNode
    {
        public GMLSyntaxNode Body { get; set; }

        public Block(GMLSyntaxNode body)
        {
            Body = body;
        }
    }

    public class IfStatement : GMLSyntaxNode
    {
        public GMLSyntaxNode Test { get; set; }
        public GMLSyntaxNode Consequent { get; set; }
        public GMLSyntaxNode Alternate { get; set; }

        public IfStatement(GMLSyntaxNode test, GMLSyntaxNode consequent, GMLSyntaxNode alternate)
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

        public DoStatement(GMLSyntaxNode body, GMLSyntaxNode test)
        {
            Body = body;
            Test = test;
        }
    }

    public class WhileStatement : GMLSyntaxNode
    {
        public GMLSyntaxNode Test { get; set; }
        public GMLSyntaxNode Body { get; set; }

        public WhileStatement(GMLSyntaxNode test, GMLSyntaxNode body)
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
            GMLSyntaxNode init,
            GMLSyntaxNode test,
            GMLSyntaxNode update,
            GMLSyntaxNode body
        )
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

        public RepeatStatement(GMLSyntaxNode test, GMLSyntaxNode body)
        {
            Test = test;
            Body = body;
        }
    }

    public class WithStatement : GMLSyntaxNode
    {
        public GMLSyntaxNode Object { get; set; }
        public GMLSyntaxNode Body { get; set; }

        public WithStatement(GMLSyntaxNode @object, GMLSyntaxNode body)
        {
            Object = @object;
            Body = body;
        }
    }

    public class SwitchStatement : GMLSyntaxNode
    {
        public GMLSyntaxNode Discriminant { get; set; }
        public GMLSyntaxNode Cases { get; set; }

        public SwitchStatement(GMLSyntaxNode discriminant, GMLSyntaxNode cases)
        {
            Discriminant = discriminant;
            Cases = cases;
        }
    }

    public class SwitchCase : GMLSyntaxNode
    {
        public GMLSyntaxNode Test { get; set; }
        public GMLSyntaxNode Body { get; set; }

        public SwitchCase(GMLSyntaxNode test, GMLSyntaxNode body)
        {
            Test = test;
            Body = body;
        }
    }

    public class ContinueStatement : GMLSyntaxNode
    {
        public ContinueStatement() { }
    }

    public class BreakStatement : GMLSyntaxNode
    {
        public BreakStatement() { }
    }

    public class ExitStatement : GMLSyntaxNode
    {
        public ExitStatement() { }
    }

    public class AssignmentExpression : GMLSyntaxNode
    {
        public string Operator { get; set; }
        public GMLSyntaxNode Left { get; set; }
        public GMLSyntaxNode Right { get; set; }

        public AssignmentExpression(string @operator, GMLSyntaxNode left, GMLSyntaxNode right)
        {
            Operator = @operator;
            Left = left;
            Right = right;
        }
    }

    public class CallExpression : GMLSyntaxNode, IHasObject
    {
        public GMLSyntaxNode Object { get; set; }
        public GMLSyntaxNode Arguments { get; set; }

        public CallExpression(GMLSyntaxNode @object, GMLSyntaxNode arguments)
        {
            Object = @object;
            Arguments = arguments;
        }
    }

    public class MemberIndexExpression : GMLSyntaxNode, IHasObject
    {
        public GMLSyntaxNode Object { get; set; }
        public GMLSyntaxNode Property { get; set; }
        public string Accessor { get; set; }

        public MemberIndexExpression(GMLSyntaxNode @object, GMLSyntaxNode property, string accessor)
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

        public MemberDotExpression(GMLSyntaxNode @object, GMLSyntaxNode property)
        {
            Object = @object;
            Property = property;
        }
    }

    public class Literal : GMLSyntaxNode
    {
        public string Text { get; set; }

        public Literal(string text)
        {
            Text = text;
        }
    }

    public class Identifier : GMLSyntaxNode
    {
        public string Name { get; set; }

        public Identifier(string name)
        {
            Name = name;
        }
    }
}
