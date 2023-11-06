using Antlr4.Runtime;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.PrintHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettierGML.SyntaxNodes.Gml
{
    internal sealed class TemplateExpression : GmlSyntaxNode
    {
        public GmlSyntaxNode Expression { get; set; }

        public TemplateExpression(ParserRuleContext context, GmlSyntaxNode expression)
            : base(context)
        {
            Expression = AsChild(expression);
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            return Doc.Group("{", Doc.SoftLine, Expression.Print(ctx), Doc.LiteralLine, "}");
        }
    }
}
