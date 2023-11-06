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
    internal sealed class TemplateText : GmlSyntaxNode
    {
        public string Text { get; set; }

        public TemplateText(ParserRuleContext context, string text)
            : base(context)
        {
            Text = text;
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            // Template strings don't contain line breaks
            return Text;
        }
    }
}
