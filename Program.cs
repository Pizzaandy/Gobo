using CSharpier;
using PrettierGML;
using System.Diagnostics;

static void Format(string input)
{
    Stopwatch sw = Stopwatch.StartNew();

    var options = new PrinterOptions() { Width = 80 };

    var astString = GmlParser.Parse(input).ToString();
    Console.WriteLine(astString);

    var result = GmlFormatter.Format(input, options, checkAst: false);
    Console.WriteLine(result);

    sw.Stop();
    Console.WriteLine($"Total Time: {sw.ElapsedMilliseconds} ms");
}

Format(
    """
repeat(abs(vel)) {
    var _xv = lengthdir_x(1,rotation);
    var _yv = lengthdir_y(1,rotation);
    if (!place_meeting(x+_xv,y____________________________________________________________+_yv___________,par_platform__________________________________________)) {
        x += _xv;
        y += _yv;
    } else {
        x_vel_current = 0;
        y_vel_current = 0;
        if (instance_exists(obj_player)) {
            with (obj_player) {
                set_state(player_state_grapple);
            }
        }
    }
}

"""
);
