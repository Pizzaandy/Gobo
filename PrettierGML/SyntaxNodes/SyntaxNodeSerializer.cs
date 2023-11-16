using System.Text.Json.Serialization;

namespace PrettierGML.SyntaxNodes
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(GmlSyntaxNode))]
    internal partial class SyntaxNodeSerializerContext : JsonSerializerContext { }
}
