using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace PrettierGML
{
    public class GameMakerASTBuilder : GameMakerLanguageParserBaseVisitor<GMLSyntaxNode>
    {
        public override GMLSyntaxNode VisitProgram(
            [NotNull] GameMakerLanguageParser.ProgramContext context
        )
        {
            if (context.statementList() != null)
            {
                return Visit(context.statementList());
            }
            else
            {
                return GMLSyntaxNode.Empty;
            }
        }

        public override GMLSyntaxNode VisitStatementList(
            [NotNull] GameMakerLanguageParser.StatementListContext context
        )
        {
            var statements = context.statement();
            var parts = new List<GMLSyntaxNode>(statements.Length);

            for (var i = 0; i < statements.Length; i++)
            {
                GMLSyntaxNode statement = Visit(statements[i]);
                if (statement is EmptyNode)
                {
                    continue;
                }
                parts.Add(statement);
            }

            return GMLSyntaxNode.List(parts);
        }

        public override GMLSyntaxNode VisitStatement(
            [NotNull] GameMakerLanguageParser.StatementContext context
        )
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
            if (context.withStatement() != null)
            {
                return Visit(context.withStatement());
            }
            if (context.returnStatement() != null)
            {
                return Visit(context.returnStatement());
            }
            if (context.exitStatement() != null)
            {
                return Visit(context.exitStatement());
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

            return GMLSyntaxNode.Empty;
        }

        public override GMLSyntaxNode VisitBlock(
            [NotNull] GameMakerLanguageParser.BlockContext context
        )
        {
            GMLSyntaxNode body;

            if (context.statementList() != null)
            {
                body = Visit(context.statementList());
            }
            else
            {
                body = GMLSyntaxNode.Empty;
            }

            return new Block(body);
        }

        public override GMLSyntaxNode VisitIfStatement(
            [NotNull] GameMakerLanguageParser.IfStatementContext context
        )
        {
            var test = Visit(context.expression());
            var consequent = Visit(context.statement()[0]);
            GMLSyntaxNode alternate = GMLSyntaxNode.Empty;

            if (context.statement().Length > 1)
            {
                alternate = Visit(context.statement()[1]);
            }

            return new IfStatement(test, consequent, alternate);
        }

        public override GMLSyntaxNode VisitDoStatement(
            [NotNull] GameMakerLanguageParser.DoStatementContext context
        )
        {
            var body = Visit(context.statement());
            var test = Visit(context.expression());
            return new DoStatement(body, test);
        }

        public override GMLSyntaxNode VisitWhileStatement(
            [NotNull] GameMakerLanguageParser.WhileStatementContext context
        )
        {
            var test = Visit(context.expression());
            var body = Visit(context.statement());
            return new WhileStatement(test, body);
        }

        public override GMLSyntaxNode VisitForStatement(
            [NotNull] GameMakerLanguageParser.ForStatementContext context
        )
        {
            GMLSyntaxNode init = GMLSyntaxNode.Empty;
            GMLSyntaxNode test = GMLSyntaxNode.Empty;
            GMLSyntaxNode update = GMLSyntaxNode.Empty;
            GMLSyntaxNode body = GMLSyntaxNode.Empty;

            if (context.variableDeclarationList() != null)
            {
                init = Visit(context.variableDeclarationList());
            }
            else if (context.assignmentExpression() != null)
            {
                init = Visit(context.assignmentExpression());
            }

            if (context.expression() != null)
            {
                test = Visit(context.expression());
            }

            if (context.statement().Length > 1)
            {
                update = Visit(context.statement()[0]);
                body = Visit(context.statement()[1]);
            }
            else
            {
                body = Visit(context.statement()[0]);
            }

            return new ForStatement(init, test, update, body);
        }

        public override GMLSyntaxNode VisitWithStatement(
            [NotNull] GameMakerLanguageParser.WithStatementContext context
        )
        {
            var @object = Visit(context.expression());
            var body = Visit(context.statement());
            return new WithStatement(@object, body);
        }

        public override GMLSyntaxNode VisitSwitchStatement(
            [NotNull] GameMakerLanguageParser.SwitchStatementContext context
        )
        {
            var discriminant = Visit(context.expression());
            var cases = Visit(context.caseBlock());
            return new SwitchStatement(discriminant, cases);
        }

        public override GMLSyntaxNode VisitCaseBlock(
            [NotNull] GameMakerLanguageParser.CaseBlockContext context
        )
        {
            var caseClauses = new List<GMLSyntaxNode>();
            if (context.caseClauses() != null)
            {
                var cases = context.caseClauses();
                foreach (var caseList in cases)
                {
                    var node = Visit(caseList);
                    if (node is NodeList nodeList)
                    {
                        caseClauses.AddRange(nodeList.Contents);
                    }
                }
            }
            if (context.defaultClause() != null)
            {
                caseClauses.Add(Visit(context.defaultClause()));
            }
            return GMLSyntaxNode.List(caseClauses);
        }

        public override GMLSyntaxNode VisitCaseClauses(
            [NotNull] GameMakerLanguageParser.CaseClausesContext context
        )
        {
            var parts = new List<GMLSyntaxNode>();
            var caseClauses = context.caseClause();
            foreach (var caseClause in caseClauses)
            {
                parts.Add(Visit(caseClause));
            }
            return GMLSyntaxNode.List(parts);
        }

        public override GMLSyntaxNode VisitCaseClause(
            [NotNull] GameMakerLanguageParser.CaseClauseContext context
        )
        {
            var test = Visit(context.expression());
            GMLSyntaxNode body = GMLSyntaxNode.Empty;
            if (context.statementList() != null)
            {
                body = Visit(context.statementList());
            }
            return new SwitchCase(test, body);
        }

        public override GMLSyntaxNode VisitDefaultClause(
            [NotNull] GameMakerLanguageParser.DefaultClauseContext context
        )
        {
            GMLSyntaxNode body = GMLSyntaxNode.Empty;
            if (context.statementList() != null)
            {
                body = Visit(context.statementList());
            }
            return new SwitchCase(GMLSyntaxNode.Empty, body);
        }

        public override GMLSyntaxNode VisitContinueStatement(
            [NotNull] GameMakerLanguageParser.ContinueStatementContext context
        )
        {
            return new ContinueStatement();
        }

        public override GMLSyntaxNode VisitBreakStatement(
            [NotNull] GameMakerLanguageParser.BreakStatementContext context
        )
        {
            return new BreakStatement();
        }

        public override GMLSyntaxNode VisitExitStatement(
            [NotNull] GameMakerLanguageParser.ExitStatementContext context
        )
        {
            return new ExitStatement();
        }

        public override GMLSyntaxNode VisitAssignmentExpression(
            [NotNull] GameMakerLanguageParser.AssignmentExpressionContext context
        )
        {
            var @operator = context.assignmentOperator().GetText();
            if (@operator == ":=")
            {
                @operator = "=";
            }
            var left = Visit(context.lValueExpression());
            var right = Visit(context.expressionOrFunction());
            return new AssignmentExpression(@operator, left, right);
        }

        public override GMLSyntaxNode VisitLiteral(
            [NotNull] GameMakerLanguageParser.LiteralContext context
        )
        {
            if (context.arrayLiteral() != null)
            {
                return Visit(context.arrayLiteral());
            }
            if (context.structLiteral() != null)
            {
                return Visit(context.structLiteral());
            }
            if (context.templateStringLiteral() != null)
            {
                return Visit(context.templateStringLiteral());
            }
            return new Literal(context.GetText());
        }

        public override GMLSyntaxNode VisitLiteralExpression(
            [NotNull] GameMakerLanguageParser.LiteralExpressionContext context
        )
        {
            return Visit(context.literal());
        }

        public override GMLSyntaxNode VisitExpressionOrFunction(
            [NotNull] GameMakerLanguageParser.ExpressionOrFunctionContext context
        )
        {
            if (context.expression() != null)
            {
                return Visit(context.expression());
            }
            if (context.functionDeclaration() != null)
            {
                return Visit(context.functionDeclaration());
            }
            if (context.expressionOrFunction() != null)
            {
                return Visit(context.expressionOrFunction());
            }
            return GMLSyntaxNode.Empty;
        }

        public override GMLSyntaxNode VisitLValueExpression(
            [NotNull] GameMakerLanguageParser.LValueExpressionContext context
        )
        {
            GMLSyntaxNode @object = Visit(context.lValueStartExpression());

            if (context.lValueChainOperator()?.Length > 0)
            {
                var ops = context.lValueChainOperator();
                foreach (var op in ops)
                {
                    var node = Visit(op);
                    (node as IHasObject)!.Object = @object;
                    @object = node;
                }
            }

            return @object;
        }

        public override GMLSyntaxNode VisitCallExpression(
            [NotNull] GameMakerLanguageParser.CallExpressionContext context
        )
        {
            return Visit(context.callStatement());
        }

        public override GMLSyntaxNode VisitArguments(
            [NotNull] GameMakerLanguageParser.ArgumentsContext context
        )
        {
            var parts = new List<GMLSyntaxNode>();
            var args = context.expressionOrFunction();
            foreach (var arg in args)
            {
                parts.Add(Visit(arg));
            }
            return GMLSyntaxNode.List(parts);
        }

        public override GMLSyntaxNode VisitCallStatement(
            [NotNull] GameMakerLanguageParser.CallStatementContext context
        )
        {
            GMLSyntaxNode @object = GMLSyntaxNode.Empty;
            if (context.callableExpression() != null)
            {
                @object = Visit(context.callableExpression());
            }
            if (context.callStatement() != null)
            {
                @object = Visit(context.callStatement());
            }
            return new CallExpression(@object, Visit(context.arguments()));
        }

        public override GMLSyntaxNode VisitCallLValue(
            [NotNull] GameMakerLanguageParser.CallLValueContext context
        )
        {
            return Visit(context.arguments());
        }

        public override GMLSyntaxNode VisitMemberDotLValue(
            [NotNull] GameMakerLanguageParser.MemberDotLValueContext context
        )
        {
            var property = Visit(context.identifier());
            return new MemberDotExpression(GMLSyntaxNode.Empty, property);
        }

        public override GMLSyntaxNode VisitMemberDotLValueFinal(
            [NotNull] GameMakerLanguageParser.MemberDotLValueFinalContext context
        )
        {
            var property = Visit(context.identifier());
            return new MemberDotExpression(GMLSyntaxNode.Empty, property);
        }

        public override GMLSyntaxNode VisitMemberIndexLValue(
            [NotNull] GameMakerLanguageParser.MemberIndexLValueContext context
        )
        {
            var property = Visit(context.expressionSequence());
            var accessor = context.accessor().GetText();
            return new MemberIndexExpression(GMLSyntaxNode.Empty, property, accessor);
        }

        public override GMLSyntaxNode VisitMemberIndexLValueFinal(
            [NotNull] GameMakerLanguageParser.MemberIndexLValueFinalContext context
        )
        {
            var property = Visit(context.expressionSequence());
            var accessor = context.accessor().GetText();
            return new MemberIndexExpression(GMLSyntaxNode.Empty, property, accessor);
        }

        public override GMLSyntaxNode VisitExpressionSequence(
            [NotNull] GameMakerLanguageParser.ExpressionSequenceContext context
        )
        {
            var parts = new List<GMLSyntaxNode>();
            foreach (var expression in context.expression())
            {
                parts.Add(Visit(expression));
            }
            return new NodeList(parts);
        }

        public override GMLSyntaxNode VisitParenthesizedExpression(
            [NotNull] GameMakerLanguageParser.ParenthesizedExpressionContext context
        )
        {
            ParserRuleContext content = context.expression();

            while (content is GameMakerLanguageParser.ParenthesizedExpressionContext clauseContext)
            {
                content = clauseContext.expression();
            }

            return Visit(content);
        }

        public override GMLSyntaxNode VisitIdentifierLValue(
            [NotNull] GameMakerLanguageParser.IdentifierLValueContext context
        )
        {
            return Visit(context.identifier());
        }

        public override GMLSyntaxNode VisitIdentifier(
            [NotNull] GameMakerLanguageParser.IdentifierContext context
        )
        {
            return new Identifier(context.GetText());
        }
    }
}
