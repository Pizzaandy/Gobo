using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.Gml;
using System.Net.Http.Headers;

namespace PrettierGML.SyntaxNodes
{
    internal class EmptyNode : GmlSyntaxNode
    {
        public static EmptyNode Instance { get; } = new();

        public override Doc PrintNode(PrintContext ctx)
        {
            return Doc.Null;
        }
    }
}
