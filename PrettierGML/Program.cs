using PrettierGML;

static string TestFormat(string input)
{
    var formatOptions = FormatOptions.DefaultTestOptions;
    formatOptions.ValidateOutput = true;
    formatOptions.Width = 80;

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    Console.WriteLine(result);

    //FormatResult secondResult = GmlFormatter.Format(result.Output, formatOptions);

    //Console.WriteLine(StringDiffer.PrintFirstDifference(result.Output, secondResult.Output));

    return result.Output;
}

var input = $$$"""
function monontone_chain(points, wrap=false) {
    var upper = [];
    var lower = [];
    var hull = [];
    var iterations = array_length(points);
    if (iterations < 3) {
        return hull;
    }
    for (var i = 0; i < iterations; i++) {
        var length = array_length(lower);
        while (length > 1 && cross_product(points[i], lower[length - 2], lower[length - 1]) <= 0) {
            array_delete(lower, length - 1, 1);
            length = array_length(lower);
        }
        array_push(lower, points[i]);
    }
    for (var i = iterations - 1; i > -1; i--) {
        var length = array_length(upper);
        while (length > 1 && cross_product(points[i], upper[length - 2], upper[length - 1]) <= 0) {
            array_delete(upper, length - 1, 1);
            length = array_length(upper);
        }
        array_push(upper, points[i]);
    }
    array_delete(lower, array_length(lower) - 1, 1);
    if (wrap == false) {
        array_delete(upper, array_length(upper) - 1, 1);
    }
    array_copy(hull, 0, lower, 0, array_length(lower));
    array_copy(hull, array_length(hull), upper, 0, array_length(upper));
    return hull;
}
""";

TestFormat(input);
