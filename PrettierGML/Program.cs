using PrettierGML;

static string TestFormat(string input)
{
    var formatOptions = FormatOptions.DefaultTestOptions;

    formatOptions.ValidateOutput = true;
    formatOptions.BraceStyle = BraceStyle.SameLine;

    FormatResult result = GmlFormatter.Format(input, formatOptions);

    Console.WriteLine(result);

    FormatResult secondResult = GmlFormatter.Format(result.Output, formatOptions);

    Console.WriteLine(StringDiffer.PrintFirstDifference(result.Output, secondResult.Output));

    return result.Output;
}

var input = $$$"""
on_error(method({this: this, player_id: _player_id}, 
 function(_err /* foo*/ // bar
 ) {
    // reduce will fail if player doesn't have item
    LOGGER.info // reduce will fail if player doesn't have item
    ("Wiping bag for player:", {player_id: player_id});
    CLIENTS.items.delete_all_items_in_equipped_bag(player_id);
    // Quest flag death flag
    CLIENTS // e
    // e


    //e
        .quests
        .set_quest_flag // Quest flag death flag
        (player_id, "player_has_died_dc");
    this.parent_components.send_action_to_player(
        player_id,
        "notify_bag_wipe"
    );
}));
""";

TestFormat(input);
