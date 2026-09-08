## Gobo: GML Formatter

Fork of [Gobo](https://github.com/Pizzaandy/Gobo) by PizzaAndy, with options for brace style, naming and doc comments.

[Try the formatter here!](https://tinkerer-red.github.io/Gobo/)

Gobo is an opinionated formatter for GameMaker Language. It enforces a consistent style by parsing and re-printing your code with its own rules, taking maximum line length into account.

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
var _i = 0;
do {
    show_debug_message(_i);
    ++constructor;
} until (!constructor < 10)
return call();
```

## Usage

```
gobo [options] <file-or-directory>
```

`--check` reports unformatted files and writes nothing. `--fast` skips output validation.
`--write-stdout` also writes to stdout, `--skip-write` writes nothing.

## Options

From `.goborc.json`, searched for from the target path upwards.

```json
{
    "useTabs": true,
    "tabWidth": 4,
    "width": 120,
    "braceStyle": "SameLine"
}
```

### Layout

| Option | Default | |
|---|---|---|
| `width` | `120` | Maximum line length. |
| `useTabs` | `true` | Indent with tabs. |
| `tabWidth` | `4` | Columns a tab advances. |
| `braceStyle` | `SameLine` | Also `NewLine`, or `NewLineIndented` for the brace indented with its body. |
| `elseOnNewLine` | `true` | `else`, `catch` and `finally` start their own line. |
| `stackedConditions` | `true` | A condition split across lines keeps one operand per line, parenthesised, operator leading. |
| `braceSwitchCases` | `true` | Braces once a case holds more than one statement. A trailing `break` does not count. |
| `inlineShortBlocks` | `true` | A body written on the keyword's line stays there, with braces added. |
| `preserveGluedStatements` | `true` | Leaves `var _i = 0; repeat (_n) {` and `_i += 1;}` as written. Never creates them. |
| `flatExpressions` | `false` | Never break an expression across lines. |

### Naming

| Option | Default | |
|---|---|---|
| `prefixLocalVariables` | `true` | `var x` becomes `var _x`. Skips shadowed names and taken ones. |
| `prefixStaticVariables` | `false` | `static x` becomes `static __x`. Static functions are skipped, since other files call them by name. |

### Comments and jsDoc

| Option | Default | |
|---|---|---|
| `spaceAfterCommentMarker` | `true` | A space after the slashes. Punctuation dividers are left alone. |
| `wrapComments` | `true` | Break comments running past `width`. |
| `normalizeJsDocTags` | `true` | `@desc` to `@description`, `{int}` to `{Real}`, and so on. |
| `sortJsDocTags` | `true` | Free text, `@category`, `@function`, `@description`, `@param`, `@returns`, `@self`, `@ignore`. |
| `alignJsDocDescriptions` | `true` | Parameter descriptions line up one tab past the longest name. |
| `wrapJsDocInRegion` | `true` | Wraps a doc comment in `#region jsDoc` so the IDE can collapse it. |
| `addSelfToMethods` | `true` | Adds `@self` naming the enclosing constructor. |
| `generateMissingJsDoc` | `true` | Writes a skeleton above an undocumented function. |
| `removeCommentedOutCode` | `false` | Deletes comments that parse as code. |

### Rewrites

| Option | Default | |
|---|---|---|
| `removeArrayCopyAccessor` | `false` | `a[@ i]` becomes `a[i]`. Only safe with the Copy on Write game option disabled, where the accessor does nothing. |

## How does it work?

Gobo is written in C# and compiles to a self-contained binary using Native AOT in .NET 9.

Gobo uses a custom GML parser to read your code and ensure that formatted code is equivalent to the original. The parser is designed to only accept valid GML (with a few exceptions) to ensure correctness. There is no officially-documented format for GML's syntax tree, so Gobo uses a format similar to JavaScript parsers.

Gobo converts your code into an intermediate "Doc" format to make decisions about wrapping lines and printing comments. The doc printing algorithm is taken from [CSharpier](https://github.com/belav/csharpier), which is itself adapted from Prettier.

The naming and comment options run as passes over the syntax tree, so formatting runs twice: once to settle braces and indentation, then again over the result.

## Limitations

Gobo cannot parse code that relies on macro expansion to be valid. Any standalone expression will be formatted with a semicolon, even if the expression is a macro.

```js
THESE_MACROS;
ARE.VALID;
BECAUSE_THEY_ARE_EXPRESSIONS()
```

Comments trailing a statement are left as written; only whole-line comments are rewritten.

Output is validated against Gobo's own parse, so it cannot catch a construct Gobo accepts and GameMaker rejects.
