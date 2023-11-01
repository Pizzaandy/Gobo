using Antlr4.Runtime;
using PrettierGML.Printer.Document.DocTypes;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class EnumBlock : GmlSyntaxNode
    {
        public List<GmlSyntaxNode> Members => Children;

        public EnumBlock(ParserRuleContext context, List<GmlSyntaxNode> declarations)
            : base(context)
        {
            AsChildren(declarations);
        }

        public override Doc Print(PrintContext ctx)
        {
            Doc members = Doc.Null;

            if (Children.Any())
            {
                var memberList = new List<Doc>();
                foreach (var member in Children)
                {
                    memberList.Add(Doc.Concat(member.Print(ctx), ","));
                }
                members = Doc.Join(Doc.HardLine, memberList);
            }

            return Block.PrintInBlock(ctx, members);
        }
    }
}
