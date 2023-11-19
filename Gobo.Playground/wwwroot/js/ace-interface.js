window.editorRender = function(element, mode, theme, readOnly) {
    var editor = ace.edit(element);
    var defineMode = "ace/mode/" + mode;
    editor.setTheme("ace/theme/" + theme);

    editor.setReadOnly(readOnly);
    editor.session.setMode(defineMode);
    editor.renderer.setScrollMargin(10, 10);
    editor.setOptions({
        autoScrollEditorIntoView: true
    });
    // enable autocompletion and snippets
    editor.setOptions({
        enableBasicAutocompletion: true,
        enableSnippets: true,
        enableLiveAutocompletion: true
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