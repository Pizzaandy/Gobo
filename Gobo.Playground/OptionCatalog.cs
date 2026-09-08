namespace Gobo.Playground;

public record Example(string Label, string Code);

public record Tip(string Title, string Description, IReadOnlyList<Example> Examples)
{
    public Tip(string title, string description, string before, string after)
        : this(
            title,
            description,
            new[] { new Example("Before", before), new Example("After", after) }
        ) { }
}

public record FormatOption(
    string Group,
    string Id,
    string Label,
    Tip Tip,
    Func<bool> Get,
    Action<bool> Set
);

/// <summary>
/// The options offered by the playground, with an example of what each one does.
/// Kept out of the page because Razor reads '@tag' and '&lt;desc&gt;' in these strings as markup.
/// </summary>
public static class OptionCatalog
{
    public static Tip BraceStyle { get; } =
        new(
            "Brace Style",
            "Where the opening brace of a block is printed.",
            new[]
            {
                new Example("Same Line", "function foo() {\n\tone();\n}"),
                new Example("New Line", "function foo()\n{\n\tone();\n}"),
                new Example("New Line Indented", "function foo()\n\t{\n\tone();\n\t}"),
            }
        );

    public static IEnumerable<FormatOption> For(FormatOptions options)
    {
        yield return new FormatOption(
            "Layout",
            "elseOnNewLine",
            "Else on new line",
            new Tip(
                "Else on new line",
                "Puts else, catch and finally on their own line instead of cuddling them against the closing brace.",
                "if (_a) {\n\tone();\n} else {\n\ttwo();\n}",
                "if (_a) {\n\tone();\n}\nelse {\n\ttwo();\n}"
            ),
            () => options.ElseOnNewLine,
            value => options.ElseOnNewLine = value
        );

        yield return new FormatOption(
            "Layout",
            "stackedConditions",
            "Stacked conditions",
            new Tip(
                "Stacked conditions",
                "A condition split across lines keeps one operand per line, each in its own parentheses, with the operator leading. Refused when an operand is itself a logical chain, which would regroup the condition.",
                "if (\n\t_grounded\n\t&& _can_jump\n) {",
                "if (_grounded)\n&& (_can_jump) {"
            ),
            () => options.StackedConditions,
            value => options.StackedConditions = value
        );

        yield return new FormatOption(
            "Layout",
            "braceSwitchCases",
            "Brace switch cases",
            new Tip(
                "Brace switch cases",
                "A case holding more than one statement gets braces so the IDE can collapse it. A trailing break does not count towards that.",
                "case PHASE.start:\n\tsetup();\n\tarm();\n\tbreak;",
                "case PHASE.start: {\n\tsetup();\n\tarm();\n\tbreak;\n}"
            ),
            () => options.BraceSwitchCases,
            value => options.BraceSwitchCases = value
        );

        yield return new FormatOption(
            "Layout",
            "inlineShortBlocks",
            "Inline short blocks",
            new Tip(
                "Inline short blocks",
                "A body written on the keyword's line stays there, with braces added. A body already written across lines is left alone, however short it is.",
                "if (_dead) exit;",
                "if (_dead) { exit; }"
            ),
            () => options.InlineShortBlocks,
            value => options.InlineShortBlocks = value
        );

        yield return new FormatOption(
            "Layout",
            "preserveGluedStatements",
            "Preserve glued statements",
            new Tip(
                "Preserve glued statements",
                "Leaves the counter idiom exactly as written. Nothing is converted into this shape; without the option the setup statement moves onto its own line and the trailing one is pulled into the body.",
                "var _i = 0; repeat (_n) {\n\twork(_i);\n_i += 1;}",
                "var _i = 0; repeat (_n) {\n\twork(_i);\n_i += 1;}"
            ),
            () => options.PreserveGluedStatements,
            value => options.PreserveGluedStatements = value
        );

        yield return new FormatOption(
            "Layout",
            "flatExpressions",
            "Flat expressions",
            new Tip(
                "Flat expressions",
                "Never breaks an expression across lines, however far past the print width it runs.",
                "_total = _first\n\t+ _second\n\t+ _third;",
                "_total = _first + _second + _third;"
            ),
            () => options.FlatExpressions,
            value => options.FlatExpressions = value
        );

        yield return new FormatOption(
            "Naming",
            "prefixLocalVariables",
            "Prefix local variables",
            new Tip(
                "Prefix local variables",
                "Renames locals and parameters missing the leading underscore. Skips a name a nested function redeclares, one that doubles as a shorthand struct key, and one whose prefixed form is already taken.",
                "var count = 0;\ncount += 1;",
                "var _count = 0;\n_count += 1;"
            ),
            () => options.PrefixLocalVariables,
            value => options.PrefixLocalVariables = value
        );

        yield return new FormatOption(
            "Naming",
            "prefixStaticVariables",
            "Prefix static variables",
            new Tip(
                "Prefix static variables",
                "Renames static variables missing the leading double underscore. Static functions are left alone: they are methods on the constructor's struct, callable by name from other files.",
                "static count = 0;\nstatic step = function() {};",
                "static __count = 0;\nstatic step = function() {};"
            ),
            () => options.PrefixStaticVariables,
            value => options.PrefixStaticVariables = value
        );

        yield return new FormatOption(
            "Doc comments",
            "wrapJsDocInRegion",
            "Wrap jsDoc in region",
            new Tip(
                "Wrap jsDoc in region",
                "Wraps the doc comment above a function in a region. A jsDoc comment cannot be collapsed in the IDE on its own.",
                "/// @function foo\nfunction foo() {}",
                "#region jsDoc\n/// @function foo\n#endregion\nfunction foo() {}"
            ),
            () => options.WrapJsDocInRegion,
            value => options.WrapJsDocInRegion = value
        );

        yield return new FormatOption(
            "Doc comments",
            "addSelfToMethods",
            "Add @self to methods",
            new Tip(
                "Add @self to methods",
                "Gives a function declared inside a constructor an @self naming that constructor, which is what Feather reads for context.",
                "/// @param {Real} _dt\nstep = function(_dt) {};",
                "/// @param {Real} _dt\n/// @self Pilot\nstep = function(_dt) {};"
            ),
            () => options.AddSelfToMethods,
            value => options.AddSelfToMethods = value
        );

        yield return new FormatOption(
            "Doc comments",
            "generateMissingJsDoc",
            "Generate missing jsDoc",
            new Tip(
                "Generate missing jsDoc",
                "Writes a skeleton above a function that has none. Optional parameters are bracketed, @returns appears only when the body returns a value, and unnamed functions are skipped.",
                "function damage(_amount, _crit = false) {\n\treturn _amount;\n}",
                "/// @function damage\n/// @description <desc>\n/// @param {Any} _amount <desc>\n/// @param {Any} [_crit] <desc>\n/// @returns {Any}\nfunction damage(_amount, _crit = false) {\n\treturn _amount;\n}"
            ),
            () => options.GenerateMissingJsDoc,
            value => options.GenerateMissingJsDoc = value
        );

        yield return new FormatOption(
            "Rewrites",
            "removeArrayCopyAccessor",
            "Remove array copy accessor",
            new Tip(
                "Remove array copy accessor",
                "Rewrites the array copy accessor as a plain one. Safe only while the Copy on Write game option is disabled; with it enabled the accessor is what makes a write reach the referenced array.",
                "_grid[@ _i] = 1;",
                "_grid[_i] = 1;"
            ),
            () => options.RemoveArrayCopyAccessor,
            value => options.RemoveArrayCopyAccessor = value
        );
    }
}
