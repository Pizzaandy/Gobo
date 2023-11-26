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
    public IToken CurrentToken
    {
        get => token;
    }

    public List<CommentGroup> CommentGroups { get; private set; } = new();

    public int LineNumber { get; private set; } = 1;
    public int ColumnNumber { get; private set; } = 1;
    public List<GmlSyntaxError> Errors { get; private set; } = new();

    private IToken token;
    private IToken accepted;
    private readonly Lexer lexer;
    private readonly List<IToken> currentCommentGroup = new();
    private readonly Stack<int> markers = new();
    private bool HitEOF => token.Type == Lexer.Eof;

    private delegate bool BinaryExpressionRule(out GmlSyntaxNode node);

    public GmlParser(string code)
    {
        ICharStream stream = CharStreams.fromString(code);
        lexer = new Lexer(stream);
        token = lexer.NextToken();
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
    }

    private bool Accept(int type, bool skipWhitespace = true)
    {
        if (skipWhitespace)
        {
            ConsumeHiddenTokens();
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

    private static TextSpan GetSpan(IToken firstToken, IToken lastTokenExclusive)
    {
        return new TextSpan(firstToken.StartIndex, lastTokenExclusive.StartIndex);
    }

    private static TextSpan GetSpan(IToken token)
    {
        return new TextSpan(token.StartIndex, token.StopIndex + 1);
    }

    private void ConsumeHiddenTokens(bool stopOnLineBreak = false)
    {
        void AcceptCommentGroup()
        {
            if (currentCommentGroup.Count > 0)
            {
                CommentGroups.Add(
                    new CommentGroup(
                        currentCommentGroup,
                        new TextSpan(
                            currentCommentGroup.First().StartIndex,
                            currentCommentGroup.Last().StopIndex
                        )
                    )
                );
            }
        }

        while (!HitEOF)
        {
            if (token.Type == Lexer.WhiteSpaces) { }
            else if (token.Type == Lexer.LineTerminator)
            {
                AcceptCommentGroup();
                if (stopOnLineBreak)
                {
                    break;
                }
            }
            else if (token.Type == Lexer.SingleLineComment || token.Type == Lexer.MultiLineComment)
            {
                currentCommentGroup.Add(token);
            }
            else
            {
                break;
            }

            NextToken();
        }

        AcceptCommentGroup();
    }

    private void AddError(string message)
    {
        var positionMessage = $"Syntax error at line {LineNumber}, column {ColumnNumber}:\n";
        Errors.Add(new GmlSyntaxError(positionMessage + message));
        throw new GmlSyntaxErrorException(Errors.Last().Message);
    }

    private void Unexpected(IToken token)
    {
        var symbolText = token.Text == "<EOF>" ? "end of file" : $"'{token.Text}'";

        string offendingSymbolMessage = $"unexpected {symbolText}";

        AddError(offendingSymbolMessage);
    }

    private void AddDefaultSyntaxError()
    {
        Unexpected(CurrentToken);
    }

    private void Document(out GmlSyntaxNode node)
    {
        var start = token;

        var statements = AcceptStatementList();

        if (!HitEOF)
        {
            AddDefaultSyntaxError();
        }

        node = new Document(GetSpan(start, token), statements);
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
            else if (Statement(out var result))
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

    private bool Statement(out GmlSyntaxNode result)
    {
        if (Block(out result) || AssignmentOrExpressionStatement(out result))
        {
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

        result = new Block(GetSpan(start, token), statements);
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

            result = new VariableDeclarationList(GetSpan(start, token), declarations, modifier);
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
                AddError($"expected an assignment, got {token.Text}");
            }

            var assignmentOperator = accepted.Text;

            Expect(Expression(out var right));

            result = new AssignmentExpression(
                GetSpan(start, token),
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

    private bool VariableDeclarator(out GmlSyntaxNode result)
    {
        var start = token;

        if (!Accept(Lexer.Identifier))
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        var id = new Identifier(GetSpan(accepted), accepted.Text);

        GmlSyntaxNode expression = GmlSyntaxNode.Empty;
        if (Accept(Lexer.Assign))
        {
            Expect(Expression(out expression));
        }

        result = new VariableDeclarator(GetSpan(start, token), id, GmlSyntaxNode.Empty, expression);
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
                    GetSpan(start, token),
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
                    GetSpan(start, token),
                    @object,
                    expressions,
                    accessor
                );
            }
            else if (Accept(Lexer.Dot))
            {
                Expect(Lexer.Identifier);
                @object = new MemberDotExpression(
                    GetSpan(start, token),
                    @object,
                    new Identifier(GetSpan(accepted), accepted.Text)
                );
            }
            else if (Arguments(out var arguments))
            {
                @object = new CallExpression(GetSpan(start, token), @object, arguments);
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
        else if (Accept(Lexer.Identifier))
        {
            result = new Identifier(GetSpan(accepted), accepted.Text);
        }
        else if (Accept(Lexer.New))
        {
            GmlSyntaxNode id = GmlSyntaxNode.Empty;
            if (Accept(Lexer.Identifier))
            {
                id = new Identifier(GetSpan(accepted), accepted.Text);
            }
            Expect(Arguments(out var arguments));
            result = new NewExpression(GetSpan(start, token), id, arguments);
        }
        else if (Accept(Lexer.OpenParen))
        {
            Expect(Expression(out var expression));
            Expect(Lexer.CloseParen);
            result = new ParenthesizedExpression(GetSpan(start, token), expression);
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

            result = new ConditionalExpression(GetSpan(start, token), result, whenTrue, whenFalse);
        }

        return true;
    }

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
            result = new BinaryExpression(GetSpan(start, token), @operator, result, right);
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
            if (token.Type == Lexer.WhiteSpaces || token.Type == Lexer.LineTerminator)
            {
                Unexpected(accepted);
            }

            var @operator = accepted.Text;
            Expect(PrimaryExpression(out var primaryExpression));
            result = new UnaryExpression(
                GetSpan(start, token),
                @operator,
                primaryExpression,
                isPrefix: true
            );
            return true;
        }

        return PrimaryExpression(out result);
    }

    #endregion

    private bool Arguments(out GmlSyntaxNode result)
    {
        var start = token;

        if (!Accept(Lexer.OpenParen))
        {
            result = GmlSyntaxNode.Empty;
            return false;
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

        result = new ArgumentList(GetSpan(start, token), arguments);
        return true;
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
        else
        {
            result = GmlSyntaxNode.Empty;
            return false;
        }

        return true;
    }
}
