using Antlr4.Runtime;

namespace PrettierGML.Parser.Antlr
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
            var parser = (ParserInterpreter)Recognizer;
            var stack = parser.GetRuleInvocationStack();
            var stackText = parser.GetRuleInvocationStackAsString();
            var lastRule = stack[0];

            var symbolText =
                OffendingSymbol.Text == "<EOF>" ? "end of file" : $"'{OffendingSymbol.Text}'";

            string offendingSymbolMessage = lastRule switch
            {
                "closeBlock" => $"expected '}}'",
                "macroStatement" => $"macro is invalid",
                _ => $"unexpected {symbolText}"
            };

            var syntaxErrorMessage = $"Syntax error at line {Line}, column {CharPositionInLine}:\n";

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
