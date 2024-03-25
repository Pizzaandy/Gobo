using System.Runtime.CompilerServices;

namespace Gobo.Parser;

internal class GmlLexer
{
    internal enum LexerMode
    {
        Default,
        TemplateString,
        RegionName,
    }

    public bool HitEof { get; private set; } = false;
    public LexerMode Mode { get; set; } = LexerMode.Default;

    private readonly string text;
    private int lineNumber;
    private int columnNumber;
    private int startIndex;
    private int index;
    private readonly int tabWidth;
    private int character;
    private string CurrentToken => text[startIndex..index];

    private static readonly char[] whitespaces = { '\u000B', '\u000C', '\u0020', '\u00A0', '\t' };

    public GmlLexer(string text, int tabWidth = 4)
    {
        this.text = text;
        this.tabWidth = tabWidth;
        index = 0;
        lineNumber = 1;
    }

    public Token NextToken()
    {
        startIndex = index;
        Advance();

        if (Mode is LexerMode.RegionName)
        {
            while (Peek() != '\r' && Peek() != '\n' && !HitEof)
            {
                Advance();
            }
            Mode = LexerMode.Default;
            return Token(TokenKind.RegionName);
        }

        if (Mode is LexerMode.TemplateString)
        {
            if (character == '"')
            {
                return Token(TokenKind.TemplateEnd);
            }
            if (character == '{')
            {
                return Token(TokenKind.TemplateMiddle);
            }

            while (IsTemplateStringCharacter(Peek()) && !HitEof)
            {
                Advance();
            }
            if (Match('{'))
            {
                return Token(TokenKind.TemplateMiddle);
            }
            if (Match('"'))
            {
                return Token(TokenKind.TemplateEnd);
            }

            return UnexpectedToken("Invalid template literal");
        }

        switch (character)
        {
            case -1:
                return Token(TokenKind.Eof);
            case '\u000B':
            case '\u000C':
            case '\u0020':
            case '\u00A0':
            case '\t':
                while (true)
                {
                    if (!MatchAny(whitespaces))
                    {
                        break;
                    }
                }
                return Token(TokenKind.Whitespace);
            case '\n':
                return Token(TokenKind.LineBreak);
            case '\r':
                if (Match('\n'))
                    return Token(TokenKind.LineBreak);
                break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\r' && Peek() != '\n' && !HitEof)
                    {
                        Advance();
                    }
                    return Token(TokenKind.SingleLineComment);
                }
                if (Match('*'))
                {
                    while (true)
                    {
                        while (Peek() != '*' && !HitEof)
                        {
                            Advance();
                        }

                        if (!Match('*'))
                        {
                            return UnexpectedToken();
                        }

                        if (Match('/'))
                        {
                            return Token(TokenKind.MultiLineComment);
                        }
                    }
                }
                if (Match('='))
                {
                    return Token(TokenKind.DivideAssign);
                }
                return Token(TokenKind.Divide);
            case '(':
                return Token(TokenKind.OpenParen);
            case ')':
                return Token(TokenKind.CloseParen);
            case '{':
                return Token(TokenKind.OpenBrace);
            case '}':
                return Token(TokenKind.CloseBrace);
            case ';':
                return Token(TokenKind.SemiColon);
            case ',':
                return Token(TokenKind.Comma);
            case '.':
                if (IsDigit(Peek()))
                {
                    while (IsDigit(Peek()) && !HitEof)
                    {
                        Advance();
                    }
                    return Token(TokenKind.DecimalLiteral);
                }
                return Token(TokenKind.Dot);
            case '\\':
                return Token(TokenKind.Backslash);
            case '=':
                if (Match('='))
                {
                    return Token(TokenKind.Equals);
                }
                return Token(TokenKind.Assign);
            case ':':
                if (Match('='))
                {
                    return Token(TokenKind.Assign);
                }
                return Token(TokenKind.Colon);
            case '+':
                if (Match('+'))
                {
                    return Token(TokenKind.PlusPlus);
                }
                if (Match('='))
                {
                    return Token(TokenKind.PlusAssign);
                }
                return Token(TokenKind.Plus);
            case '-':
                if (Match('-'))
                {
                    return Token(TokenKind.MinusMinus);
                }
                if (Match('='))
                {
                    return Token(TokenKind.MinusAssign);
                }
                return Token(TokenKind.Minus);
            case '*':
                if (Match('='))
                {
                    return Token(TokenKind.MultiplyAssign);
                }
                if (Match('*'))
                {
                    return Token(TokenKind.Power);
                }
                return Token(TokenKind.Multiply);
            case '%':
                if (Match('='))
                {
                    return Token(TokenKind.ModulusAssign);
                }
                return Token(TokenKind.Modulo);
            case '~':
                return Token(TokenKind.BitNot);
            case '?':
                if (Match('?'))
                {
                    if (Match('='))
                    {
                        return Token(TokenKind.NullCoalescingAssign);
                    }
                    return Token(TokenKind.NullCoalesce);
                }
                return Token(TokenKind.QuestionMark);
            case '!':
                if (Match('='))
                {
                    return Token(TokenKind.NotEquals);
                }
                return Token(TokenKind.Not);
            case '[':
                if (Match('|'))
                {
                    return Token(TokenKind.ListAccessor);
                }
                else if (Match('?'))
                {
                    return Token(TokenKind.MapAccessor);
                }
                else if (Match('#'))
                {
                    return Token(TokenKind.GridAccessor);
                }
                else if (Match('@'))
                {
                    return Token(TokenKind.ArrayAccessor);
                }
                else if (Match('$'))
                {
                    return Token(TokenKind.StructAccessor);
                }
                return Token(TokenKind.OpenBracket);
            case ']':
                return Token(TokenKind.CloseBracket);
            case '<':
                if (Match('<'))
                {
                    if (Match('='))
                    {
                        return Token(TokenKind.LeftShiftArithmeticAssign);
                    }
                    return Token(TokenKind.LeftShiftArithmetic);
                }
                if (Match('>'))
                {
                    return Token(TokenKind.NotEquals);
                }
                if (Match('='))
                {
                    return Token(TokenKind.LessThanEquals);
                }
                return Token(TokenKind.LessThan);
            case '>':
                if (Match('>'))
                {
                    if (Match('='))
                    {
                        return Token(TokenKind.RightShiftArithmeticAssign);
                    }
                    return Token(TokenKind.RightShiftArithmetic);
                }
                if (Match('='))
                {
                    return Token(TokenKind.GreaterThanEquals);
                }
                return Token(TokenKind.GreaterThan);
            case '|':
                if (Match('|'))
                {
                    return Token(TokenKind.Or);
                }
                if (Match('='))
                {
                    return Token(TokenKind.BitOrAssign);
                }
                return Token(TokenKind.BitOr);
            case '&':
                if (Match('&'))
                {
                    return Token(TokenKind.And);
                }
                if (Match('='))
                {
                    return Token(TokenKind.BitAndAssign);
                }
                return Token(TokenKind.BitAnd);
            case '^':
                if (Match('^'))
                {
                    return Token(TokenKind.Xor);
                }
                if (Match('='))
                {
                    return Token(TokenKind.BitXorAssign);
                }
                return Token(TokenKind.BitXor);
            case '#':
                while (IsAlpha(Peek()) || IsHexDigit(Peek()))
                {
                    Advance();
                }
                var kind = MatchDirectiveOrHexLiteral();

                if (
                    kind is TokenKind.Define or TokenKind.Region or TokenKind.EndRegion
                    && Peek() != '\r'
                    && Peek() != '\n'
                )
                {
                    Mode = LexerMode.RegionName;
                }

                return Token(kind);
            case '@':
                if (Peek() != '"' && Peek() != '\'')
                {
                    return UnexpectedToken();
                }

                var quote = Peek();
                Advance();

                while (!HitEof)
                {
                    if (Peek() == quote)
                    {
                        if (Peek(2) == quote)
                        {
                            Advance();
                        }
                        else
                        {
                            break;
                        }
                    }
                    Advance();
                }

                if (Match(quote))
                {
                    return Token(TokenKind.VerbatimStringLiteral);
                }

                return UnexpectedToken("Unterminated string literal");
            case '$':
                if (Match('"'))
                {
                    while (IsTemplateStringCharacter(Peek()) && !HitEof)
                    {
                        Advance();
                    }
                    if (Match('{'))
                    {
                        return Token(TokenKind.TemplateStart);
                    }
                    if (Match('"'))
                    {
                        return Token(TokenKind.SimpleTemplateString);
                    }

                    return UnexpectedToken();
                }
                else if (IsHexDigit(Peek()))
                {
                    Advance();
                    while (IsHexDigit(Peek()))
                    {
                        Advance();
                    }
                    return Token(TokenKind.HexIntegerLiteral);
                }

                return UnexpectedToken();
            case '"':
                MatchStringCharacters();
                if (!Match('"'))
                {
                    return UnexpectedToken("Unterminated string literal");
                }
                return Token(TokenKind.StringLiteral);
            default:
                if (IsDigit(character))
                {
                    // binary literal
                    if (character == '0' && Peek(1) == 'b' && IsBinaryDigit(Peek(2)))
                    {
                        Advance();
                        while (Peek() == '0' || Peek() == '1' || Peek() == '_')
                        {
                            Advance();
                        }
                        return Token(TokenKind.BinaryLiteral);
                    }

                    if (character == '0' && Peek(1) == 'x' && IsHexDigit(Peek(2)))
                    {
                        Advance();
                        while (IsHexDigit(Peek()))
                        {
                            Advance();
                        }
                        return Token(TokenKind.HexIntegerLiteral);
                    }

                    while (IsDigit(Peek()) || Peek() == '_')
                    {
                        Advance();
                    }

                    if (Peek() == '.')
                    {
                        Advance();
                        // Underscores are only allowed before the '.'
                        while (IsDigit(Peek()) && !HitEof)
                        {
                            Advance();
                        }
                        return Token(TokenKind.DecimalLiteral);
                    }
                    else
                    {
                        return Token(TokenKind.IntegerLiteral);
                    }
                }
                if (IsAlpha(character))
                {
                    while (IsAlpha(Peek()) || IsDigit(Peek()))
                    {
                        Advance();
                    }
                    return Token(MatchKeywordOrIdentifier());
                }
                break;
        }

        return UnexpectedToken();
    }

    private TokenKind MatchKeywordOrIdentifier()
    {
        return CurrentToken switch
        {
            "and" => TokenKind.And,
            "or" => TokenKind.Or,
            "xor" => TokenKind.Xor,
            "not" => TokenKind.Not,
            "mod" => TokenKind.Modulo,
            "div" => TokenKind.IntegerDivide,
            "begin" => TokenKind.OpenBrace,
            "end" => TokenKind.CloseBrace,
            "true" => TokenKind.BooleanLiteral,
            "false" => TokenKind.BooleanLiteral,
            "break" => TokenKind.Break,
            "exit" => TokenKind.Exit,
            "do" => TokenKind.Do,
            "until" => TokenKind.Until,
            "case" => TokenKind.Case,
            "else" => TokenKind.Else,
            "new" => TokenKind.New,
            "var" => TokenKind.Var,
            "globalvar" => TokenKind.GlobalVar,
            "try" => TokenKind.Try,
            "catch" => TokenKind.Catch,
            "finally" => TokenKind.Finally,
            "return" => TokenKind.Return,
            "continue" => TokenKind.Continue,
            "for" => TokenKind.For,
            "switch" => TokenKind.Switch,
            "while" => TokenKind.While,
            "repeat" => TokenKind.Repeat,
            "function" => TokenKind.Function,
            "with" => TokenKind.With,
            "default" => TokenKind.Default,
            "if" => TokenKind.If,
            "then" => TokenKind.Then,
            "throw" => TokenKind.Throw,
            "delete" => TokenKind.Delete,
            "enum" => TokenKind.Enum,
            "constructor" => TokenKind.Constructor,
            "static" => TokenKind.Static,
            "undefined" => TokenKind.Undefined,
            "noone" => TokenKind.Noone,
            _ => TokenKind.Identifier,
        };
    }

    private TokenKind MatchDirectiveOrHexLiteral()
    {
        var text = CurrentToken;
        var directive = text switch
        {
            "#macro" => TokenKind.Macro,
            "#region" => TokenKind.Region,
            "#endregion" => TokenKind.EndRegion,
            "#define" => TokenKind.Define,
            _ => TokenKind.UnknownDirective
        };

        if (directive is TokenKind.UnknownDirective)
        {
            foreach (var c in text.Skip(1))
            {
                if (!IsHexDigit(c))
                {
                    return TokenKind.Unexpected;
                }
            }
            return TokenKind.HexIntegerLiteral;
        }

        return directive;
    }

    private void MatchStringCharacters()
    {
        while (!HitEof)
        {
            if (Peek() == '\\')
            {
                Advance();
                Advance();
                continue;
            }

            if (Peek() == '"')
            {
                break;
            }

            Advance();
        }
    }

    private static bool IsDigit(int c)
    {
        return c >= '0' && c <= '9';
    }

    private static bool IsAlpha(int c)
    {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_';
    }

    private static bool IsHexDigit(int c)
    {
        return IsDigit(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f') || c == '_';
    }

    private static bool IsBinaryDigit(int c)
    {
        return c == '0' || c == '1';
    }

    private static bool IsTemplateStringCharacter(int c)
    {
        return c != '"' && c != '{' && c != '\r' && c != '\n';
    }

    private bool Match(int expected)
    {
        var next = Peek();

        if (next == expected)
        {
            Advance();
            return true;
        }

        return false;
    }

    private bool MatchAny(char[] expected)
    {
        var next = Peek();

        if (Array.Exists(expected, c => next == c))
        {
            Advance();
            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Peek(int amount = 1)
    {
        var targetIndex = index + amount - 1;
        if (targetIndex >= text.Length)
        {
            return -1;
        }
        return text[targetIndex];
    }

    private void Advance()
    {
        if (index >= text.Length)
        {
            HitEof = true;
            character = -1;
            return;
        }

        character = text[index];
        index++;

        switch (character)
        {
            case '\n':
                lineNumber += 1;
                columnNumber = 0;
                break;
            case '\t':
                columnNumber += tabWidth;
                break;
            default:
                columnNumber += 1;
                break;
        }
    }

    private Token Token(TokenKind kind, string? error = null)
    {
        var token = new Token
        {
            Line = lineNumber,
            Column = columnNumber,
            Kind = kind,
            Text = CurrentToken,
            StartIndex = startIndex,
            EndIndex = index,
            ErrorMessage = error
        };

        return token;
    }

    private Token UnexpectedToken(string? error = null)
    {
        return Token(TokenKind.Unexpected, error);
    }
}
