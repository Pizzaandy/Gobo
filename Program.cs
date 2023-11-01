using PrettierGML;
using System.Diagnostics;

static void FormatFile(string filePath)
{
    string input = File.ReadAllText(filePath);
    var formatted = Format(input);
    File.WriteAllText(filePath, formatted);
}

static string Format(string input)
{
    var formatOptions = new FormatOptions() { BraceStyle = BraceStyle.SameLine, CheckAst = true };

    Stopwatch sw = Stopwatch.StartNew();
    var result = GmlFormatter.Format(input, formatOptions);
    sw.Stop();

    Console.WriteLine(result);
    Console.WriteLine($"Total Time: {sw.ElapsedMilliseconds} ms");

    return result;
}

// example usage
var input = """
        switch (player_lives)
    {
        case 3:
            draw_sprite(20, 20, spr_face_healthy);
        break;

        case 2:
            draw_sprite(20, 20, spr_face_hurt);
        break;

        case stacked:
        case "bar":
        case 1:
            draw_sprite(20, 20, spr_face_fatal);
        break;
        // comment
        default:
            draw_sprite(20, 20, spr_face_fainted);
        break;
    }

    enum foo {
     bar, baz 
    }
    """;

Format(input);
