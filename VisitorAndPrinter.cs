using Antlr4.Runtime.Misc;

namespace GMLParser
{
	internal class VisitorAndPrinter : GameMakerLanguageParserBaseVisitor<Doc>
	{
		public override Doc VisitProgram([NotNull] GameMakerLanguageParser.ProgramContext context)
		{
			if (context.statementList() != null)
			{
				return Visit(context.statementList());
			}
			else
			{
				return Doc.Null;
			}
		}

		public override Doc VisitStatementList([NotNull] GameMakerLanguageParser.StatementListContext context)
		{
			var statements = context.statement();
			var result = new List<Doc>(statements.Length);
			for (var i = 0; i < statements.Length; i++)
			{
				Doc statement = Visit(statements[i]);
				if (statement is NullDoc)
				{
					continue;
				}
				result.Add(statement);
			}
			return Doc.Concat(result);
		}

		public override Doc VisitStatement([NotNull] GameMakerLanguageParser.StatementContext context)
		{
			if (context.block() != null)
			{
				return Visit(context.block());
			}
			if (context.ifStatement() != null)
			{
				return Visit(context.ifStatement());
			}
			if (context.variableDeclarationList() != null)
			{
				return Visit(context.variableDeclarationList());
			}
			if (context.assignmentExpression() != null)
			{
				return Visit(context.assignmentExpression());
			}
			if (context.callStatement() != null)
			{
				return Visit(context.callStatement());
			}
			if (context.iterationStatement() != null)
			{
				return Visit(context.iterationStatement());
			}
			if (context.functionDeclaration() != null)
			{
				return Visit(context.functionDeclaration());
			}
			if (context.switchStatement() != null)
			{
				return Visit(context.switchStatement());
			}
			if (context.enumeratorDeclaration() != null)
			{
				return Visit(context.enumeratorDeclaration());
			}
			if (context.incDecStatement() != null)
			{
				return Visit(context.incDecStatement());
			}
			if (context.returnStatement() != null)
			{
				return Visit(context.returnStatement());
			}
			if (context.exitStatement() != null)
			{
				return Visit(context.exitStatement());
			}
			if (context.withStatement() != null)
			{
				return Visit(context.withStatement());
			}
			if (context.continueStatement() != null)
			{
				return Visit(context.continueStatement());
			}
			if (context.breakStatement() != null)
			{
				return Visit(context.breakStatement());
			}
			if (context.throwStatement() != null)
			{
				return Visit(context.throwStatement());
			}
			if (context.tryStatement() != null)
			{
				return Visit(context.tryStatement());
			}
			if (context.globalVarStatement() != null)
			{
				return Visit(context.globalVarStatement());
			}
			if (context.macroStatement() != null)
			{
				return Visit(context.macroStatement());
			}
			if (context.defineStatement() != null)
			{
				return Visit(context.defineStatement());
			}
			if (context.regionStatement() != null)
			{
				return Visit(context.regionStatement());
			}
			return Doc.Null;
		}

		public override Doc VisitBlock([NotNull] GameMakerLanguageParser.BlockContext context)
		{
			Doc body;
			if (context.statementList() != null)
			{
				body = Visit(context.statementList());
			}
			else
			{
				body = Doc.Null;
			}
			return Doc.Concat(new Doc[] { "{", body, "}" });
		}
	}
}
