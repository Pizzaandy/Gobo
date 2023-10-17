using Antlr4.Runtime;

namespace PrettierGML
{
    public class GmlSyntaxErrorException : Exception
    {
        public GmlSyntaxErrorException(string message)
            : base(message) { }
    }

    public class GmlSyntaxError
    {
        public readonly IRecognizer Recognizer;
        public readonly IToken OffendingSymbol;
        public readonly int Line;
        public readonly int CharPositionInLine;
        public string Message;
        public readonly RecognitionException Exception;

        public GmlSyntaxError(
            IRecognizer recognizer,
            IToken offendingSymbol,
            int line,
            int charPositionInLine,
            string message,
            RecognitionException exception
        )
        {
            Recognizer = recognizer;
            OffendingSymbol = offendingSymbol;
            Line = line;
            CharPositionInLine = charPositionInLine;
            Message = message;
            Exception = exception;
        }

        public void Throw()
        {
            var parser = (Parser)Recognizer;
            var stack = parser.GetRuleInvocationStack();
            var stackText = parser.GetRuleInvocationStackAsString();
            var lastRule = stack[0];

            string offendingSymbolMessage;

            if (lastRule == "closeBlock")
            {
                offendingSymbolMessage = $"missing '}}' at {stack[2]}";
            }
            else
            {
                offendingSymbolMessage = $"unexpected '{OffendingSymbol.Text}'";
            }

            var syntaxErrorMessage =
                $"Syntax error at line {Line + 1}, column {CharPositionInLine}:\n";

            var message =
                syntaxErrorMessage
                + offendingSymbolMessage
                + "\n"
                + Message
                + $"\nStack: {stackText}";

            throw new GmlSyntaxErrorException(message);
        }
    }

    public class GameMakerLanguageErrorListener : BaseErrorListener
    {
        public List<GmlSyntaxError> Errors = new();

        public override void SyntaxError(
            TextWriter output,
            IRecognizer recognizer,
            IToken offendingSymbol,
            int line,
            int charPositionInLine,
            string msg,
            RecognitionException e
        )
        {
            var error = new GmlSyntaxError(
                recognizer,
                offendingSymbol,
                line,
                charPositionInLine,
                msg,
                e
            );
            error.Throw();
        }
    }
}
