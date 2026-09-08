window.editorRender = function(element, mode, theme, readOnly, tabSize) {
    var editor = ace.edit(element);
    var defineMode = "ace/mode/" + mode;
    editor.setTheme("ace/theme/" + theme);

    editor.setReadOnly(readOnly);
    editor.session.setMode(defineMode);
    editor.session.setTabSize(tabSize);
    editor.renderer.setScrollMargin(10, 10);
    editor.commands.removeCommands(["openCommandPalette", "showSettingsMenu"]);

    editor.setOptions({
        autoScrollEditorIntoView: true
    });
};

window.ace_destroy = function(element) {
    var editor = ace.edit(element);
    editor.destroy();
    editor.container.remove();
}

window.ace_set_readonly = function(element, readOnly) {
    var editor = ace.edit(element);
    editor.setReadOnly(readOnly);
}


window.GetCode = function(dotNetHelper, element) {
    var editor = ace.edit(element);
    var code = editor.getSession().getValue();
    dotNetHelper.invokeMethodAsync('ReceiveCode', code)
};

window.SetCode = function (dotNetHelper, element, code) {
    var editor = ace.edit(element);
    editor.getSession().setValue(code);
    editor.renderer.updateFull();
    dotNetHelper.invokeMethodAsync('ReceiveCode', code);
}

window.SetWidth = function (element, width) {
    var editor = ace.edit(element);
    editor.setOption("printMarginColumn", width);
}

// Renders a snippet with the same tokenizer and theme as the editors, so the examples in
// the option tooltips are coloured identically.
window.highlightGml = function (code) {
    var mode = ace.require("ace/mode/game_maker_language");
    var tokenizer = new mode.Mode().getTokenizer();
    var lines = code.split("\n");
    var state = "start";
    var html = "";

    var escape = function (text) {
        return text
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;");
    };

    for (var i = 0; i < lines.length; i++) {
        var data = tokenizer.getLineTokens(lines[i], state);
        state = data.state;

        for (var t = 0; t < data.tokens.length; t++) {
            var token = data.tokens[t];
            var classes = "ace_" + token.type.replace(/\./g, " ace_");
            html += '<span class="' + classes + '">' + escape(token.value) + "</span>";
        }

        if (i < lines.length - 1) {
            html += "\n";
        }
    }

    return html;
};
