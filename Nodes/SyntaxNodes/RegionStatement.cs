using Antlr4.Runtime;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class RegionStatement : GmlSyntaxNode
    {
        public string Name { get; set; }
        public bool IsEndRegion { get; set; }

        public RegionStatement(
            ParserRuleContext context,
            CommonTokenStream tokenStream,
            string name,
            bool isEndRegion
        )
            : base(context, tokenStream)
        {
            Name = name;
            IsEndRegion = isEndRegion;
        }

        public override Doc Print()
        {
            return IsEndRegion ? "#endregion" : Doc.Concat("#region", Name);
        }
    }
}
