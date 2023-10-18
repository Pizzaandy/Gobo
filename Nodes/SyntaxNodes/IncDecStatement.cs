using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class IncDecStatement : GmlSyntaxNode
    {
        public string Operator { get; set; }
        public GmlSyntaxNode Argument { get; set; }
        public bool IsPrefix { get; set; }

        public IncDecStatement(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            string @operator,
            GmlSyntaxNode argument,
            bool isPrefix
        )
            : base(context, tokenStream)
        {
            Operator = @operator;
            Argument = AsChild(argument);
            IsPrefix = isPrefix;
        }

        public override Doc Print()
        {
            if (IsPrefix)
            {
                return Doc.Concat(Operator, Argument.Print());
            }
            else
            {
                return Doc.Concat(Argument.Print(), Operator);
            }
        }
    }
}
