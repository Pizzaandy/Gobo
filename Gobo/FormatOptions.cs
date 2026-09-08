using System.Text.Json.Serialization;

namespace Gobo;

public enum BraceStyle
{
    SameLine,
    NewLine,

    /// <summary>
    /// The brace sits on its own line, indented with the body it opens.
    /// </summary>
    NewLineIndented,
}

public class FormatOptions
{
    public bool UseTabs { get; set; } = true;
    public int TabWidth { get; set; } = 4;
    public int Width { get; set; } = 120;
    public bool FlatExpressions { get; set; } = false;

    /// <summary>
    /// Where the opening brace of a block is printed.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<BraceStyle>))]
    public BraceStyle BraceStyle { get; set; } = BraceStyle.SameLine;

    /// <summary>
    /// Print 'else', 'catch' and 'finally' on their own line.
    /// </summary>
    public bool ElseOnNewLine { get; set; } = true;

    /// <summary>
    /// Rewrite the array copy accessor 'a[@ i]' as a plain accessor 'a[i]'. Off by default:
    /// with the Copy on Write game option enabled the accessor is what makes a write reach
    /// the referenced array, and removing it changes what the code does.
    /// </summary>
    public bool RemoveArrayCopyAccessor { get; set; } = false;

    /// <summary>
    /// Rename local variables and parameters that are missing the leading underscore.
    /// </summary>
    public bool PrefixLocalVariables { get; set; } = true;

    /// <summary>
    /// Rename static variables that are missing the leading double underscore.
    /// Off by default, because a static is readable by name from other files.
    /// </summary>
    public bool PrefixStaticVariables { get; set; } = false;

    /// <summary>
    /// Keep a statement written on a loop's line, or on the closing brace's line, where it is.
    /// </summary>
    public bool PreserveGluedStatements { get; set; } = true;

    /// <summary>
    /// Print a multi line condition as one operand per line, each in its own parentheses,
    /// with the operator leading the line.
    /// </summary>
    public bool StackedConditions { get; set; } = true;

    /// <summary>
    /// Brace a switch case that holds more than one statement, not counting a trailing
    /// 'break'. Shorter cases print on the case label's line.
    /// </summary>
    public bool BraceSwitchCases { get; set; } = true;

    /// <summary>
    /// Keep a control flow body on the keyword's line when it holds a single statement and
    /// fits the print width, as in 'if (surface_exists(_s)) { surface_free(_s); }'.
    /// </summary>
    public bool InlineShortBlocks { get; set; } = true;

    /// <summary>
    /// Put a space after the slashes of a comment. Dividers made only of punctuation are
    /// left as they are.
    /// </summary>
    public bool SpaceAfterCommentMarker { get; set; } = true;

    /// <summary>
    /// Rewrite jsDoc tags to their canonical names and their types to Feather's.
    /// </summary>
    public bool NormalizeJsDocTags { get; set; } = true;

    /// <summary>
    /// Order jsDoc tags: category, function, description, param, returns, self, ignore.
    /// </summary>
    public bool SortJsDocTags { get; set; } = true;

    /// <summary>
    /// Delete comments that hold code rather than prose. It is decided by parsing the
    /// comment, and it deletes what it finds, so it is off unless asked for.
    /// </summary>
    public bool RemoveCommentedOutCode { get; set; } = false;

    /// <summary>
    /// Break a comment that runs past the print width onto continuation lines, measured
    /// against the indentation it sits at.
    /// </summary>
    public bool WrapComments { get; set; } = true;

    /// <summary>
    /// Line parameter descriptions up one tab past the longest name in the block.
    /// </summary>
    public bool AlignJsDocDescriptions { get; set; } = true;

    /// <summary>
    /// Wrap the doc comment above a function in '#region jsDoc', which the IDE can collapse.
    /// </summary>
    public bool WrapJsDocInRegion { get; set; } = true;

    /// <summary>
    /// Give a function declared inside a constructor an '@self' naming that constructor.
    /// </summary>
    public bool AddSelfToMethods { get; set; } = true;

    /// <summary>
    /// Write a doc comment skeleton above a function that has none.
    /// </summary>
    public bool GenerateMissingJsDoc { get; set; } = true;

    [JsonIgnore]
    public bool ValidateOutput { get; set; } = true;

    [JsonIgnore]
    public bool RemoveSyntaxExtensions { get; set; } = false;

    [JsonIgnore]
    public bool GetDebugInfo { get; set; } = false;

    /// <summary>
    /// Fixtures pad identifiers to exact lengths, so they pin their own width.
    /// </summary>
    public static FormatOptions DefaultTestOptions { get; } =
        new() { GetDebugInfo = true, Width = 90 };

    public static FormatOptions Default { get; } = new();
}

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(FormatOptions))]
public partial class FormatOptionsSerializer : JsonSerializerContext { }
