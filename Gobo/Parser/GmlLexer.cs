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
    private TextReader reader;
    private int lineNumber;
    private int columnNumber;
    private int startIndex;
    private int index;
    private readonly int tabWidth;
    private int character;
    private readonly List<char> currentToken = new();
    private readonly char[] charBuffer = new char[4];

    public GmlLexer(TextReader reader, int tabWidth = 4)
    {
        this.reader = reader;
        this.tabWidth = tabWidth;
        index = 0;
    }

    public Token NextToken()
    {
        startIndex = index;
        Advance();

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
                    if (!MatchAny('\u000B', '\u000C', '\u0020', '\u00A0', '\t'))
                    {
                        break;
                    }
                }
                return Token(TokenKind.WhiteSpace);
            case '\n':
                return Token(TokenKind.LineBreak);
            case '\r':
                if (Match('\n'))
                {
                    return Token(TokenKind.LineBreak);
                }
                break;
            case '/':
                if (Match('/'))
                {
                    MatchUntil('\n');
                    return Token(TokenKind.SingleLineComment);
                }
                else if (Match('*'))
                {
                    while (true)
                    {
                        MatchUntil('*');
                        if (!Match('*'))
                        {
                            return Token(TokenKind.UnexpectedCharacter);
                        }
                        if (Match('/'))
                        {
                            return Token(TokenKind.MultiLineComment);
                        }
                    }
                }
                return Token(TokenKind.Divide);
            default:
                break;
        }

        return Token(TokenKind.UnexpectedCharacter);
    }

    private bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private bool IsHexDigit(char c)
    {
        return IsDigit(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
    }

    private bool Match(char expected)
    {
        var next = Peek();

        if (next == expected)
        {
            Advance();
            return true;
        }

        return false;
    }

    private bool MatchAny(params char[] expected)
    {
        var next = Peek();

        if (Array.Exists(expected, c => next == c))
        {
            Advance();
            return true;
        }

        return false;
    }

    private void MatchUntil(char terminator)
    {
        while (Peek() != terminator && !HitEof)
        {
            Advance();
        }
    }

    private int Peek(int amount = 1)
    {
        return reader.Peek();
    }

    private void Advance()
    {
        character = reader.Read();

        if (character == -1)
        {
            HitEof = true;
            return;
        }

        currentToken.Add((char)character);
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

    private Token Token(TokenKind kind)
    {
        var token = new Token
        {
            Line = lineNumber,
            Column = columnNumber,
            Kind = kind,
            Text = string.Join("", currentToken),
            StartIndex = startIndex,
            EndIndex = index,
        };

        currentToken.Clear();

        return token;
    }
}
