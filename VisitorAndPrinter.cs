using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace PrettierGML
{
    internal class VisitorAndPrinter : GameMakerLanguageParserBaseVisitor<Doc>
    {
        private readonly CommonTokenStream _tokens;
        private HashSet<int> _printedCommentGroups = new();

        public VisitorAndPrinter(CommonTokenStream tokens)
            : base()
        {
            _tokens = tokens;
        }

        public Doc Visit(ParserRuleContext tree)
        {
            // These rules are directly ripped off from Go's comment mapping :)
            // A comment group g is associated with a node n if:
            //
            //   - g starts on the same line as n ends (trailing)
            //   - g starts on the line immediately following n, and there is
            //     at least one empty line after g and before the next node (
            //   - g starts before n and is not associated to the node before n
            //     via the previous rules
            //

            if (!CanAttachComments(tree))
            {
                return base.Visit(tree);
            }

            var trailingComments = PrintTrailingComments(tree);
            var leadingComments = PrintLeadingComments(tree);

            if (trailingComments is not NullDoc || leadingComments is not NullDoc)
            {
                return Doc.Concat(leadingComments, base.Visit(tree), trailingComments);
            }
            else
            {
                return base.Visit(tree);
            }
        }

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
            var parts = new List<Doc>(statements.Length);

            for (var i = 0; i < statements.Length; i++)
            {
                Doc statement = Visit(statements[i]);
                if (statement is NullDoc)
                {
                    continue;
                }

                parts.Add(statement);
                parts.Add(PrintTrailingComments(statements[i]));
                if (i != statements.Length - 1)
                {
                    parts.Add(Doc.HardLine);
                    if (IsNextLineBlank(statements[i]))
                    {
                        parts.Add(Doc.HardLine);
                    }
                }
            }

            return Doc.Concat(parts);
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

            return Doc.Concat("{", Doc.Indent(Doc.HardLine, body), Doc.HardLine, "}");
        }

        public override Doc VisitIfStatement(
            [NotNull] GameMakerLanguageParser.IfStatementContext context
        )
        {
            var parts = new List<Doc>
            {
                PrintSingleClauseStatement("if", context.expression(), context.statement()[0])
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

        public override Doc VisitWithStatement(
            [NotNull] GameMakerLanguageParser.WithStatementContext context
        )
        {
            return PrintSingleClauseStatement("if", context.expression(), context.statement());
        }

        public override Doc VisitWhileStatement(
            [NotNull] GameMakerLanguageParser.WhileStatementContext context
        )
        {
            return PrintSingleClauseStatement("while", context.expression(), context.statement());
        }

        public override Doc VisitRepeatStatement(
            [NotNull] GameMakerLanguageParser.RepeatStatementContext context
        )
        {
            return PrintSingleClauseStatement("repeat", context.expression(), context.statement());
        }

        public override Doc VisitAssignmentExpression(
            [NotNull] GameMakerLanguageParser.AssignmentExpressionContext context
        )
        {
            var @operator = context.assignmentOperator().GetText();
            if (@operator == ":=")
            {
                @operator = "=";
            }

            ParserRuleContext expressionContext = context.expressionOrFunction();

            // trim unecessary parentheses
            if (context.expressionOrFunction().expression() != null)
            {
                expressionContext = context.expressionOrFunction().expression();

                while (
                    expressionContext
                        is GameMakerLanguageParser.ParenthesizedExpressionContext parenthesized
                )
                {
                    expressionContext = parenthesized.expression();
                }
            }

            var shouldIndent = false;

            //if (IsBinaryExpression(expressionContext))
            //{
            //    shouldIndent = true;
            //}

            return Doc.Group(
                Doc.Group(Visit(context.lValueExpression())),
                " ",
                @operator,
                " ",
                shouldIndent
                    ? Doc.Group(Doc.Indent(Visit(expressionContext)))
                    : Doc.Group(Visit(expressionContext))
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

            var separator = Doc.Concat(",", Doc.Line);
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
                // L value expressions handle printing arguments
                if (context.callableExpression().lValueExpression() != null)
                {
                    return callee;
                }
            }

            return Doc.Concat(callee, Visit(context.arguments()));
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
                    var trailingComments = PrintTrailingComments(expressions[i]);
                    parts.Add(Visit(expressions[i]));
                    if (i != expressions.Length - 1)
                    {
                        parts.Add(",");
                        if (trailingComments is not NullDoc)
                        {
                            parts.Add(trailingComments);
                        }

                        parts.Add(Doc.Line);
                    }
                }
            }

            if (parts.Count == 0)
            {
                var innerComments = PrintInnerComments(context, out bool containsSingleLineComment);
                if (innerComments is not NullDoc)
                {
                    var lineDoc = containsSingleLineComment ? Doc.HardLine : Doc.SoftLine;
                    return Doc.Group("(", Doc.Indent(lineDoc, innerComments), lineDoc, ")");
                }
                return "()";
            }

            return Doc.Group("(", Doc.Indent(Doc.SoftLine, Doc.Concat(parts)), Doc.SoftLine, ")");
        }

        // TODO: simplify logic
        public override Doc VisitLValueExpression(
            [NotNull] GameMakerLanguageParser.LValueExpressionContext context
        )
        {
            var root = Visit(context.lValueStartExpression());

            var flatParts = new List<Doc> { root };
            var breakParts = new List<Doc> { root };
            var indentedBreakParts = new List<Doc>();

            var rootText = context.lValueStartExpression().GetText();
            bool isRootCapitalized = char.IsUpper(rootText[0]);
            bool isRootShort = rootText.Length <= 4;

            bool isFirstCallHandled = false;
            ParserRuleContext? lastChainOp = null;

            var ops = context.lValueChainOperator();

            // accumulate operations
            if (ops != null && ops.Length > 0)
            {
                for (var i = 0; i < ops.Length; i++)
                {
                    var opDoc = Visit(ops[i]);

                    flatParts.Add(opDoc);

                    if (isFirstCallHandled)
                    {
                        if (ops[i] is GameMakerLanguageParser.CallLValueContext)
                        {
                            indentedBreakParts.Add(Doc.Null);
                        }
                        else if (ops[i] is GameMakerLanguageParser.MemberDotLValueContext)
                        {
                            if (i > 0 && ops[i - 1] is GameMakerLanguageParser.CallLValueContext)
                            {
                                indentedBreakParts.Add(Doc.HardLine);
                            }
                            else
                            {
                                indentedBreakParts.Add(Doc.SoftLine);
                            }
                        }
                        else if (i != 0)
                        {
                            indentedBreakParts.Add(Doc.SoftLine);
                        }

                        indentedBreakParts.Add(opDoc);
                    }
                    else
                    {
                        if (
                            i < ops.Length - 1
                            && ops[i] is GameMakerLanguageParser.MemberDotLValueContext
                            && ops[i + 1] is GameMakerLanguageParser.CallLValueContext
                        )
                        {
                            // keep the first call on the first line of the expression under certain circumstances
                            indentedBreakParts.Add(Doc.Null);
                            indentedBreakParts.Add(Doc.HardLine);

                            indentedBreakParts.Add(opDoc);
                            isFirstCallHandled = true;
                        }
                        else
                        {
                            breakParts.Add(opDoc);
                        }
                    }
                }

                lastChainOp = ops[ops.Length - 1];
            }

            if (context.lValueFinalOperator() != null)
            {
                var opDoc = Visit(context.lValueFinalOperator());
                flatParts.Add(opDoc);

                if (isFirstCallHandled)
                {
                    if (
                        context.Parent is GameMakerLanguageParser.CallableExpressionContext
                        || (
                            context.lValueFinalOperator()
                                is GameMakerLanguageParser.MemberDotLValueFinalContext
                            && lastChainOp is GameMakerLanguageParser.CallLValueContext
                        )
                    )
                    {
                        indentedBreakParts.Add(Doc.HardLine);
                    }
                    else
                    {
                        indentedBreakParts.Add(Doc.SoftLine);
                    }

                    indentedBreakParts.Add(opDoc);
                }
                else
                {
                    breakParts.Add(opDoc);
                }
            }

            if (context.Parent is GameMakerLanguageParser.CallableExpressionContext)
            {
                var currentContext = context.Parent;
                while (currentContext is not GameMakerLanguageParser.CallStatementContext)
                {
                    currentContext = currentContext.Parent;
                }
                var callContext = (GameMakerLanguageParser.CallStatementContext)currentContext;
                var argsDoc = Visit(callContext.arguments());
                flatParts.Add(argsDoc);

                if (isFirstCallHandled)
                {
                    indentedBreakParts.Add(argsDoc);
                }
                else
                {
                    breakParts.Add(argsDoc);
                }
            }

            if (flatParts.Count == 1)
            {
                return flatParts[0];
            }

            var breakPartsDoc = Doc.Concat(breakParts);
            var indentedBreakDoc = Doc.Indent(Doc.Fill(indentedBreakParts));

            return Doc.ConditionalGroup(
                Doc.Concat(flatParts),
                Doc.Concat(breakPartsDoc, indentedBreakDoc)
            );
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
            var innerComments = PrintInnerComments(context, out bool _);
            if (innerComments is not NullDoc)
            {
                return Doc.Concat(innerComments, Doc.HardLine, ".", Visit(context.identifier()));
            }
            return Doc.Concat(".", Visit(context.identifier()));
        }

        public override Doc VisitMemberDotLValueFinal(
            [NotNull] GameMakerLanguageParser.MemberDotLValueFinalContext context
        )
        {
            return Doc.Concat(".", Visit(context.identifier()));
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

        public override Doc VisitParenthesizedExpression(
            [NotNull] GameMakerLanguageParser.ParenthesizedExpressionContext context
        )
        {
            return Doc.Group(
                "(",
                Doc.Indent(Doc.SoftLine, Visit(context.expression())),
                Doc.SoftLine,
                ")"
            );
        }

        public override Doc VisitLiteralExpression(
            [NotNull] GameMakerLanguageParser.LiteralExpressionContext context
        )
        {
            return Visit(context.literal());
        }

        public override Doc VisitMultiplicativeExpression(
            [NotNull] GameMakerLanguageParser.MultiplicativeExpressionContext context
        )
        {
            return PrintBinaryExpressionGroup(context);
        }

        public override Doc VisitAdditiveExpression(
            [NotNull] GameMakerLanguageParser.AdditiveExpressionContext context
        )
        {
            return PrintBinaryExpressionGroup(context);
        }

        public override Doc VisitCoalesceExpression(
            [NotNull] GameMakerLanguageParser.CoalesceExpressionContext context
        )
        {
            return PrintBinaryExpressionGroup(context);
        }

        public override Doc VisitBitShiftExpression(
            [NotNull] GameMakerLanguageParser.BitShiftExpressionContext context
        )
        {
            return PrintBinaryExpressionGroup(context);
        }

        public override Doc VisitLogicalOrExpression(
            [NotNull] GameMakerLanguageParser.LogicalOrExpressionContext context
        )
        {
            return PrintBinaryExpressionGroup(context);
        }

        public override Doc VisitLogicalAndExpression(
            [NotNull] GameMakerLanguageParser.LogicalAndExpressionContext context
        )
        {
            return PrintBinaryExpressionGroup(context);
        }

        public override Doc VisitLogicalXorExpression(
            [NotNull] GameMakerLanguageParser.LogicalXorExpressionContext context
        )
        {
            return PrintBinaryExpressionGroup(context);
        }

        public override Doc VisitEqualityExpression(
            [NotNull] GameMakerLanguageParser.EqualityExpressionContext context
        )
        {
            return PrintBinaryExpressionGroup(context);
        }

        public override Doc VisitRelationalExpression(
            [NotNull] GameMakerLanguageParser.RelationalExpressionContext context
        )
        {
            return PrintBinaryExpressionGroup(context);
        }

        public override Doc VisitBitAndExpression(
            [NotNull] GameMakerLanguageParser.BitAndExpressionContext context
        )
        {
            return PrintBinaryExpressionGroup(context);
        }

        public override Doc VisitBitOrExpression(
            [NotNull] GameMakerLanguageParser.BitOrExpressionContext context
        )
        {
            return PrintBinaryExpressionGroup(context);
        }

        public override Doc VisitBitXOrExpression(
            [NotNull] GameMakerLanguageParser.BitXOrExpressionContext context
        )
        {
            return PrintBinaryExpressionGroup(context);
        }

        Doc PrintBinaryExpressionGroup(GameMakerLanguageParser.ExpressionContext context)
        {
            var parts = PrintBinaryExpression(context);
            return Doc.Concat(parts[0], Doc.Indent(parts.Skip(1).ToList()));
        }

        List<Doc> PrintBinaryExpression(GameMakerLanguageParser.ExpressionContext context)
        {
            var parts = new List<Doc>();

            var expressions = context.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>();
            var left = expressions[0];
            var right = expressions[1];

            var @operator = context.children[1].GetText();
            switch (@operator)
            {
                case "and":
                    @operator = "&&";
                    break;
                case "or":
                    @operator = "||";
                    break;
                case "xor":
                    @operator = "^^";
                    break;
                case "<>":
                    @operator = "!=";
                    break;
                case "mod":
                    @operator = "%";
                    break;
                case "not":
                    @operator = "!";
                    break;
            }

            Console.WriteLine(
                $"Node is {context.GetType()}, Precedence is {GetPrecedence(context)}"
            );
            Console.WriteLine(
                $"Parent node is {context.Parent.GetType()}, Precedence is {GetPrecedence(context.Parent as ParserRuleContext)}"
            );

            var shouldGroup =
                context.GetType() != context.Parent!.GetType()
                && GetPrecedence(context) != GetPrecedence(context.Parent as ParserRuleContext)
                && left.GetType() != context.GetType()
                && right.GetType() != context.GetType();

            var binaryOnTheRight = context is GameMakerLanguageParser.CoalesceExpressionContext;

            if (binaryOnTheRight)
            {
                parts.Add(Visit(left));
                parts.Add(Doc.Line);
                parts.Add(@operator);
                parts.Add(" ");
            }

            var possibleBinary = binaryOnTheRight ? right : left;
            var shouldInline = context is GameMakerLanguageParser.EqualityExpressionContext;

            if (
                IsBinaryExpression(possibleBinary)
                && (GetPrecedence(context) == GetPrecedence(possibleBinary))
            )
            {
                parts.AddRange(PrintBinaryExpression(possibleBinary));
            }
            else
            {
                parts.Add(Visit(possibleBinary));
            }

            if (binaryOnTheRight)
            {
                return shouldGroup
                    ? new List<Doc> { parts[0], Doc.Group(parts.Skip(1).ToList()) }
                    : parts;
            }

            var rightDoc = Doc.Concat(shouldInline ? " " : Doc.Line, @operator, " ", Visit(right));

            parts.Add(shouldGroup ? Doc.Group(rightDoc) : rightDoc);

            return parts;
        }

        static int GetPrecedence(ParserRuleContext context)
        {
            return context switch
            {
                GameMakerLanguageParser.BitXOrExpressionContext => 1,
                GameMakerLanguageParser.BitOrExpressionContext => 2,
                GameMakerLanguageParser.BitAndExpressionContext => 3,
                GameMakerLanguageParser.RelationalExpressionContext => 4,
                GameMakerLanguageParser.EqualityExpressionContext => 5,
                GameMakerLanguageParser.LogicalXorExpressionContext => 6,
                GameMakerLanguageParser.LogicalAndExpressionContext => 7,
                GameMakerLanguageParser.LogicalOrExpressionContext => 8,
                GameMakerLanguageParser.CoalesceExpressionContext => 9,
                GameMakerLanguageParser.BitShiftExpressionContext => 10,
                GameMakerLanguageParser.AdditiveExpressionContext => 11,
                GameMakerLanguageParser.MultiplicativeExpressionContext => 12,
                _ => -1,
            };
        }

        bool IsBinaryExpression(ParserRuleContext node)
        {
            return node is GameMakerLanguageParser.ExpressionContext
                && node is not GameMakerLanguageParser.RelationalExpressionContext
                && node.GetRuleContexts<GameMakerLanguageParser.ExpressionContext>().Length == 2;
        }

        Doc PrintSingleClauseStatement(
            string keyword,
            ParserRuleContext clause,
            ParserRuleContext body
        )
        {
            while (clause is GameMakerLanguageParser.ParenthesizedExpressionContext clauseContext)
            {
                clause = clauseContext.expression();
            }

            return Doc.Concat(
                keyword,
                " (",
                Doc.Group(Doc.Indent(Doc.SoftLine, Visit(clause))),
                ") ",
                PrintStatementInBlock(body)
            );
        }

        Doc PrintStatementInBlock(ParserRuleContext statementContext)
        {
            if (
                statementContext is GameMakerLanguageParser.StatementContext context
                && context.block() != null
            )
            {
                return Visit(context.block());
            }

            return Doc.Concat(
                "{",
                Doc.Indent(Doc.Concat(new[] { Doc.HardLine, Visit(statementContext) })),
                Doc.HardLine,
                "}"
            );
        }

        // only use to print comments in small, flat sequences of tokens
        Doc PrintInnerComments(ParserRuleContext context, out bool shouldBreak)
        {
            var tokens = _tokens.Get(context.Start.TokenIndex, context.Stop.TokenIndex);
            var parts = new List<Doc>();
            var currentGroup = new List<IToken>();
            shouldBreak = false;

            for (var i = 0; i < tokens.Count; i++)
            {
                if (
                    tokens[i].Type == GameMakerLanguageLexer.WhiteSpaces
                    || tokens[i].Type == GameMakerLanguageLexer.LineTerminator
                    || tokens[i].Type == GameMakerLanguageLexer.SingleLineComment
                    || tokens[i].Type == GameMakerLanguageLexer.MultiLineComment
                )
                {
                    if (tokens[i].Type == GameMakerLanguageLexer.SingleLineComment)
                    {
                        shouldBreak = true;
                    }
                    currentGroup.Add(tokens[i]);
                }
                else
                {
                    if (
                        currentGroup.Count > 0
                        && _printedCommentGroups.Add(currentGroup[0].TokenIndex)
                    )
                    {
                        parts.Add(PrintCommentsAndWhitespace(currentGroup, isTrailing: false));
                    }
                    currentGroup = new();
                }
            }

            if (parts.Count == 0)
            {
                return Doc.Null;
            }

            return Doc.Join(" ", parts);
        }

        Doc PrintLeadingComments(ParserRuleContext context)
        {
            var hiddenTokens = _tokens.GetHiddenTokensToLeft(context.Start.TokenIndex);
            bool leadingSingleLineComment = false;

            if (hiddenTokens != null)
            {
                for (var i = 0; i < hiddenTokens.Count; i++)
                {
                    var type = hiddenTokens[i].Type;
                    if (type == GameMakerLanguageLexer.WhiteSpaces)
                    {
                        continue;
                    }
                    else if (type == GameMakerLanguageLexer.SingleLineComment)
                    {
                        leadingSingleLineComment = true;
                        break;
                    }
                }

                if (!_printedCommentGroups.Add(hiddenTokens[0].TokenIndex))
                {
                    return Doc.Null;
                }

                var printed = PrintCommentsAndWhitespace(hiddenTokens, isTrailing: false);

                if (printed is not NullDoc)
                {
                    return leadingSingleLineComment
                        ? Doc.Concat(printed, Doc.HardLine)
                        : Doc.Concat(printed, " ");
                }
            }

            return Doc.Null;
        }

        Doc PrintTrailingComments(ParserRuleContext context)
        {
            int index;
            bool leadingSeparator = false;

            if (IsSeparator(_tokens.Get(context.Stop.TokenIndex + 1)))
            {
                leadingSeparator = true;
                index = context.Stop.TokenIndex + 1;
            }
            else
            {
                index = context.Stop.TokenIndex;
            }

            var hiddenTokens = _tokens.GetHiddenTokensToRight(index);

            if (hiddenTokens != null)
            {
                for (var i = 0; i < hiddenTokens.Count; i++)
                {
                    var type = hiddenTokens[i].Type;
                    if (type == GameMakerLanguageLexer.WhiteSpaces)
                    {
                        continue;
                    }
                    else if (type == GameMakerLanguageLexer.LineTerminator)
                    {
                        // comment group must start on same line as context
                        return Doc.Null;
                    }
                    else if (leadingSeparator && type == GameMakerLanguageLexer.MultiLineComment)
                    {
                        // multi-line comments can't be attached through a separator
                        return Doc.Null;
                    }
                    else
                    {
                        break;
                    }
                }

                if (!_printedCommentGroups.Add(hiddenTokens[0].TokenIndex))
                {
                    return Doc.Null;
                }

                var printed = PrintCommentsAndWhitespace(hiddenTokens, isTrailing: true);
                return printed is NullDoc ? Doc.Null : Doc.Concat(" ", printed);
            }

            return Doc.Null;
        }

        static Doc PrintCommentsAndWhitespace(
            IList<IToken> tokens,
            bool isTrailing,
            bool trim = true
        )
        {
            var parts = new List<Doc>();

            int firstCommentIndex = -1;
            int lastCommentIndex = -1;
            int consecutiveLineBreaks = 0;

            if (tokens.Count == 0)
            {
                return Doc.Null;
            }

            for (int i = 0; i < tokens.Count; i++)
            {
                var type = tokens[i].Type;

                if (type == GameMakerLanguageLexer.WhiteSpaces)
                {
                    if (
                        i < tokens.Count - 1
                        && tokens[i + 1].Type != GameMakerLanguageLexer.LineTerminator
                    )
                    {
                        parts.Add(" ");
                    }
                }
                else if (type == GameMakerLanguageLexer.LineTerminator)
                {
                    if (consecutiveLineBreaks < 2 && tokens[i].Text.Contains('\r'))
                    {
                        parts.Add(Doc.HardLine);
                        consecutiveLineBreaks++;
                    }
                }
                else
                {
                    consecutiveLineBreaks = 0;
                }

                if (
                    type == GameMakerLanguageLexer.SingleLineComment
                    || type == GameMakerLanguageLexer.MultiLineComment
                )
                {
                    var commentType =
                        type == GameMakerLanguageLexer.SingleLineComment
                            ? CommentType.SingleLine
                            : CommentType.MultiLine;

                    if (commentType == CommentType.SingleLine)
                    {
                        if (isTrailing)
                        {
                            parts.Add(Doc.TrailingComment(tokens[i].Text, commentType));
                        }
                        else
                        {
                            parts.Add(Doc.LeadingComment(tokens[i].Text, commentType));
                        }
                    }
                    else
                    {
                        parts.Add(tokens[i].Text);
                    }

                    if (firstCommentIndex == -1)
                    {
                        firstCommentIndex = parts.Count;
                    }
                    lastCommentIndex = parts.Count;
                }
            }

            if (firstCommentIndex == -1)
            {
                return Doc.Null;
            }

            var trimmedParts = parts.GetRange(
                firstCommentIndex - 1,
                lastCommentIndex - firstCommentIndex + 1
            );

            return Doc.Concat(trimmedParts);
        }

        bool IsNextLineBlank(ParserRuleContext context)
        {
            var hiddenTokens = _tokens.GetHiddenTokensToRight(context.Stop.TokenIndex);

            if (hiddenTokens == null)
            {
                return false;
            }

            int lineCount = 0;
            for (int i = 0; i < hiddenTokens.Count; i++)
            {
                if (
                    lineCount == 1
                    && (
                        hiddenTokens[i].Type == GameMakerLanguageLexer.SingleLineComment
                        || hiddenTokens[i].Type == GameMakerLanguageLexer.MultiLineComment
                    )
                )
                {
                    return false;
                }

                if (
                    hiddenTokens[i].Type == GameMakerLanguageLexer.LineTerminator
                    && !hiddenTokens[i].Text.Contains('\r')
                )
                {
                    lineCount++;
                }
            }

            return lineCount >= 2;
        }

        bool IsSeparator(IToken token)
        {
            return (
                token.Type == GameMakerLanguageLexer.Comma
                || token.Type == GameMakerLanguageLexer.SemiColon
            );
        }

        bool CanAttachComments(ParserRuleContext context)
        {
            return !(
                context is GameMakerLanguageParser.ProgramContext
                || context is GameMakerLanguageParser.StatementListContext
            );
        }
    }
}
