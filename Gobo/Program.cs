using Gobo.Parser;

var input = $$$"""
x = $"{{ a : $"{{ wtf : ":3" }}" }}"
""";

var parser = new GmlParser(input);

var ast = parser.Parse().Ast;
Console.WriteLine(ast);
