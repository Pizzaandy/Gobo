using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PrettierGML.Printer.DocTypes;
using PrettierGML.SyntaxNodes.Gml;

namespace PrettierGML.SyntaxNodes
{
    internal class EmptyNode : GmlSyntaxNode
    {
        internal enum EmptyType
        {
            Discard,
            UndefinedArgument
        }

        public static EmptyNode Instance { get; } = new();
        public static EmptyNode UndefinedArgument { get; } = new(EmptyType.UndefinedArgument);

        [JsonConverter(typeof(StringEnumConverter))]
        public EmptyType Type { get; init; }

        public EmptyNode(EmptyType type = EmptyType.Discard)
        {
            Type = type;
        }

        public override Doc PrintNode(PrintContext ctx)
        {
            // Empty nodes in argument lists are treated as undefined
            if (Type is EmptyType.UndefinedArgument)
            {
                return Literal.Undefined;
            }
            return Doc.Null;
        }

        public override int GetHashCode()
        {
            if (Type is EmptyType.UndefinedArgument)
            {
                return Literal.Undefined.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }
    }
}
