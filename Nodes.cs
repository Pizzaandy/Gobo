using Antlr4.Runtime;

namespace GML.AST
{
	public abstract class AstNode
	{
		public NodeLocation Location { get; set; }
		public string Type
		{
			get { return GetType().Name; }
		}
		public AstNode(ParserRuleContext context)
		{
			Location = new(context.Start.StartIndex, context.Stop.StopIndex);
		}
	}

	public class NodeLocation
	{
		public int StartIndex;
		public int EndIndex;
		public NodeLocation(int start, int end)
		{
			StartIndex = start;
			EndIndex = end;
		}
	}

	public class BlockStatement : AstNode
	{
		public AstNode[] Body;
		public BlockStatement(ParserRuleContext context, AstNode[] body) : base(context)
		{
			Body = body;
		}
	}

	public class IfStatement : AstNode
	{
		public AstNode Test;
		public AstNode Consequent;
		public AstNode Alternate;
		public IfStatement(ParserRuleContext context, AstNode test, AstNode consequent, AstNode alternate) : base(context)
		{
			Test = test;
			Consequent = consequent;
			Alternate = alternate;
		}
	}

	public class DoStatement : AstNode
	{
		public AstNode Body;
		public AstNode Test;
		public DoStatement(ParserRuleContext context, AstNode body, AstNode test) : base(context)
		{
			Body = body;
			Test = test;
		}
	}

	public class WhileStatement : AstNode
	{
		public AstNode Body;
		public AstNode Test;
		public WhileStatement(ParserRuleContext context, AstNode body, AstNode test) : base(context)
		{
			Body = body;
			Test = test;
		}
	}

	public class RepeatStatement : AstNode
	{
		public AstNode Body;
		public AstNode Test;
		public RepeatStatement(ParserRuleContext context, AstNode body, AstNode test) : base(context)
		{
			Body = body;
			Test = test;
		}
	}

	public class ForStatement : AstNode
	{
		public AstNode Init;
		public AstNode Test;
		public AstNode Update;
		public AstNode Body;
		public ForStatement(ParserRuleContext context, AstNode init, AstNode test, AstNode update, AstNode body) : base(context)
		{
			Init = init;
			Test = test;
			Update = update;
			Body = body;
		}
	}

	public class WithStatemement : AstNode
	{
		public AstNode Object;
		public AstNode Body;
		public WithStatemement(ParserRuleContext context, AstNode @object, AstNode body) : base(context)
		{
			Object = @object;
			Body = body;
		}
	}

	public class SwitchStatement : AstNode
	{
		public AstNode Discriminant;
		public AstNode[] Cases;
		public SwitchStatement(ParserRuleContext context, AstNode discriminant, AstNode[] cases) : base(context)
		{
			Discriminant = discriminant;
			Cases = cases;
		}
	}

	public class SwitchCase : AstNode
	{
		public AstNode Test;
		public AstNode Consequent;
		public SwitchCase(ParserRuleContext context, AstNode test, AstNode consequent) : base(context)
		{
			Test = test;
			Consequent = consequent;
		}
	}

	public class ContinueStatement : AstNode
	{
		public ContinueStatement(ParserRuleContext context) : base(context) { }
	}

	public class BreakStatement : AstNode
	{
		public BreakStatement(ParserRuleContext context) : base(context) { }
	}

	public class ExitStatement : AstNode
	{
		public ExitStatement(ParserRuleContext context) : base(context) { }
	}

	public class ThrowStatement : AstNode
	{
		public AstNode Argument;
		public ThrowStatement(ParserRuleContext context, AstNode argument) : base(context)
		{
			Argument = argument;
		}
	}

	public class ReturnStatement : AstNode
	{
		public AstNode Argument;
		public ReturnStatement(ParserRuleContext context, AstNode argument) : base(context)
		{
			Argument = argument;
		}
	}

	public class DeleteStatement : AstNode
	{
		public AstNode Argument;
		public DeleteStatement(ParserRuleContext context, AstNode argument) : base(context)
		{
			Argument = argument;
		}
	}

	public class TryStatement : AstNode
	{
		public AstNode Body;
		public AstNode Handler;
		public AstNode Finalizer;
		public TryStatement(ParserRuleContext context, AstNode body, AstNode handler, AstNode finalizer) : base(context)
		{
			Body = body;
			Handler = handler;
			Finalizer = finalizer;
		}
	}

	public class CatchClause : AstNode
	{
		public AstNode Param;
		public AstNode Body;
		public CatchClause(ParserRuleContext context, AstNode param, AstNode body) : base(context)
		{
			Param = param;
			Body = body;
		}
	}

	public class Finalizer : AstNode
	{
		public AstNode Body;
		public Finalizer(ParserRuleContext context, AstNode body) : base(context)
		{
			Body = body;
		}
	}

	public class Assignment : AstNode
	{
		public string Operator;
		public AstNode Left;
		public AstNode Right;
		public Assignment(ParserRuleContext context, string @operator, AstNode left, AstNode right) : base(context)
		{
			Operator = @operator;
			Left = left;
			Right = right;
		}
	}

	public class VariableDeclaration : AstNode
	{
		public AstNode[] Declarations;
		public string Kind;
		public VariableDeclaration(ParserRuleContext context, AstNode[] declarations, string kind) : base(context)
		{
			Declarations = declarations;
			Kind = kind;
		}
	}

	public class VariableDeclarator : AstNode
	{
		public AstNode Id;
		public AstNode Init;
		public VariableDeclarator(ParserRuleContext context, AstNode id, AstNode init) : base(context)
		{
			Id = id;
			Init = init;
		}
	}

	public class CallExpression : AstNode
	{
		public AstNode Object;
		public AstNode[] Arguments;

		public CallExpression(ParserRuleContext context, AstNode @object, AstNode[] arguments) : base(context)
		{
			Object = @object;
			Arguments = arguments;
		}
	}

	public class MemberIndexExpression : AstNode
	{
		public AstNode Object;
		public AstNode Property;
		public string Accessor;
		public MemberIndexExpression(ParserRuleContext context, AstNode @object, AstNode property, string accessor) : base(context)
		{
			Object = @object;
			Property = property;
			Accessor = accessor;
		}
	}

	public class MemberDotExpression : AstNode
	{
		public AstNode Object;
		public AstNode Property;
		public MemberDotExpression(ParserRuleContext context, AstNode @object, AstNode property) : base(context)
		{
			Object = @object;
			Property = property;
		}
	}

	public class UnaryExpression : AstNode
	{
		public string Operator;
		public bool IsPrefix;
		public AstNode Argument;
		public UnaryExpression(ParserRuleContext context, string @operator, bool isPrefix, AstNode argument) : base(context)
		{
			Operator = @operator;
			IsPrefix = isPrefix;
			Argument = argument;
		}
	}

	public class BinaryExpression : AstNode
	{
		public string Operator;
		public AstNode Left;
		public AstNode Right;
		public BinaryExpression(ParserRuleContext context, string @operator, AstNode left, AstNode right) : base(context)
		{
			Operator = @operator;
			Left = left;
			Right = right;
		}
	}

	public class TernaryExpression : AstNode
	{
		public AstNode Test;
		public AstNode Consequent;
		public AstNode Alternate;
		public TernaryExpression(ParserRuleContext context, AstNode test, AstNode consequent, AstNode alternate) : base(context)
		{
			Test = test;
			Consequent = consequent;
			Alternate = alternate;
		}
	}

	public class NewExpression : AstNode
	{
		public AstNode Expression;
		public AstNode Arguments;
		public NewExpression(ParserRuleContext context, AstNode expression, AstNode arguments) : base(context)
		{
			Expression = expression;
			Arguments = arguments;
		}
	}

	public class ArrayExpression : AstNode
	{
		public AstNode[] Elements;
		public bool HasTrailingComma;
		public ArrayExpression(ParserRuleContext context, AstNode[] elements, bool hasTrailingComma = false) : base(context)
		{
			Elements = elements;
			HasTrailingComma = hasTrailingComma;
		}
	}

	public class TemplateStringLiteral : AstNode
	{
		public AstNode[] Atoms;
		public TemplateStringLiteral(ParserRuleContext context, AstNode[] atoms) : base(context)
		{
			Atoms = atoms;
		}
	}

	public class TemplateStringExpression : AstNode
	{
		public AstNode Expression;
		public TemplateStringExpression(ParserRuleContext context, AstNode expression) : base(context)
		{
			Expression = expression;
		}
	}

	public class TemplateStringText : AstNode
	{
		public string Value;
		public TemplateStringText(ParserRuleContext context, string text) : base(context)
		{
			Value = text;
		}
	}

	public class RealLiteral : AstNode
	{
		public float Value;
		public string Text;
		public RealLiteral(ParserRuleContext context, string text) : base(context)
		{
			Text = text;
			Value = int.Parse(text);
		}
	}
	public class StringLiteral : AstNode
	{
		public string Value;
		public StringLiteral(ParserRuleContext context, string text) : base(context)
		{
			Value = text;
		}
	}
}
