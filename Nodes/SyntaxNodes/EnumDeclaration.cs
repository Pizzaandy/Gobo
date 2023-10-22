using Antlr4.Runtime;
using System.Diagnostics;

namespace PrettierGML.Nodes.SyntaxNodes
{
    internal class EnumDeclaration : GmlSyntaxNode
    {
        public GmlSyntaxNode Name { get; set; }
        public GmlSyntaxNode Members { get; set; }

        public EnumDeclaration(ParserRuleContext context, GmlSyntaxNode name, GmlSyntaxNode members)
            : base(context)
        {
            Name = AsChild(name);
            Members = AsChild(members);
            Debug.Assert(Members is NodeList or EmptyNode);
        }

        public override Doc Print(PrintContext ctx)
        {
            Doc members = Doc.Null;

            if (Members.Children.Any())
            {
                var memberList = new List<Doc>();
                foreach (var member in Members.Children)
                {
                    memberList.Add(Doc.Concat(member.Print(ctx), ","));
                }
                members = Doc.Join(Doc.HardLine, memberList);
            }

            return Doc.Concat("enum", " ", Name.Print(ctx), " ", Block.PrintInBlock(ctx, members));
        }
    }
}
