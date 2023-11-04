using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal class RegionStatement : GmlSyntaxNode
    {
        public string? Name { get; set; }
        public bool IsEndRegion { get; set; }

        public RegionStatement(ParserRuleContext context, string? name, bool isEndRegion)
            : base(context)
        {
            Name = name;
            IsEndRegion = isEndRegion;
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return IsEndRegion ? "#endregion" : Doc.Concat("#region", Name);
        }
    }
}
