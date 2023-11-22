using System.Text.Json.Serialization;
using Gobo.SyntaxNodes.Gml;
using Gobo.SyntaxNodes.Gml.Literals;
using Gobo.SyntaxNodes.GmlExtensions;

namespace Gobo.SyntaxNodes;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    GenerationMode = JsonSourceGenerationMode.Serialization,
    MaxDepth = 256
)]
[JsonSerializable(typeof(GmlSyntaxNode))]
internal partial class SyntaxNodeSerializerContext : JsonSerializerContext { }

// Currently, the only way to use source-generated System.Text.Json serializers
// with polymorphism is to explicitly declare each subtype. This is fine...
[JsonDerivedType(typeof(ArgumentList))]
[JsonDerivedType(typeof(ArrayExpression))]
[JsonDerivedType(typeof(AssignmentExpression))]
[JsonDerivedType(typeof(BinaryExpression))]
[JsonDerivedType(typeof(Block))]
[JsonDerivedType(typeof(BreakStatement))]
[JsonDerivedType(typeof(CallExpression))]
[JsonDerivedType(typeof(CatchProduction))]
[JsonDerivedType(typeof(ConditionalExpression))]
[JsonDerivedType(typeof(ConstructorClause))]
[JsonDerivedType(typeof(ContinueStatement))]
[JsonDerivedType(typeof(DefineStatement))]
[JsonDerivedType(typeof(DeleteStatement))]
[JsonDerivedType(typeof(Document))]
[JsonDerivedType(typeof(DoStatement))]
[JsonDerivedType(typeof(EnumBlock))]
[JsonDerivedType(typeof(EnumDeclaration))]
[JsonDerivedType(typeof(EnumMember))]
[JsonDerivedType(typeof(ExitStatement))]
[JsonDerivedType(typeof(FinallyProduction))]
[JsonDerivedType(typeof(ForStatement))]
[JsonDerivedType(typeof(FunctionDeclaration))]
[JsonDerivedType(typeof(GlobalVariableStatement))]
[JsonDerivedType(typeof(Identifier))]
[JsonDerivedType(typeof(IfStatement))]
[JsonDerivedType(typeof(IncDecStatement))]
[JsonDerivedType(typeof(Literal))]
[JsonDerivedType(typeof(MacroDeclaration))]
[JsonDerivedType(typeof(MemberDotExpression))]
[JsonDerivedType(typeof(MemberIndexExpression))]
[JsonDerivedType(typeof(NewExpression))]
[JsonDerivedType(typeof(Parameter))]
[JsonDerivedType(typeof(ParameterList))]
[JsonDerivedType(typeof(ParenthesizedExpression))]
[JsonDerivedType(typeof(RegionStatement))]
[JsonDerivedType(typeof(RepeatStatement))]
[JsonDerivedType(typeof(ReturnStatement))]
[JsonDerivedType(typeof(StructExpression))]
[JsonDerivedType(typeof(StructProperty))]
[JsonDerivedType(typeof(SwitchBlock))]
[JsonDerivedType(typeof(SwitchCase))]
[JsonDerivedType(typeof(SwitchStatement))]
[JsonDerivedType(typeof(TemplateExpression))]
[JsonDerivedType(typeof(TemplateLiteral))]
[JsonDerivedType(typeof(TemplateText))]
[JsonDerivedType(typeof(ThrowStatement))]
[JsonDerivedType(typeof(TryStatement))]
[JsonDerivedType(typeof(UnaryExpression))]
[JsonDerivedType(typeof(UndefinedArgument))]
[JsonDerivedType(typeof(VariableDeclarationList))]
[JsonDerivedType(typeof(VariableDeclarator))]
[JsonDerivedType(typeof(WhileStatement))]
[JsonDerivedType(typeof(WithStatement))]
[JsonDerivedType(typeof(EmptyNode))]
[JsonDerivedType(typeof(IntegerLiteral))]
[JsonDerivedType(typeof(DecimalLiteral))]
[JsonDerivedType(typeof(UndefinedLiteral))]
// Syntax extensions
[JsonDerivedType(typeof(TypeAnnotation))]
internal abstract partial class GmlSyntaxNode { }
