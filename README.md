## Gobo: GML Code Formatter

Gobo is an opinionated formatter for GameMaker Language. It enforces a consistent style by parsing and re-printing your code with its own rules, taking maximum line length into account.

Gobo currently provides a few basic options that affect formatting and has no plans to add more. It follows the [Option Philosophy](https://prettier.io/docs/en/option-philosophy.html) of prettier.

Gobo is currently only available as a native CLI tool, but it will eventually be available as an IDE plugin.

### Input

```js
x = a and b or c  a=0xFG=1 var var var i := 0
do begin
;;;;show_debug_message(i)
;;;;++constructor
end until not constructor < 10 return

call()
```

### Output

```js
x = a && b || c;
a = 0xF;
G = 1;
var i = 0;
do {
    show_debug_message(i);
    ++constructor;
} until (!constructor < 10)
return call();
```

## How does it work?
Gobo is written in C# and compiles to a self-contained binary using Native AOT in .NET 8. This may be changed in the future!

Gobo uses [Antlr4](https://www.antlr.org/)-generated code to parse GML and convert it into an abstract syntax tree. There is no officially-documented format for GML's syntax tree, so Gobo uses a format similar to TypeScript's AST. The grammar spec has been designed to only handle valid GML code, barring a few exceptions, to ensure correctness.

Gobo uses a modified implementation of Prettier's printing algorithm to make decisions about wrapping lines and printing comments. The implementation of the "Doc" printing algorithm is taken from [CSharpier](https://github.com/belav/csharpier).


