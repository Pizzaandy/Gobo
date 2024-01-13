## Gobo: GML Formatter

[Try the formatter here!](https://pizzaandy.github.io/Gobo/)

Gobo is an opinionated formatter for GameMaker Language. It enforces a consistent style by parsing and re-printing your code with its own rules, taking maximum line length into account.

By using Gobo, you agree to cede control over the nitty-gritty details of formatting. In return, Gobo gives you speed, determinism, and smaller git diffs. End style debates with your team and save mental energy for what's important!

Gobo provides a few basic formatting options and has no plans to add more. It follows the [Option Philosophy](https://prettier.io/docs/en/option-philosophy.html) of Prettier.

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

Gobo uses a custom GML parser to read your code and ensure that formatted code is equivalent to the original. The parser is designed to only accept valid GML code (with a few exceptions) to ensure correctness. The goal is to eventually add error recovery and a "strict" mode that only accepts compilable GML. There is no officially-documented format for GML's syntax tree, so Gobo uses a format similar to JavaScript parsers. 

Gobo converts your code into an intermediate "Doc" format to make decisions about wrapping lines and printing comments. The doc printing algorithm is taken from [CSharpier](https://github.com/belav/csharpier), which is itself adapted from Prettier.

## Limitations
Gobo cannot format code that relies on macro expansion to be valid. You must escape any invalid GML with `//fmt-ignore`
```js
// fmt-ignore
{
    ABUSE_MACROS 
    IN_THIS
    BLOCK
}

OTHERWISE_I_WILL;
ADD_SEMICOLONS;
```
