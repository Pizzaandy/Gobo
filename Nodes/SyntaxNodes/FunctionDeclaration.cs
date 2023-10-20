using Antlr4.Runtime;
using System.Diagnostics;

namespace PrettierGML.Nodes
{
    internal class FunctionDeclaration : GmlSyntaxNode
    {
        public GmlSyntaxNode Id { get; set; }
        public GmlSyntaxNode Parameters { get; set; }
        public GmlSyntaxNode Body { get; set; }
        public GmlSyntaxNode ConstructorParent { get; set; }

        public FunctionDeclaration(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            GmlSyntaxNode id,
            GmlSyntaxNode parameters,
            GmlSyntaxNode body,
            GmlSyntaxNode parent
        )
            : base(context, tokenStream)
        {
            Id = AsChild(id);
            Parameters = AsChild(parameters);
            Debug.Assert(Parameters is NodeList);
            Body = AsChild(body);
            ConstructorParent = AsChild(parent);
        }

        public override Doc Print()
        {
            var parts = new List<Doc>
            {
                Doc.Concat("function", Id.IsEmpty ? "" : " ", Id.Print()),
                PrintHelper.PrintArgumentListLikeSyntax("(", Parameters, ")", ",")
            };

            if (!ConstructorParent.IsEmpty)
            {
                parts.Add(ConstructorParent.Print());
            }

            parts.Add(" ");
            parts.Add(PrintHelper.EnsureStatementInBlock(Body));

            return Doc.Concat(parts);
        }
    }
}
