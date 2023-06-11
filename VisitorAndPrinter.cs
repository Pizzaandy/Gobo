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
                return Doc.Concat(new[] { leadingComments, base.Visit(tree), trailingComments });
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
                parts.Add(Doc.HardLine);
                if (i != statements.Length - 1 && IsNextLineBlank(statements[i]))
                {
                    parts.Add(Doc.HardLine);
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
                // L value expressions handle printing arguments
                if (context.callableExpression().lValueExpression() != null)
                {
                    return callee;
                }
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
                    var trailingComments = PrintTrailingComments(expressions[i]);
                    parts.Add(Visit(expressions[i]));
                    if (i != expressions.Length - 1)
                    {
                        parts.Add(",");
                        if (trailingComments is not NullDoc)
                        {
                            parts.Add(" ");
                            parts.Add(trailingComments);
                        }

                        parts.Add(Doc.Line);
                    }
                }
            }

            if (parts.Count == 0)
            {
                return "()";
            }

            return Doc.Group(
                new Doc[]
                {
                    "(",
                    Doc.Indent(new[] { Doc.SoftLine, Doc.Concat(parts) }),
                    Doc.SoftLine,
                    ")"
                }
            );
        }

        // TODO: simplify logic
        public override Doc VisitLValueExpression(
            [NotNull] GameMakerLanguageParser.LValueExpressionContext context
        )
        {
            var flatParts = new List<Doc>();
            var breakParts = new List<Doc>();
            var indentedBreakParts = new List<Doc>();

            var root = Visit(context.lValueStartExpression());
            var rootText = context.lValueStartExpression().GetText();
            bool isRootCapitalized = char.IsUpper(rootText[0]);
            bool isRootShort = rootText.Length <= 4;

            flatParts.Add(root);
            breakParts.Add(root);

            bool indentStarted = false;
            ParserRuleContext? lastChainOp = null;

            var ops = context.lValueChainOperator();

            // accumulate operations
            if (ops != null && ops.Length > 0)
            {
                for (var i = 0; i < ops.Length; i++)
                {
                    var opDoc = Visit(ops[i]);

                    if (
                        i > 0
                        && ops[i - 1] is GameMakerLanguageParser.CallLValueContext
                        && ops[i] is GameMakerLanguageParser.MemberDotLValueContext
                    )
                    {
                        indentStarted = true;
                        indentedBreakParts.Add(Doc.HardLine);
                    }

                    flatParts.Add(opDoc);

                    if (indentStarted)
                    {
                        indentedBreakParts.Add(opDoc);
                    }
                    else
                    {
                        if (
                            i < ops.Length - 1
                            && ops[i] is GameMakerLanguageParser.MemberDotLValueContext
                            && ops[i + 1] is GameMakerLanguageParser.CallLValueContext
                            && (i == 0 && !(isRootCapitalized || isRootShort))
                        )
                        {
                            indentStarted = true;
                            indentedBreakParts.Add(Doc.HardLine);
                            indentedBreakParts.Add(opDoc);
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

                if (indentStarted)
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
                var callContext = currentContext as GameMakerLanguageParser.CallStatementContext;
                var argsDoc = Visit(callContext.arguments());

                flatParts.Add(argsDoc);

                if (indentStarted)
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

            var indentedBreakDoc = Doc.Fill(Doc.JoinList(Doc.SoftLine, indentedBreakParts));

            return Doc.ConditionalGroup(
                new Doc[]
                {
                    Doc.Concat(flatParts),
                    Doc.Concat(new Doc[] { Doc.Concat(breakParts), Doc.Indent(indentedBreakDoc) }),
                }
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
            return Doc.Concat(".", Visit(context.identifier()));
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

                Console.WriteLine("Leading Index: " + (hiddenTokens[0].TokenIndex));
                Console.WriteLine(printed);

                if (printed is not NullDoc)
                {
                    return leadingSingleLineComment
                        ? Doc.Concat(new[] { printed, Doc.HardLine })
                        : Doc.Concat(new[] { printed, " " });
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

                Console.WriteLine("Trailing Index: " + (hiddenTokens[0].TokenIndex));
                Console.WriteLine(printed);

                return printed;
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

                if (type == GameMakerLanguageLexer.LineTerminator)
                {
                    if (
                        consecutiveLineBreaks < 2
                        && tokens[i].Text.Replace("\r", @"\r").Contains(@"\r")
                    )
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
                    && !hiddenTokens[i].Text.Replace("\r", @"\r").Contains(@"\r")
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
