## Gobo: GML Code Formatter

[Try out the formatter here!](https://pizzaandy.github.io/Gobo/)

Gobo is an opinionated formatter for GameMaker Language. It enforces a consistent style by parsing and re-printing your code with its own rules, taking maximum line length into account.

Gobo currently provides a few basic options that affect formatting and has no plans to add more. It follows the [Option Philosophy](https://prettier.io/docs/en/option-philosophy.html) of Prettier.

Gobo is currently only available as a native CLI tool, but it will eventually be available as an IDE plugin (hopefully!).

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
Gobo is written in C# and compiles to a self-contained binary using Native AOT in .NET 8.

Gobo uses a custom GML parser to read your code and ensure that formatted code is equivalent to the original. The parser is context-free and accepts a superset of valid GML. The goal is to eventually add error recovery and a "strict" mode that only accepts compilable GML. There is no officially-documented format for GML's syntax tree, so Gobo uses a format similar to JavaScript parsers. 

Gobo converts your code into an intermediate "Doc" format to make decisions about wrapping lines and printing comments. The doc printing algorith is taken from [CSharpier](https://github.com/belav/csharpier), which is itself adapted from Prettier.


