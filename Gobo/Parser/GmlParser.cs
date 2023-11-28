using Antlr4.Runtime;
using Gobo.SyntaxNodes;
using Gobo.SyntaxNodes.Gml;
using Gobo.SyntaxNodes.Gml.Literals;
using Lexer = GameMakerLanguageLexer;

namespace Gobo.Parser;

internal struct GmlParseResult
{
    public GmlSyntaxNode Ast;
    public List<GmlSyntaxError> Errors;
    public List<CommentGroup> CommentGroups;
}

internal class GmlSyntaxErrorException : Exception
{
    public GmlSyntaxErrorException(string message)
        : base(message) { }
}

internal readonly struct GmlSyntaxError
{
    public string Message { get; init; }

    public GmlSyntaxError(string message)
    {
        Message = message;
    }
}

internal class GmlParser
{
    public IToken CurrentToken => token;

    public List<CommentGroup> CommentGroups { get; private set; } = new();

    public int LineNumber { get; private set; } = 1;
    public int ColumnNumber { get; private set; } = 1;
    public List<GmlSyntaxError> Errors { get; private set; } = new();
    public bool Strict { get; set; } = false;

    private IToken token;
    private IToken accepted;
    private readonly Lexer lexer;
    private List<IToken> currentCommentGroup = new();
    private bool HitEOF => token.Type == Lexer.Eof;

    private delegate bool BinaryExpressionRule(out GmlSyntaxNode node);

    public GmlParser(string code)
    {
        ICharStream stream = CharStreams.fromString(code);
        lexer = new Lexer(stream);
        token = lexer.NextToken();
        ProcessToken(token);
    }

    public GmlParseResult Parse()
    {
        Document(out var ast);
        return new GmlParseResult()
        {
            Ast = ast,
            CommentGroups = CommentGroups,
            Errors = Errors
        };
    }

    private void NextToken()
    {
        token = lexer.NextToken();
        LineNumber = token.Line;
        ColumnNumber = token.Column;
        ProcessToken(token);
    }

    private bool Accept(int type, bool skipWhitespace = true)
    {
        if (skipWhitespace)
        {
            while (!HitEOF && IsHiddenToken(token))
            {
                NextToken();
            }
        }

        if (token.Type == type)
        {
            accepted = token;
            NextToken();
            return true;
        }

        return false;
    }

    private void Expect(int type, bool skipWhitespace = true)
    {
        if (!Accept(type, skipWhitespace))
        {
            AddDefaultSyntaxError();
        }
    }

    private void Expect(bool returnValue)
    {
        if (!returnValue)
        {
            AddDefaultSyntaxError();
        }
    }

    private bool AcceptAny(params int[] types)
    {
        foreach (var type in types)
        {
            if (Accept(type))
            {
                return true;
            }
        }
        return false;
    }

    private static TextSpan GetSpan(IToken firstToken, IToken lastToken)
    {
        return new TextSpan(firstToken.StartIndex, lastToken.StartIndex + 1);
    }

    private static TextSpan GetSpan(IToken token)
    {
        return new TextSpan(token.StartIndex, token.StopIndex + 1);
    }

    private static bool IsHiddenToken(IToken tok)
    {
        return tok.Type == Lexer.WhiteSpaces
            || tok.Type == Lexer.LineTerminator
            || tok.Type == Lexer.SingleLineComment
            || tok.Type == Lexer.MultiLineComment;
    }

    private void ProcessToken(IToken tok)
    {
        switch (tok.Type)
        {
            case Lexer.WhiteSpaces:
                if (currentCommentGroup.Count > 0)
                {
                    currentCommentGroup.Add(tok);
                }
                break;
            case Lexer.LineTerminator:
            case Lexer.Eof:
                AcceptCommentGroup();
                break;
            case Lexer.SingleLineComment:
            case Lexer.MultiLineComment:
                currentCommentGroup.Add(tok);
                break;
        }
    }

    private void AcceptCommentGroup()
    {
        if (currentCommentGroup.Count == 0)
        {
            return;
        }

        if (currentCommentGroup.Count > 1)
        {
            var group = currentCommentGroup.AsEnumerable();
            while (
                group.Last().Type == Lexer.WhiteSpaces || group.Last().Type == Lexer.LineTerminator
            )
            {
                group = group.SkipLast(1);
            }
            currentCommentGroup = group.ToList();
        }

        CommentGroups.Add(
            new CommentGroup(
                new(currentCommentGroup),
                new TextSpan(
                    currentCommentGroup.First().StartIndex,
                    currentCommentGroup.Last().StopIndex + 1
                )
            )
        );

        currentCommentGroup.Clear();
    }

    private void AddError(string message, IToken? token = null)
    {
        token ??= this.token;
        var positionMessage = $"Syntax error at line {token.Line}, column {token.Column}:\n";
        Errors.Add(new GmlSyntaxError(positionMessage + message));
        throw new GmlSyntaxErrorException(Errors.Last().Message);
    }

    private void ThrowUnexpected(IToken token)
    {
        var symbolText = token.Text == "<EOF>" ? "end of file" : $"'{token.Text}'";
        var offendingSymbolMessage = $"unexpected {symbolText}";
        AddError(offendingSymbolMessage);
    }

    private void ThrowExpected(string symbol, IToken location)
    {
        var offendingSymbolMessage = $"expected '{symbol}'";
        AddError(offendingSymbolMessage);
    }

    private void AddDefaultSyntaxError()
    {
        ThrowUnexpected(CurrentToken);
    }

    private void Document(out GmlSyntaxNode node)
    {
        var start = token;

        var statements = AcceptStatementList();

        node = new Document(GetSpan(start, accepted), statements);
    }

    private List<GmlSyntaxNode> AcceptStatementList()
    {
        var statements = new List<GmlSyntaxNode>();

        while (!HitEOF)
        {
            if (Accept(Lexer.SemiColon))
            {
                continue;
            }

            if (Statement(out var result, acceptSemicolons: false))
            {
                statements.Add(result);
            }
            else
            {
                break;
            }
        }

        return statements;
    }

    #region Statements

    private bool Statement(out GmlSyntaxNode result, bool acceptSemicolons = true)
    {
        if (
            Block(out result)
            || AssignmentOrExpressionStatement(out result)
            || IfStatement(out result)
            || FunctionDeclaration(out result)
            || DoStatement(out result)
            || WhileStatement(out result)
            || WithStatement(out result)
            || RepeatStatement(out result)
            || ForStatement(out result)
            || ThrowStatement(out result)
            || ContinueStatement(out result)
            || BreakStatement(out result)
            || ExitStatement(out result)
            || ReturnStatement(out result)
            || RegionStatement(out result)
            || SwitchStatement(out result)
            || TryStatement(out result)
            || DefineStatement(out result)
            || DeleteStatement(out result)
            || EnumeratorDeclaration(out result)
            || MacroStatement(out result)
        )
        {
            if (acceptSemicolons)
            {
                while (!HitEOF)
                {
                    if (!Accept(Lexer.SemiColon))
                    {
                        break;
                    }
                }
            }

            return true;
        }
        else
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }
    }

    private bool Block(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.OpenBrace))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        var statements = AcceptStatementList();

        Expect(Lexer.CloseBrace);

        result = new Block(GetSpan(start, accepted), statements);
        return true;
    }

    private bool FunctionDeclaration(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.Function))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        Identifier(out var identifier);
        Expect(ParameterList(out var parameters));

        GmlSyntaxNode constructorClause = GmlSyntaxNode.Empty;

        if (Accept(Lexer.Colon))
        {
            GmlSyntaxNode parentName = GmlSyntaxNode.Empty;
            GmlSyntaxNode parentArgs = GmlSyntaxNode.Empty;
            // Parent name cannot be 'constructor'
            Expect(Lexer.Identifier);
            parentName = new Identifier(GetSpan(accepted), accepted.Text);

            Expect(ArgumentList(out parentArgs));
            Expect(Lexer.Constructor);

            constructorClause = new ConstructorClause(
                GetSpan(start, accepted),
                parentName,
                parentArgs
            );
        }
        else if (Accept(Lexer.Constructor))
        {
            constructorClause = new ConstructorClause(
                GetSpan(start, accepted),
                GmlSyntaxNode.Empty,
                GmlSyntaxNode.Empty
            );
        }

        Expect(Statement(out var body));

        result = new FunctionDeclaration(
            GetSpan(start, accepted),
            identifier,
            parameters,
            body,
            constructorClause
        );
        return true;
    }

    private bool AssignmentOrExpressionStatement(out GmlSyntaxNode result)
    {
        var start = token;
        result = GmlSyntaxNode.Empty;

        if (AcceptAny(Lexer.Var, Lexer.Static, Lexer.GlobalVar))
        {
            // Variable declaration list
            var modifier = accepted.Text;

            while (Accept(Lexer.Var))
            {
                continue;
            }

            if (!VariableDeclarator(out var firstDeclaration))
            {
                AddDefaultSyntaxError();
                return false;
            }

            var declarations = new List<GmlSyntaxNode>() { firstDeclaration };

            while (!HitEOF)
            {
                if (Accept(Lexer.Comma))
                {
                    Expect(VariableDeclarator(out var variableDeclarator));
                    declarations.Add(variableDeclarator);
                }
                else
                {
                    break;
                }
            }

            result = new VariableDeclarationList(GetSpan(start, accepted), declarations, modifier);
        }
        else if (UnaryExpression(out var left))
        {
            // expression statement
            if (left is UnaryExpression { Operator: "++" or "--" } or CallExpression)
            {
                result = left;
                return true;
            }

            // assignment
            if (
                !AcceptAny(
                    Lexer.Assign,
                    Lexer.MultiplyAssign,
                    Lexer.DivideAssign,
                    Lexer.PlusAssign,
                    Lexer.MinusAssign,
                    Lexer.ModulusAssign,
                    Lexer.LeftShiftArithmeticAssign,
                    Lexer.RightShiftArithmeticAssign,
                    Lexer.BitAndAssign,
                    Lexer.BitXorAssign,
                    Lexer.BitOrAssign
                )
            )
            {
                // uncomment to support identifiers as statements
                //if (left is Identifier)
                //{
                //    result = left;
                //    return true;
                //}
                AddError($"unexpected expression", start);
            }

            var assignmentOperator = accepted.Text;

            Expect(Expression(out var right));

            result = new AssignmentExpression(
                GetSpan(start, accepted),
                assignmentOperator,
                left,
                right,
                GmlSyntaxNode.Empty
            );
        }
        else
        {
            return false;
        }

        return true;
    }

    private bool IfStatement(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.If))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        Expect(Expression(out var condition));
        Accept(Lexer.Then);
        Expect(Statement(out var statement));

        GmlSyntaxNode alternate = GmlSyntaxNode.Empty;
        if (Accept(Lexer.Else))
        {
            Expect(Statement(out alternate));
        }

        result = new IfStatement(GetSpan(start, accepted), condition, statement, alternate);
        return true;
    }

    private bool DoStatement(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.Do))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        Expect(Statement(out var body));
        Expect(Lexer.Until);
        Expect(Expression(out var test));

        result = new DoStatement(GetSpan(start, accepted), body, test);
        return true;
    }

    private bool WhileStatement(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.While))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        Expect(Expression(out var test));
        Expect(Statement(out var body));

        result = new WhileStatement(GetSpan(start, accepted), test, body);
        return true;
    }

    private bool RepeatStatement(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.Repeat))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        Expect(Expression(out var test));
        Expect(Statement(out var body));

        result = new RepeatStatement(GetSpan(start, accepted), test, body);
        return true;
    }

    private bool WithStatement(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.With))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        Expect(Expression(out var @object));
        Expect(Statement(out var body));

        result = new WithStatement(GetSpan(start, accepted), @object, body);
        return true;
    }

    private bool ForStatement(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.For))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        Expect(Lexer.OpenParen);
        Statement(out var init, acceptSemicolons: false);
        Expect(Lexer.SemiColon);
        Expression(out var test);
        Expect(Lexer.SemiColon);
        Statement(out var update, acceptSemicolons: true);
        Expect(Lexer.CloseParen);
        Expect(Statement(out var body));

        result = new ForStatement(GetSpan(start, accepted), init, test, update, body);
        return true;
    }

    private bool SwitchStatement(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.Switch))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        Expect(Expression(out var condition));

        var blockStart = token;
        Expect(Lexer.OpenBrace);

        var cases = new List<GmlSyntaxNode>();
        while (!HitEOF)
        {
            if (SwitchCase(out var switchCase))
            {
                cases.Add(switchCase);
            }
            else
            {
                break;
            }
        }

        Expect(Lexer.CloseBrace);

        var caseBlock = new SwitchBlock(GetSpan(blockStart, token), cases);
        result = new SwitchStatement(GetSpan(start, accepted), condition, caseBlock);
        return true;
    }

    private bool SwitchCase(out GmlSyntaxNode result)
    {
        var start = token;
        if (Accept(Lexer.Case))
        {
            Expect(Expression(out var test));
            Expect(Lexer.Colon);
            var statements = AcceptStatementList();
            result = new SwitchCase(GetSpan(start, accepted), test, statements);
        }
        else if (Accept(Lexer.Default))
        {
            Expect(Lexer.Colon);
            var statements = AcceptStatementList();
            result = new SwitchCase(GetSpan(start, accepted), GmlSyntaxNode.Empty, statements);
        }
        else
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        return true;
    }

    private bool ContinueStatement(out GmlSyntaxNode result)
    {
        if (!Accept(Lexer.Continue))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }
        result = new ContinueStatement(GetSpan(accepted));
        return true;
    }

    private bool BreakStatement(out GmlSyntaxNode result)
    {
        if (!Accept(Lexer.Break))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }
        result = new BreakStatement(GetSpan(accepted));
        return true;
    }

    private bool ExitStatement(out GmlSyntaxNode result)
    {
        if (!Accept(Lexer.Exit))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }
        result = new ExitStatement(GetSpan(accepted));
        return true;
    }

    private bool ReturnStatement(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.Return))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }
        Expression(out var argument);
        result = new ReturnStatement(GetSpan(start, accepted), argument);
        return true;
    }

    private bool ThrowStatement(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.Throw))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }
        Expression(out var argument);
        result = new ThrowStatement(GetSpan(start, accepted), argument);
        return true;
    }

    private bool DeleteStatement(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.Delete))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }
        Expression(out var argument);
        result = new DeleteStatement(GetSpan(start, accepted), argument);
        return true;
    }

    private bool RegionStatement(out GmlSyntaxNode result)
    {
        var start = token;
        result = GmlSyntaxNode.Empty;
        if (!AcceptAny(Lexer.Region, Lexer.EndRegion))
        {
            return false;
        }

        bool isEndRegion = accepted.Type == Lexer.EndRegion;
        string? name = null;

        var tokens = new List<string>();

        while (!HitEOF)
        {
            if (Accept(Lexer.RegionCharacters))
            {
                tokens.Add(accepted.Text);
            }
            else if (Accept(Lexer.RegionEOL))
            {
                break;
            }
            else
            {
                AddDefaultSyntaxError();
            }
        }

        name = tokens.Count > 0 ? string.Join("", tokens).TrimEnd() : null;

        result = new RegionStatement(GetSpan(start, accepted), name, isEndRegion);
        return true;
    }

    private bool DefineStatement(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.Define))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        var tokens = new List<string>();

        while (!HitEOF)
        {
            if (AcceptAny(Lexer.RegionCharacters, Lexer.RegionEOL))
            {
                tokens.Add(accepted.Text);
            }
            else
            {
                break;
            }
        }

        var name = string.Join("", tokens).TrimEnd();
        result = new DefineStatement(GetSpan(start, accepted), name);
        return true;
    }

    private bool MacroStatement(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.Macro))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        Expect(Lexer.Identifier);
        var name = accepted.Text;

        var tokens = new List<string>();
        bool ignoreNextLineBreak = false;

        Accept(Lexer.WhiteSpaces);

        while (!HitEOF)
        {
            string tokenText;

            if (Accept(Lexer.LineTerminator, skipWhitespace: false))
            {
                if (ignoreNextLineBreak)
                {
                    tokenText = accepted.Text;
                    ignoreNextLineBreak = false;
                }
                else
                {
                    break;
                }
            }
            else if (Accept(Lexer.EscapedNewLine, skipWhitespace: false))
            {
                tokenText = @"\";
                ignoreNextLineBreak = true;
            }
            else if (Accept(Lexer.WhiteSpaces, skipWhitespace: false))
            {
                tokenText = accepted.Text;
            }
            else
            {
                tokenText = token.Text;
                ignoreNextLineBreak = false;
                NextToken();
            }

            tokens.Add(tokenText);

            if (token.Type == Lexer.Eof)
            {
                break;
            }
        }

        result = new MacroDeclaration(
            GetSpan(start, accepted),
            name,
            string.Join("", tokens).TrimEnd()
        );
        return true;
    }

    private bool TryStatement(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.Try))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        GmlSyntaxNode catchProduction = GmlSyntaxNode.Empty;
        GmlSyntaxNode finallyProduction = GmlSyntaxNode.Empty;

        Expect(Statement(out var tryBody));

        if (Accept(Lexer.Catch))
        {
            GmlSyntaxNode identifier = GmlSyntaxNode.Empty;
            if (Accept(Lexer.OpenParen))
            {
                Expect(Identifier(out identifier));
                Expect(Lexer.CloseParen);
            }
            Expect(Statement(out var body));
            catchProduction = new CatchProduction(GetSpan(start, accepted), identifier, body);
        }

        if (Accept(Lexer.Finally))
        {
            Expect(Statement(out var finallyBody));
            finallyProduction = new FinallyProduction(GetSpan(start, accepted), finallyBody);
        }

        result = new TryStatement(
            GetSpan(start, accepted),
            tryBody,
            catchProduction,
            finallyProduction
        );
        return true;
    }

    private bool EnumeratorDeclaration(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.Enum))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        Expect(Identifier(out var name));

        var startBlock = token;
        Expect(Lexer.OpenBrace);

        if (Accept(Lexer.CloseBrace))
        {
            result = new EnumDeclaration(
                GetSpan(start, accepted),
                name,
                new EnumBlock(GetSpan(startBlock, token), new())
            );
            return true;
        }

        var enumMembers = new List<GmlSyntaxNode>();
        bool expectDelimiter = false;

        while (!HitEOF)
        {
            if (expectDelimiter)
            {
                Expect(Lexer.Comma);
            }
            else
            {
                Expect(EnumMember(out var enumMember));
                enumMembers.Add(enumMember);
            }
            expectDelimiter = !expectDelimiter;

            if (Accept(Lexer.CloseBrace))
            {
                break;
            }
        }

        if (accepted.Type != Lexer.CloseBrace)
        {
            ThrowExpected("}", accepted);
        }

        var enumBlock = new EnumBlock(GetSpan(startBlock, token), enumMembers);
        result = new EnumDeclaration(GetSpan(start, accepted), name, enumBlock);
        return true;
    }

    private bool EnumMember(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Identifier(out var identifier))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        GmlSyntaxNode expression = GmlSyntaxNode.Empty;
        if (Accept(Lexer.Assign))
        {
            Expect(Expression(out expression));
        }

        result = new EnumMember(GetSpan(start, accepted), identifier, expression);
        return true;
    }

    private bool VariableDeclarator(out GmlSyntaxNode result)
    {
        var start = token;

        if (!Identifier(out var identifier))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }
        GmlSyntaxNode expression = GmlSyntaxNode.Empty;
        if (Accept(Lexer.Assign))
        {
            Expect(Expression(out expression));
        }

        result = new VariableDeclarator(
            GetSpan(start, accepted),
            identifier,
            GmlSyntaxNode.Empty,
            expression
        );
        return true;
    }

    #endregion

    #region Expressions
    private bool PrimaryExpression(out GmlSyntaxNode result)
    {
        var start = token;

        if (!PrimaryExpressionStart(out var startExpression))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        GmlSyntaxNode @object = startExpression;

        while (!HitEOF)
        {
            if (
                Accept(Lexer.PlusPlus, skipWhitespace: false)
                || Accept(Lexer.MinusMinus, skipWhitespace: false)
            )
            {
                var @operator = accepted.Text;
                @object = new UnaryExpression(
                    GetSpan(start, accepted),
                    @operator,
                    @object,
                    isPrefix: false
                );
                break;
            }
            else if (
                AcceptAny(
                    Lexer.OpenBracket,
                    Lexer.ArrayAccessor,
                    Lexer.ListAccessor,
                    Lexer.MapAccessor,
                    Lexer.GridAccessor,
                    Lexer.ArrayAccessor,
                    Lexer.StructAccessor
                )
            )
            {
                var accessor = accepted.Text;
                var expressions = new List<GmlSyntaxNode>();
                Expect(Expression(out var firstExpression));
                expressions.Add(firstExpression);

                while (!HitEOF)
                {
                    if (Accept(Lexer.Comma))
                    {
                        Expect(Expression(out var expression));
                        expressions.Add(expression);
                    }
                    else if (Accept(Lexer.CloseBracket))
                    {
                        break;
                    }
                    else
                    {
                        AddDefaultSyntaxError();
                    }
                }

                @object = new MemberIndexExpression(
                    GetSpan(start, accepted),
                    @object,
                    expressions,
                    accessor
                );
            }
            else if (Accept(Lexer.Dot))
            {
                Expect(Identifier(out var identifier));
                @object = new MemberDotExpression(GetSpan(start, accepted), @object, identifier);
            }
            else if (ArgumentList(out var arguments))
            {
                @object = new CallExpression(GetSpan(start, accepted), @object, arguments);
            }
            else
            {
                break;
            }
        }

        result = @object;
        return true;
    }

    private bool PrimaryExpressionStart(out GmlSyntaxNode result)
    {
        var start = token;

        if (Literal(out var literal))
        {
            result = literal;
        }
        else if (Identifier(out var identifier))
        {
            result = identifier;
        }
        else if (Accept(Lexer.New))
        {
            GmlSyntaxNode id = GmlSyntaxNode.Empty;
            if (Identifier(out var constructorName))
            {
                id = constructorName;
            }
            Expect(ArgumentList(out var arguments));
            result = new NewExpression(GetSpan(start, accepted), id, arguments);
        }
        else if (Accept(Lexer.OpenParen))
        {
            Expect(Expression(out var expression));
            Expect(Lexer.CloseParen);
            result = new ParenthesizedExpression(GetSpan(start, accepted), expression);
        }
        else
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        return true;
    }

    private bool Expression(out GmlSyntaxNode result)
    {
        if (FunctionDeclaration(out result))
        {
            return true;
        }
        return ConditionalExpression(out result);
    }

    private bool ConditionalExpression(out GmlSyntaxNode result)
    {
        var start = token;

        if (!(BitXorExpression(out result)))
        {
            return false;
        }

        if (Accept(Lexer.QuestionMark))
        {
            Expect(Expression(out var whenTrue));
            Expect(Lexer.Colon);
            Expect(Expression(out var whenFalse));

            result = new ConditionalExpression(
                GetSpan(start, accepted),
                result,
                whenTrue,
                whenFalse
            );
        }

        return true;
    }

    // TODO: optimize if needed
    private bool HandleBinaryExpression(
        BinaryExpressionRule nextRule,
        out GmlSyntaxNode result,
        params int[] tokenTypes
    )
    {
        var start = token;

        if (!nextRule(out result))
        {
            return false;
        }

        while (AcceptAny(tokenTypes))
        {
            var @operator = accepted.Text;
            Expect(nextRule(out var right));
            result = new BinaryExpression(GetSpan(start, accepted), @operator, result, right);
        }

        return true;
    }

    private bool BitXorExpression(out GmlSyntaxNode result)
    {
        return HandleBinaryExpression(BitOrExpression, out result, Lexer.BitXor);
    }

    private bool BitOrExpression(out GmlSyntaxNode result)
    {
        return HandleBinaryExpression(BitAndExpression, out result, Lexer.BitOr);
    }

    private bool BitAndExpression(out GmlSyntaxNode result)
    {
        return HandleBinaryExpression(NullCoalescingExpression, out result, Lexer.BitAnd);
    }

    private bool NullCoalescingExpression(out GmlSyntaxNode result)
    {
        return HandleBinaryExpression(XorExpression, out result, Lexer.NullCoalesce);
    }

    private bool XorExpression(out GmlSyntaxNode result)
    {
        return HandleBinaryExpression(AndExpression, out result, Lexer.Xor);
    }

    private bool AndExpression(out GmlSyntaxNode result)
    {
        return HandleBinaryExpression(OrExpression, out result, Lexer.And);
    }

    private bool OrExpression(out GmlSyntaxNode result)
    {
        return HandleBinaryExpression(EqualityExpression, out result, Lexer.Or);
    }

    private bool EqualityExpression(out GmlSyntaxNode result)
    {
        return HandleBinaryExpression(
            RelationalExpression,
            out result,
            Lexer.Equals_,
            Lexer.Assign,
            Lexer.NotEquals
        );
    }

    private bool RelationalExpression(out GmlSyntaxNode result)
    {
        return HandleBinaryExpression(
            ShiftExpression,
            out result,
            Lexer.LessThan,
            Lexer.GreaterThan,
            Lexer.LessThanEquals,
            Lexer.GreaterThanEquals
        );
    }

    private bool ShiftExpression(out GmlSyntaxNode result)
    {
        return HandleBinaryExpression(
            AdditiveExpression,
            out result,
            Lexer.LeftShiftArithmetic,
            Lexer.RightShiftArithmetic
        );
    }

    private bool AdditiveExpression(out GmlSyntaxNode result)
    {
        return HandleBinaryExpression(
            MultiplicativeExpression,
            out result,
            Lexer.Plus,
            Lexer.Minus
        );
    }

    private bool MultiplicativeExpression(out GmlSyntaxNode result)
    {
        return HandleBinaryExpression(
            UnaryExpression,
            out result,
            Lexer.Multiply,
            Lexer.Divide,
            Lexer.Modulo,
            Lexer.IntegerDivide
        );
    }

    private bool UnaryExpression(out GmlSyntaxNode result)
    {
        var start = token;

        if (
            AcceptAny(
                Lexer.Plus,
                Lexer.Minus,
                Lexer.Not,
                Lexer.BitNot,
                Lexer.PlusPlus,
                Lexer.MinusMinus
            )
        )
        {
            // Increment/decrement operators must be adjacent to their operands
            if (
                (accepted.Type == Lexer.PlusPlus || accepted.Type == Lexer.MinusMinus)
                && (token.Type == Lexer.WhiteSpaces || token.Type == Lexer.LineTerminator)
            )
            {
                ThrowUnexpected(accepted);
            }

            var @operator = accepted.Text;
            Expect(PrimaryExpression(out var primaryExpression));
            result = new UnaryExpression(
                GetSpan(start, accepted),
                @operator,
                primaryExpression,
                isPrefix: true
            );
            return true;
        }

        return PrimaryExpression(out result);
    }

    #endregion

    private bool ParameterList(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.OpenParen))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        if (Accept(Lexer.CloseParen))
        {
            result = new ParameterList(GetSpan(start, accepted), new());
            return true;
        }

        Expect(Parameter(out var firstParameter));
        var parameters = new List<GmlSyntaxNode>() { firstParameter };

        while (!HitEOF)
        {
            if (Accept(Lexer.Comma))
            {
                Expect(Parameter(out var parameter));
                parameters.Add(parameter);
            }
            else if (Accept(Lexer.CloseParen))
            {
                break;
            }
            else
            {
                AddDefaultSyntaxError();
                result = GmlSyntaxNode.Empty;
                return false;
            }
        }

        result = new ParameterList(GetSpan(start, accepted), parameters);
        return true;
    }

    private bool Parameter(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Identifier(out var identifier))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        GmlSyntaxNode initializer = GmlSyntaxNode.Empty;

        if (Accept(Lexer.Assign))
        {
            Expect(Expression(out initializer));
        }

        result = new Parameter(
            GetSpan(start, accepted),
            identifier,
            GmlSyntaxNode.Empty,
            initializer
        );
        return true;
    }

    private bool ArgumentList(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.OpenParen))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        if (Accept(Lexer.CloseParen))
        {
            result = new ArgumentList(GetSpan(start, accepted), new());
            return true;
        }

        var arguments = new List<GmlSyntaxNode>();
        bool previousChildWasPunctuator = true;

        while (!HitEOF)
        {
            if (Expression(out var expression))
            {
                if (!previousChildWasPunctuator)
                {
                    AddError("Expected ','");
                    result = GmlSyntaxNode.Empty;
                    return false;
                }
                previousChildWasPunctuator = false;
                arguments.Add(expression);
            }

            if (Accept(Lexer.Comma))
            {
                if (previousChildWasPunctuator)
                {
                    arguments.Add(new UndefinedArgument(token.StartIndex - 1));
                }
                previousChildWasPunctuator = true;
            }
            else if (Accept(Lexer.CloseParen))
            {
                if (previousChildWasPunctuator && arguments.Count > 0)
                {
                    arguments.Add(new UndefinedArgument(token.StartIndex - 1));
                }
                break;
            }
            else
            {
                AddDefaultSyntaxError();
                result = GmlSyntaxNode.Empty;
                return false;
            }
        }

        result = new ArgumentList(GetSpan(start, accepted), arguments);
        return true;
    }

    private bool Identifier(out GmlSyntaxNode result)
    {
        result = GmlSyntaxNode.Empty;

        if (Accept(Lexer.Identifier))
        {
            result = new Identifier(GetSpan(accepted), accepted.Text);
            return true;
        }
        else if (Accept(Lexer.Constructor))
        {
            result = new Identifier(GetSpan(accepted), accepted.Text);
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool Literal(out GmlSyntaxNode result)
    {
        if (Accept(Lexer.Undefined))
        {
            result = new UndefinedLiteral(GetSpan(accepted), accepted.Text);
        }
        else if (Accept(Lexer.IntegerLiteral))
        {
            result = new IntegerLiteral(GetSpan(accepted), accepted.Text);
        }
        else if (Accept(Lexer.DecimalLiteral))
        {
            result = new DecimalLiteral(GetSpan(accepted), accepted.Text);
        }
        else if (Accept(Lexer.StringLiteral))
        {
            result = new StringLiteral(GetSpan(accepted), accepted.Text);
        }
        else if (Accept(Lexer.TemplateStringStart))
        {
            var start = token;
            var atoms = new List<GmlSyntaxNode>();

            while (!HitEOF)
            {
                if (Accept(Lexer.TemplateStringEnd))
                {
                    break;
                }
                else if (Accept(Lexer.TemplateStringStartExpression))
                {
                    var expressionStart = token;
                    Expression(out var expression);
                    Expect(Lexer.TemplateStringEndExpression);
                    atoms.Add(
                        new TemplateExpression(GetSpan(expressionStart, accepted), expression)
                    );
                }
                else if (Accept(Lexer.TemplateStringText))
                {
                    atoms.Add(new TemplateText(GetSpan(accepted), accepted.Text));
                }
                else
                {
                    result = GmlSyntaxNode.Empty;
                    return false;
                }
            }

            if (accepted.Type != Lexer.TemplateStringEnd)
            {
                ThrowExpected("\"", accepted);
            }

            result = new TemplateLiteral(GetSpan(start, accepted), atoms);
        }
        else if (
            AcceptAny(
                Lexer.NoOne,
                Lexer.BooleanLiteral,
                Lexer.VerbatimStringLiteral,
                Lexer.HexIntegerLiteral,
                Lexer.BinaryLiteral
            )
        )
        {
            result = new Literal(GetSpan(accepted), accepted.Text);
        }
        else if (ArrayLiteral(out var arrayLiteral))
        {
            result = arrayLiteral;
        }
        else if (StructLiteral(out var structLiteral))
        {
            result = structLiteral;
        }
        else
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        return true;
    }

    private bool ArrayLiteral(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.OpenBracket))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        if (Accept(Lexer.CloseBracket))
        {
            result = new ArrayExpression(GetSpan(start, accepted), new());
            return true;
        }

        var elements = new List<GmlSyntaxNode>();

        bool expectDelimiter = false;
        while (!HitEOF)
        {
            if (expectDelimiter)
            {
                Expect(Lexer.Comma);
            }
            else
            {
                Expect(Expression(out var expression));
                elements.Add(expression);
            }
            expectDelimiter = !expectDelimiter;

            if (Accept(Lexer.CloseBracket))
            {
                break;
            }
        }

        if (accepted.Type != Lexer.CloseBracket)
        {
            ThrowExpected("]", accepted);
        }

        result = new ArrayExpression(GetSpan(start, accepted), elements);
        return true;
    }

    private bool StructLiteral(out GmlSyntaxNode result)
    {
        var start = token;
        if (!Accept(Lexer.OpenBrace))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        if (Accept(Lexer.CloseBrace))
        {
            result = new StructExpression(GetSpan(start, accepted), new());
            return true;
        }

        var properties = new List<GmlSyntaxNode>();

        bool expectDelimiter = false;
        while (!HitEOF)
        {
            if (expectDelimiter)
            {
                Expect(Lexer.Comma);
            }
            else
            {
                Expect(PropertyAssignment(out var property));
                properties.Add(property);
            }
            expectDelimiter = !expectDelimiter;

            if (Accept(Lexer.CloseBrace))
            {
                break;
            }
        }

        if (accepted.Type != Lexer.CloseBrace)
        {
            ThrowExpected("}", accepted);
        }

        result = new StructExpression(GetSpan(start, accepted), properties);
        return true;
    }

    private bool PropertyAssignment(out GmlSyntaxNode result)
    {
        var start = token;
        GmlSyntaxNode name = GmlSyntaxNode.Empty;

        if (AcceptAny(Lexer.Identifier, Lexer.Constructor, Lexer.NoOne))
        {
            name = new Identifier(GetSpan(accepted), accepted.Text);
        }
        else if (Accept(Lexer.StringLiteral))
        {
            name = new StringLiteral(GetSpan(accepted), accepted.Text);
        }
        else
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        Expect(Lexer.Colon);
        Expect(Expression(out var initializer));

        result = new StructProperty(GetSpan(start, accepted), name, initializer);

        return true;
    }
}
