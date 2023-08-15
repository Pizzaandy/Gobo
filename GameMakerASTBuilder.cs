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
                var statements = Visit(context.statementList());
                return new Program(context, statements);
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

            return GMLSyntaxNode.List(context, parts);
        }

        public override GMLSyntaxNode VisitStatement(
            [NotNull] GameMakerLanguageParser.StatementContext context
        )
        {
            if (context.block() != null)
            {
                return Visit(context.block());
            }
            else if (context.ifStatement() != null)
            {
                return Visit(context.ifStatement());
            }
            else if (context.variableDeclarationList() != null)
            {
                return Visit(context.variableDeclarationList());
            }
            else if (context.assignmentExpression() != null)
            {
                return Visit(context.assignmentExpression());
            }
            else if (context.callStatement() != null)
            {
                return Visit(context.callStatement());
            }
            else if (context.iterationStatement() != null)
            {
                return Visit(context.iterationStatement());
            }
            else if (context.functionDeclaration() != null)
            {
                return Visit(context.functionDeclaration());
            }
            else if (context.switchStatement() != null)
            {
                return Visit(context.switchStatement());
            }
            else if (context.enumeratorDeclaration() != null)
            {
                return Visit(context.enumeratorDeclaration());
            }
            else if (context.incDecStatement() != null)
            {
                return Visit(context.incDecStatement());
            }
            else if (context.withStatement() != null)
            {
                return Visit(context.withStatement());
            }
            else if (context.returnStatement() != null)
            {
                return Visit(context.returnStatement());
            }
            else if (context.exitStatement() != null)
            {
                return Visit(context.exitStatement());
            }
            else if (context.continueStatement() != null)
            {
                return Visit(context.continueStatement());
            }
            else if (context.breakStatement() != null)
            {
                return Visit(context.breakStatement());
            }
            else if (context.throwStatement() != null)
            {
                return Visit(context.throwStatement());
            }
            else if (context.tryStatement() != null)
            {
                return Visit(context.tryStatement());
            }
            else if (context.globalVarStatement() != null)
            {
                return Visit(context.globalVarStatement());
            }
            else if (context.macroStatement() != null)
            {
                return Visit(context.macroStatement());
            }
            else if (context.defineStatement() != null)
            {
                return Visit(context.defineStatement());
            }
            else if (context.regionStatement() != null)
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

            return new Block(context, body);
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

            return new IfStatement(context, test, consequent, alternate);
        }

        public override GMLSyntaxNode VisitDoStatement(
            [NotNull] GameMakerLanguageParser.DoStatementContext context
        )
        {
            var body = Visit(context.statement());
            var test = Visit(context.expression());
            return new DoStatement(context, body, test);
        }

        public override GMLSyntaxNode VisitWhileStatement(
            [NotNull] GameMakerLanguageParser.WhileStatementContext context
        )
        {
            var test = Visit(context.expression());
            var body = Visit(context.statement());
            return new WhileStatement(context, test, body);
        }

        public override GMLSyntaxNode VisitForStatement(
            [NotNull] GameMakerLanguageParser.ForStatementContext context
        )
        {
            GMLSyntaxNode init = GMLSyntaxNode.Empty;
            GMLSyntaxNode test = GMLSyntaxNode.Empty;
            GMLSyntaxNode update = GMLSyntaxNode.Empty;
            GMLSyntaxNode body;

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

            return new ForStatement(context, init, test, update, body);
        }

        public override GMLSyntaxNode VisitWithStatement(
            [NotNull] GameMakerLanguageParser.WithStatementContext context
        )
        {
            var @object = Visit(context.expression());
            var body = Visit(context.statement());
            return new WithStatement(context, @object, body);
        }

        public override GMLSyntaxNode VisitSwitchStatement(
            [NotNull] GameMakerLanguageParser.SwitchStatementContext context
        )
        {
            var discriminant = Visit(context.expression());
            var cases = Visit(context.caseBlock());
            return new SwitchStatement(context, discriminant, cases);
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
            return GMLSyntaxNode.List(context, caseClauses);
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
            return GMLSyntaxNode.List(context, parts);
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
            return new SwitchCase(context, test, body);
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
            return new SwitchCase(context, GMLSyntaxNode.Empty, body);
        }

        public override GMLSyntaxNode VisitContinueStatement(
            [NotNull] GameMakerLanguageParser.ContinueStatementContext context
        )
        {
            return new ContinueStatement(context);
        }

        public override GMLSyntaxNode VisitBreakStatement(
            [NotNull] GameMakerLanguageParser.BreakStatementContext context
        )
        {
            return new BreakStatement(context);
        }

        public override GMLSyntaxNode VisitExitStatement(
            [NotNull] GameMakerLanguageParser.ExitStatementContext context
        )
        {
            return new ExitStatement(context);
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
            return new AssignmentExpression(context, @operator, left, right);
        }

        public override GMLSyntaxNode VisitVariableDeclarationList(
            [NotNull] GameMakerLanguageParser.VariableDeclarationListContext context
        )
        {
            var declarations = new List<GMLSyntaxNode>();
            var declarationContexts = context.variableDeclaration();
            foreach (var declaration in declarationContexts)
            {
                declarations.Add(Visit(declaration));
            }
            var kind = context.varModifier().GetText();
            return new VariableDeclarationList(
                context,
                GMLSyntaxNode.List(context, declarations),
                kind
            );
        }

        public override GMLSyntaxNode VisitVariableDeclaration(
            [NotNull] GameMakerLanguageParser.VariableDeclarationContext context
        )
        {
            var id = Visit(context.identifier());
            GMLSyntaxNode initializer = GMLSyntaxNode.Empty;
            if (context.expressionOrFunction() != null)
            {
                initializer = Visit(context.expressionOrFunction());
            }
            return new VariableDeclarator(context, id, initializer);
        }

        public override GMLSyntaxNode VisitFunctionDeclaration(
            [NotNull] GameMakerLanguageParser.FunctionDeclarationContext context
        )
        {
            GMLSyntaxNode id = GMLSyntaxNode.Empty;
            var parameters = Visit(context.parameterList());
            var body = Visit(context.block());
            GMLSyntaxNode constructorClause = GMLSyntaxNode.Empty;

            if (context.Identifier() != null)
            {
                id = new Identifier(context.Identifier(), context.Identifier().GetText());
            }

            if (context.constructorClause() != null)
            {
                constructorClause = Visit(context.constructorClause());
            }

            return new FunctionDeclaration(context, id, parameters, body, constructorClause);
        }

        public override GMLSyntaxNode VisitConstructorClause(
            [NotNull] GameMakerLanguageParser.ConstructorClauseContext context
        )
        {
            GMLSyntaxNode id = GMLSyntaxNode.Empty;
            GMLSyntaxNode parameters = GMLSyntaxNode.Empty;
            if (context.parameterList() != null)
            {
                parameters = Visit(context.parameterList());
            }
            if (context.Identifier() != null)
            {
                id = new Identifier(context.Identifier(), context.Identifier().GetText());
            }
            return new ConstructorClause(context, id, parameters);
        }

        public override GMLSyntaxNode VisitParameterList(
            [NotNull] GameMakerLanguageParser.ParameterListContext context
        )
        {
            var parts = new List<GMLSyntaxNode>();
            var args = context.parameterArgument();
            foreach (var arg in args)
            {
                parts.Add(Visit(arg));
            }
            return GMLSyntaxNode.List(context, parts);
        }

        public override GMLSyntaxNode VisitParameterArgument(
            [NotNull] GameMakerLanguageParser.ParameterArgumentContext context
        )
        {
            var name = Visit(context.identifier());
            GMLSyntaxNode initializer = GMLSyntaxNode.Empty;
            if (context.expressionOrFunction() != null)
            {
                initializer = Visit(context.expressionOrFunction());
            }
            return new Parameter(context, name, initializer);
        }

        public override GMLSyntaxNode VisitLiteral(
            [NotNull] GameMakerLanguageParser.LiteralContext context
        )
        {
            if (context.arrayLiteral() != null)
            {
                return Visit(context.arrayLiteral());
            }
            else if (context.structLiteral() != null)
            {
                return Visit(context.structLiteral());
            }
            else if (context.templateStringLiteral() != null)
            {
                return Visit(context.templateStringLiteral());
            }
            else
            {
                return new Literal(context, context.GetText());
            }
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
            else if (context.functionDeclaration() != null)
            {
                return Visit(context.functionDeclaration());
            }
            else if (context.expressionOrFunction() != null)
            {
                return Visit(context.expressionOrFunction());
            }
            else
            {
                return GMLSyntaxNode.Empty;
            }
        }

        public override GMLSyntaxNode VisitLValueStartExpression(
            [NotNull] GameMakerLanguageParser.LValueStartExpressionContext context
        )
        {
            if (context.identifier() != null)
            {
                return Visit(context.identifier());
            }
            else if (context.expressionOrFunction() != null)
            {
                return Visit(context.expressionOrFunction());
            }
            else
            {
                return Visit(context.newExpression());
            }
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

            if (context.lValueFinalOperator() != null)
            {
                var node = Visit(context.lValueFinalOperator());
                (node as IHasObject)!.Object = @object;
                @object = node;
            }

            return @object;
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
            return GMLSyntaxNode.List(context, parts);
        }

        public override GMLSyntaxNode VisitCallExpression(
            [NotNull] GameMakerLanguageParser.CallExpressionContext context
        )
        {
            return Visit(context.callStatement());
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
            return new CallExpression(context, @object, Visit(context.arguments()));
        }

        public override GMLSyntaxNode VisitCallLValue(
            [NotNull] GameMakerLanguageParser.CallLValueContext context
        )
        {
            return new CallExpression(context, GMLSyntaxNode.Empty, Visit(context.arguments()));
        }

        public override GMLSyntaxNode VisitCallableExpression(
            [NotNull] GameMakerLanguageParser.CallableExpressionContext context
        )
        {
            if (context.lValueExpression() != null)
            {
                return Visit(context.lValueExpression());
            }
            if (context.functionDeclaration() != null)
            {
                return Visit(context.functionDeclaration());
            }
            if (context.callableExpression() != null)
            {
                return Visit(context.callableExpression());
            }
            return GMLSyntaxNode.Empty;
        }

        public override GMLSyntaxNode VisitNewExpression(
            [NotNull] GameMakerLanguageParser.NewExpressionContext context
        )
        {
            var name = Visit(context.identifier());
            var arguments = Visit(context.arguments());
            return new NewExpression(context, name, arguments);
        }

        public override GMLSyntaxNode VisitMemberDotLValue(
            [NotNull] GameMakerLanguageParser.MemberDotLValueContext context
        )
        {
            var property = Visit(context.identifier());
            return new MemberDotExpression(context, GMLSyntaxNode.Empty, property);
        }

        public override GMLSyntaxNode VisitMemberDotLValueFinal(
            [NotNull] GameMakerLanguageParser.MemberDotLValueFinalContext context
        )
        {
            var property = Visit(context.identifier());
            return new MemberDotExpression(context, GMLSyntaxNode.Empty, property);
        }

        public override GMLSyntaxNode VisitMemberIndexLValue(
            [NotNull] GameMakerLanguageParser.MemberIndexLValueContext context
        )
        {
            var property = Visit(context.expressionSequence());
            var accessor = context.accessor().GetText();
            return new MemberIndexExpression(context, GMLSyntaxNode.Empty, property, accessor);
        }

        public override GMLSyntaxNode VisitMemberIndexLValueFinal(
            [NotNull] GameMakerLanguageParser.MemberIndexLValueFinalContext context
        )
        {
            var property = Visit(context.expressionSequence());
            var accessor = context.accessor().GetText();
            return new MemberIndexExpression(context, GMLSyntaxNode.Empty, property, accessor);
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
            return GMLSyntaxNode.List(context, parts);
        }

        public override GMLSyntaxNode VisitParenthesizedExpression(
            [NotNull] GameMakerLanguageParser.ParenthesizedExpressionContext context
        )
        {
            ParserRuleContext content = context.expression();
            return Visit(content);
        }

        public override GMLSyntaxNode VisitArrayLiteral(
            [NotNull] GameMakerLanguageParser.ArrayLiteralContext context
        )
        {
            if (context.elementList() == null)
            {
                return new ArrayExpression(context, GMLSyntaxNode.Empty);
            }

            var elements = context.elementList().expressionOrFunction();
            var elementNodes = new List<GMLSyntaxNode>();
            foreach (var element in elements)
            {
                elementNodes.Add(Visit(element));
            }

            return new ArrayExpression(context, GMLSyntaxNode.List(context, elementNodes));
        }

        public override GMLSyntaxNode VisitStructLiteral(
            [NotNull] GameMakerLanguageParser.StructLiteralContext context
        )
        {
            var properties = context.propertyAssignment();
            var propertyNodes = new List<GMLSyntaxNode>();
            foreach (var property in properties)
            {
                propertyNodes.Add(Visit(property));
            }
            return new StructExpression(context, GMLSyntaxNode.List(context, propertyNodes));
        }

        public override GMLSyntaxNode VisitPropertyAssignment(
            [NotNull] GameMakerLanguageParser.PropertyAssignmentContext context
        )
        {
            var name = new Identifier(
                context.propertyIdentifier(),
                context.propertyIdentifier().GetText()
            );
            var expression = Visit(context.expressionOrFunction());
            return new StructProperty(context, name, expression);
        }

        public override GMLSyntaxNode VisitIdentifier(
            [NotNull] GameMakerLanguageParser.IdentifierContext context
        )
        {
            return new Identifier(context, context.GetText());
        }
    }
}
