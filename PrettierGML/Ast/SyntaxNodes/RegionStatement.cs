using Antlr4.Runtime;
using PrettierGML.Printer;
using PrettierGML.Printer.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
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

        public override Doc Print(PrintContext ctx)
        {
            return IsEndRegion ? "#endregion" : Doc.Concat("#region", Name);
        }
    }
}
