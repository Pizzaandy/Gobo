using Antlr4.Runtime;
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

        public override Doc VisitStatementList(
            [NotNull] GameMakerLanguageParser.StatementListContext context
        )
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
                result.Add(Doc.HardLine);
            }

            return Doc.Concat(result);
        }

        public override Doc VisitStatement(
            [NotNull] GameMakerLanguageParser.StatementContext context
        )
        {
            var parts = new List<Doc>();
            bool needsSemicolon = false;

            if (context.block() != null)
            {
                parts.Add(Visit(context.block()));
            }
            if (context.ifStatement() != null)
            {
                parts.Add(Visit(context.ifStatement()));
            }
            if (context.variableDeclarationList() != null)
            {
                needsSemicolon = true;
                parts.Add(Visit(context.variableDeclarationList()));
            }
            if (context.assignmentExpression() != null)
            {
                needsSemicolon = true;
                parts.Add(Visit(context.assignmentExpression()));
            }
            if (context.callStatement() != null)
            {
                needsSemicolon = true;
                parts.Add(Visit(context.callStatement()));
            }
            if (context.iterationStatement() != null)
            {
                parts.Add(Visit(context.iterationStatement()));
            }
            if (context.functionDeclaration() != null)
            {
                parts.Add(Visit(context.functionDeclaration()));
            }
            if (context.switchStatement() != null)
            {
                parts.Add(Visit(context.switchStatement()));
            }
            if (context.enumeratorDeclaration() != null)
            {
                parts.Add(Visit(context.enumeratorDeclaration()));
            }
            if (context.incDecStatement() != null)
            {
                needsSemicolon = true;
                parts.Add(Visit(context.incDecStatement()));
            }
            if (context.withStatement() != null)
            {
                parts.Add(Visit(context.withStatement()));
            }
            if (context.returnStatement() != null)
            {
                needsSemicolon = true;
                parts.Add(Visit(context.returnStatement()));
            }
            if (context.exitStatement() != null)
            {
                needsSemicolon = true;
                parts.Add(Visit(context.exitStatement()));
            }
            if (context.continueStatement() != null)
            {
                needsSemicolon = true;
                parts.Add(Visit(context.continueStatement()));
            }
            if (context.breakStatement() != null)
            {
                needsSemicolon = true;
                parts.Add(Visit(context.breakStatement()));
            }
            if (context.throwStatement() != null)
            {
                needsSemicolon = true;
                parts.Add(Visit(context.throwStatement()));
            }
            if (context.tryStatement() != null)
            {
                parts.Add(Visit(context.tryStatement()));
            }
            if (context.globalVarStatement() != null)
            {
                needsSemicolon = true;
                parts.Add(Visit(context.globalVarStatement()));
            }
            if (context.macroStatement() != null)
            {
                parts.Add(Visit(context.macroStatement()));
            }
            if (context.defineStatement() != null)
            {
                parts.Add(Visit(context.defineStatement()));
            }
            if (context.regionStatement() != null)
            {
                parts.Add(Visit(context.regionStatement()));
            }

            if (needsSemicolon)
            {
                parts.Add(";");
            }
            return Doc.Concat(parts);
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

        public override Doc VisitIfStatement(
            [NotNull] GameMakerLanguageParser.IfStatementContext context
        )
        {
            var parts = new List<Doc>
            {
                PrintSingleClauseStatement(
                    "if",
                    Visit(context.expression()),
                    context.statement()[0]
                )
            };
            if (context.statement().Length > 1)
            {
                parts.Add(" else ");
                var elseStatement = context.statement()[1];
                if (elseStatement.children[0] is GameMakerLanguageParser.IfStatementContext)
                {
                    parts.Add(Visit(elseStatement));
                }
                else
                {
                    parts.Add(PrintStatementInBlock(elseStatement));
                }
            }
            return Doc.Concat(parts);
        }

        public override Doc VisitAssignmentExpression(
            [NotNull] GameMakerLanguageParser.AssignmentExpressionContext context
        )
        {
            return Doc.Group(
                new Doc[]
                {
                    Doc.Group(Visit(context.lValueExpression())),
                    " ",
                    context.assignmentOperator().GetText(),
                    " ",
                    Doc.Group(Visit(context.expressionOrFunction()))
                }
            );
        }

        public override Doc VisitVariableDeclarationList(
            [NotNull] GameMakerLanguageParser.VariableDeclarationListContext context
        )
        {
            var parts = new List<Doc>() { context.varModifier().GetText(), " " };
            var variableDeclarations = context.variableDeclaration();
            var assignments = new List<Doc>();

            for (var i = 0; i < variableDeclarations.Length; i++)
            {
                assignments.Add(Visit(variableDeclarations[i]));
            }

            var separator = Doc.Concat(new Doc[] { ",", Doc.Line });
            parts.Add(Doc.Group(Doc.Indent(Doc.Join(separator, assignments))));
            return Doc.Concat(parts);
        }

        public override Doc VisitVariableDeclaration(
            [NotNull] GameMakerLanguageParser.VariableDeclarationContext context
        )
        {
            if (context.expressionOrFunction() != null)
            {
                var parts = new List<Doc>()
                {
                    Visit(context.identifier()),
                    " = ",
                    Visit(context.expressionOrFunction())
                };
                return Doc.Concat(parts);
            }
            else
            {
                return Visit(context.identifier());
            }
        }

        public override Doc VisitCallExpression(
            [NotNull] GameMakerLanguageParser.CallExpressionContext context
        )
        {
            return Visit(context.callStatement());
        }

        public override Doc VisitCallStatement(
            [NotNull] GameMakerLanguageParser.CallStatementContext context
        )
        {
            Doc callee;
            if (context.callStatement() != null)
            {
                callee = Visit(context.callStatement());
            }
            else
            {
                callee = Visit(context.callableExpression());
            }

            return Doc.Concat(new[] { callee, Visit(context.arguments()) });
        }

        public override Doc VisitArguments(
            [NotNull] GameMakerLanguageParser.ArgumentsContext context
        )
        {
            var parts = new List<Doc>();
            if (context.expressionOrFunction() != null)
            {
                var expressions = context.expressionOrFunction();
                for (var i = 0; i < expressions.Length; i++)
                {
                    parts.Add(Visit(expressions[i]));
                }
            }
            var separator = Doc.Concat(new Doc[] { ",", Doc.Line });
            return Doc.Group(
                new Doc[]
                {
                    "(",
                    Doc.Indent(new[] { Doc.SoftLine, Doc.Join(separator, parts) }),
                    Doc.SoftLine,
                    ")"
                }
            );
        }

        public override Doc VisitLValueExpression(
            [NotNull] GameMakerLanguageParser.LValueExpressionContext context
        )
        {
            var root = Visit(context.lValueStartExpression());
            var indentedParts = new List<Doc>();
            var currentGroup = new List<Doc>();

            // accumulate operations
            if (context.lValueChainOperator() != null)
            {
                var ops = context.lValueChainOperator();
                bool foundFirstCall = false;

                for (var i = 0; i < ops.Length; i++)
                {
                    if (
                        !foundFirstCall
                        && i < ops.Length - 1
                        && ops[i] is GameMakerLanguageParser.MemberDotLValueContext
                        && ops[i + 1] is GameMakerLanguageParser.CallLValueContext
                    )
                    {
                        foundFirstCall = true;
                        indentedParts.Add(Doc.Fill(currentGroup));
                        indentedParts.Add(Doc.SoftLine);
                        currentGroup = new List<Doc>();
                    }

                    if (
                        i > 0
                        && ops[i] is GameMakerLanguageParser.MemberDotLValueContext
                        && ops[i - 1] is GameMakerLanguageParser.CallLValueContext
                    )
                    {
                        indentedParts.Add(Doc.Fill(currentGroup));
                        indentedParts.Add(Doc.SoftLine);
                        currentGroup = new List<Doc>();
                    }

                    currentGroup.Add(Visit(ops[i]));
                }
            }

            if (currentGroup.Count > 0)
            {
                indentedParts.Add(Doc.Fill(currentGroup));
            }

            if (context.lValueFinalOperator() != null)
            {
                var op = context.lValueFinalOperator();

                if (op.Parent?.Parent is not GameMakerLanguageParser.CallableExpressionContext)
                {
                    // add a softline before a .call()
                    indentedParts.Add(Doc.SoftLine);
                }

                indentedParts.Add(Visit(op));
            }

            return Doc.Concat(new Doc[] { root, Doc.Group(Doc.Indent(indentedParts)) });
        }

        public override Doc VisitCallLValue(
            [NotNull] GameMakerLanguageParser.CallLValueContext context
        )
        {
            return Visit(context.arguments());
        }

        public override Doc VisitMemberDotLValue(
            [NotNull] GameMakerLanguageParser.MemberDotLValueContext context
        )
        {
            return Doc.Concat(Doc.SoftLine, ".", Visit(context.identifier()));
        }

        public override Doc VisitMemberDotLValueFinal(
            [NotNull] GameMakerLanguageParser.MemberDotLValueFinalContext context
        )
        {
            return Doc.Concat(".", Visit(context.identifier()));
        }

        public override Doc VisitIdentifierLValue(
            [NotNull] GameMakerLanguageParser.IdentifierLValueContext context
        )
        {
            return Visit(context.identifier());
        }

        public override Doc VisitIdentifier(
            [NotNull] GameMakerLanguageParser.IdentifierContext context
        )
        {
            return context.GetText();
        }

        public override Doc VisitLiteral([NotNull] GameMakerLanguageParser.LiteralContext context)
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
            if (context.HexIntegerLiteral() != null || context.BinaryLiteral() != null)
            {
                return context.GetText();
            }

            return context.GetText();
        }

        public override Doc VisitExpressionOrFunction(
            [NotNull] GameMakerLanguageParser.ExpressionOrFunctionContext context
        )
        {
            if (context.expressionOrFunction() != null)
            {
                return Visit(context.expressionOrFunction());
            }
            else if (context.expression() != null)
            {
                return Visit(context.expression());
            }
            else
            {
                return Visit(context.functionDeclaration());
            }
        }

        public override Doc VisitLiteralExpression(
            [NotNull] GameMakerLanguageParser.LiteralExpressionContext context
        )
        {
            return Visit(context.literal());
        }

        Doc PrintSingleClauseStatement(string keyword, Doc clause, ParserRuleContext body)
        {
            var parts = new Doc[]
            {
                keyword,
                " (",
                Doc.Group(Doc.Indent(new Doc[] { Doc.SoftLine, clause })),
                ") ",
                PrintStatementInBlock(body)
            };
            return Doc.Concat(parts);
        }

        Doc PrintStatementInBlock(ParserRuleContext statementContext)
        {
            if (statementContext is GameMakerLanguageParser.BlockContext)
            {
                return Visit(statementContext);
            }
            return Doc.Concat(
                new Doc[]
                {
                    "{",
                    Doc.Indent(Doc.Concat(new[] { Doc.HardLine, Visit(statementContext) })),
                    Doc.HardLine,
                    "}"
                }
            );
        }
    }
}
