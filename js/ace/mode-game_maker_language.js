ace.define("ace/mode/game_maker_language_highlight_rules",["require","exports","module","ace/lib/oop","ace/mode/text_highlight_rules"], function(require, exports, module){/* ***** BEGIN LICENSE BLOCK *****
 * Distributed under the BSD license:
 *
 * Copyright (c) 2012, Ajax.org B.V.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of Ajax.org B.V. nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL AJAX.ORG B.V. BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * ***** END LICENSE BLOCK ***** */
"use strict";
var oop = require("../lib/oop");
var TextHighlightRules = require("./text_highlight_rules").TextHighlightRules;
var GameMakerLanguageHighlightRules = function () {
    this.$rules = {
        start: [{
                include: "#jsdoc"
            }, {
                include: "#comments"
            }, {
                include: "#regions"
            }, {
                include: "#keywords"
            }, {
                include: "#constants"
            }, {
                include: "#operators"
            }, {
                include: "#functions"
            }, {
                include: "#function_calls"
            }, {
                include: "#numbers"
            }, {
                include: "#macros"
            }, {
                include: "#structs"
            }, {
                include: "#variables"
            }, {
                include: "#strings"
            }],
        "#comments": [{
                token: "comment.line.triple-slash.gml",
                regex: /\/\/\/.*$/
            }, {
                token: "comment.line.double-slash.gml",
                regex: /\/\/.*$/
            }, {
                token: "punctuation.definition.comment.begin.gml",
                regex: /\/\*/,
                push: [{
                        token: "punctuation.definition.comment.end.gml",
                        regex: /\*\//,
                        next: "pop"
                    }, {
                        defaultToken: "comment.block.gml"
                    }]
            }],
        "#regions": [{
                token: [
                    "keyword.region.begin.gml",
                    "comment.line.region.title.gml"
                ],
                regex: /(#region\b)((?:\s+.*)?)$/
            }, {
                token: "keyword.region.end.gml",
                regex: /#endregion\b/
            }],
        "#keywords": [{
                token: "storage.type.gml",
                regex: /\b(?:var|globalvar|enum)\b/
            }, {
                token: "storage.modifier.gml",
                regex: /\bstatic\b/
            }, {
                token: "storage.type.class",
                regex: /\bconstructor\b/
            }, {
                token: "keyword.operator.new.gml",
                regex: /\bnew\b/
            }, {
                token: "keyword.control.gml",
                regex: /\b(?:begin|end|if|then|else|while|do|for|break|continue|with|until|repeat|exit|return|switch|case|default|global|try|catch|finally|throw|delete)\b/
            }],
        "#operators": [{
                token: "keyword.operator.arithmetic.gml",
                regex: /[-+*\/%]|\b(?:mod|div)\b/
            }, {
                token: "keyword.operator.comparison.gml",
                regex: /<|>|<=|>=|==|!=/
            }, {
                token: "keyword.operator.logical.gml",
                regex: /&&|\|\||\^\^|!|\b(?:and|or|xor|not)\b/
            }, {
                token: "keyword.operator.assignment.gml",
                regex: /\+=|-=|\*=|\/=|%=|\|=|&=|\^=|\?\?=|<<=|>>=|>>>=|=/
            }, {
                token: "keyword.operator.bitwise.gml",
                regex: /&|\||\^|<<|>>|>>>|~/
            }, {
                token: "keyword.operator.ternary.gml",
                regex: /\?|:/
            }],
        "#constants": [{
                token: "constant.language.gml",
                regex: /\b(?:true|false|pi|NaN|infinity|self|other|noone|all|global|undefined|pointer_invalid|pointer_null)\b/
            }],
        "#functions": [{
                token: "storage.type.function.gml",
                regex: /\bfunction\b/
            }, {
                token: "entity.name.function.gml",
                regex: /\bfunction\s+\w+\s*\(/
            }],
        "#function_calls": [{
                token: [
                    "support.function.gml",
                    "meta.function-call.gml"
                ],
                regex: /\b(\w+)(\s*\()/
            }],
        "#strings": [{
                token: "punctuation.definition.string.begin.gml",
                regex: /[@$]?"/,
                push: [{
                        token: "punctuation.definition.string.end.gml",
                        regex: /"/,
                        next: "pop"
                    }, {
                        include: "#escape_sequences"
                    }, {
                        defaultToken: "string.quoted.double.gml"
                    }]
            }],
        "#numbers": [{
                token: "constant.numeric.integer.hexadecimal.gml",
                regex: /\b(?:0x[0-9a-fA-F]+|\$[0-9a-fA-F]+)\b/
            }, {
                token: "constant.numeric.integer.binary.gml",
                regex: /\b0b[01]+\b/
            }, {
                token: "constant.numeric.integer.decimal.gml",
                regex: /\b\d+\b/
            }],
        "#macros": [{
                token: [
                    "storage.type.gml",
                    "storage.type.macro.gml",
                    "variable.other.macro.gml",
                    "storage.type.macro.gml"
                ],
                regex: /(#macro)(\s+)(\w+)(\s*)/,
                push: [{
                        token: "storage.type.macro.gml",
                        regex: /(?<!\\[ \t]*)$/,
                        next: "pop"
                    }, {
                        include: "#strings"
                    }, {
                        include: "#numbers"
                    }, {
                        include: "#keywords"
                    }, {
                        include: "#constants"
                    }, {
                        include: "#functions"
                    }, {
                        include: "#function_calls"
                    }, {
                        include: "#structs"
                    }, {
                        include: "#variables"
                    }, {
                        defaultToken: "storage.type.macro.gml"
                    }]
            }],
        "#structs": [{
                token: "entity.name.struct.gml",
                regex: /\bstruct\b/
            }],
        "#variables": [{
                token: "variable.other.global.gml",
                regex: /\bglobal\.\w+\b/
            }, {
                token: "variable.other.local.gml",
                regex: /\b\w+\b/
            }, {
                token: "variable.other.member.gml",
                regex: /\bself\.\w+\b/
            }, {
                token: "variable.other.object.gml",
                regex: /\bother\.\w+\b/
            }],
        "#escape_sequences": [{
                token: "constant.character.escape.gml",
                regex: /\\(?:[btnfr"'\\]|[0-7]{1,3}|x[0-9a-fA-F]{2})/
            }],
        "#jsdoc": [{
                token: [
                    "punctuation.definition.comment.gml",
                    "comment.block.documentation.gml",
                    "storage.type.class.gml",
                    "comment.block.documentation.gml"
                ],
                regex: /(\/\/\/|\*)(\s*)(@(?:description|desc|deprecated|ignore|pure|mixin))\b(.*$)/
            }, {
                token: [
                    "punctuation.definition.comment.gml",
                    "comment.block.documentation.gml",
                    "storage.type.class.gml",
                    "comment.block.documentation.gml",
                    "entity.name.type.instance.gml"
                ],
                regex: /(\/\/\/|\*)(\s*)(@(?:self|context))(\s+)([^\s]+)/,
                push: [{
                        token: "comment.block.documentation.gml",
                        regex: /$/,
                        next: "pop"
                    }, {
                        defaultToken: "comment.block.documentation.gml"
                    }]
            }, {
                token: [
                    "punctuation.definition.comment.gml",
                    "comment.block.documentation.gml",
                    "storage.type.class.gml",
                    "comment.block.documentation.gml",
                    "punctuation.definition.bracket.curly.begin.gml",
                    "comment.block.documentation.gml",
                    "entity.name.type.instance.gml",
                    "comment.block.documentation.gml",
                    "punctuation.definition.bracket.curly.end.gml"
                ],
                regex: /(\/\/\/|\*)(\s*)(@(?:returns?|type))\b(?:(\s*)(\{)(\s*)([^}]+)(\s*)(\}))?/,
                push: [{
                        token: "comment.block.documentation.gml",
                        regex: /$/,
                        next: "pop"
                    }, {
                        defaultToken: "comment.block.documentation.gml"
                    }]
            }, {
                token: [
                    "punctuation.definition.comment.gml",
                    "comment.block.documentation.gml",
                    "storage.type.class.gml",
                    "comment.block.documentation.gml",
                    "punctuation.definition.bracket.curly.begin.gml",
                    "comment.block.documentation.gml",
                    "entity.name.type.instance.gml",
                    "comment.block.documentation.gml",
                    "punctuation.definition.bracket.curly.end.gml",
                    "comment.block.documentation.gml",
                    "punctuation.definition.optional-value.begin.bracket.square.gml",
                    "comment.block.documentation.gml",
                    "variable.other.gml",
                    "comment.block.documentation.gml",
                    "punctuation.definition.optional-value.end.bracket.square.gml"
                ],
                regex: /(\/\/\/|\*)(\s*)(@(?:param|arg|argument|parameter|localvar|var|globalvar|prop|property|instancevar|template))\b(?:(\s*)(\{)(\s*)([^}]+)(\s*)(\}))?(?:(\s*)((?:\[)?)(\s*)([_$[:alpha:]][_$[:alnum:]]*)(\s*)((?:\])?))?/,
                push: [{
                        token: "comment.block.documentation.gml",
                        regex: /$/,
                        next: "pop"
                    }, {
                        defaultToken: "comment.block.documentation.gml"
                    }]
            }]
    };
    this.normalizeRules();
};
GameMakerLanguageHighlightRules.metaData = {
    "$schema": "https://raw.githubusercontent.com/martinring/tmlanguage/master/tmlanguage.json",
    name: "GameMaker Language",
    scopeName: "source.gml",
    fileTypes: ["gml"]
};
oop.inherits(GameMakerLanguageHighlightRules, TextHighlightRules);
exports.GameMakerLanguageHighlightRules = GameMakerLanguageHighlightRules;

});

ace.define("ace/mode/folding/cstyle",["require","exports","module","ace/lib/oop","ace/range","ace/mode/folding/fold_mode"], function(require, exports, module){"use strict";
var oop = require("../../lib/oop");
var Range = require("../../range").Range;
var BaseFoldMode = require("./fold_mode").FoldMode;
var FoldMode = exports.FoldMode = function (commentRegex) {
    if (commentRegex) {
        this.foldingStartMarker = new RegExp(this.foldingStartMarker.source.replace(/\|[^|]*?$/, "|" + commentRegex.start));
        this.foldingStopMarker = new RegExp(this.foldingStopMarker.source.replace(/\|[^|]*?$/, "|" + commentRegex.end));
    }
};
oop.inherits(FoldMode, BaseFoldMode);
(function () {
    this.foldingStartMarker = /([\{\[\(])[^\}\]\)]*$|^\s*(\/\*)/;
    this.foldingStopMarker = /^[^\[\{\(]*([\}\]\)])|^[\s\*]*(\*\/)/;
    this.singleLineBlockCommentRe = /^\s*(\/\*).*\*\/\s*$/;
    this.tripleStarBlockCommentRe = /^\s*(\/\*\*\*).*\*\/\s*$/;
    this.startRegionRe = /^\s*(\/\*|\/\/)#?region\b/;
    this._getFoldWidgetBase = this.getFoldWidget;
    this.getFoldWidget = function (session, foldStyle, row) {
        var line = session.getLine(row);
        if (this.singleLineBlockCommentRe.test(line)) {
            if (!this.startRegionRe.test(line) && !this.tripleStarBlockCommentRe.test(line))
                return "";
        }
        var fw = this._getFoldWidgetBase(session, foldStyle, row);
        if (!fw && this.startRegionRe.test(line))
            return "start"; // lineCommentRegionStart
        return fw;
    };
    this.getFoldWidgetRange = function (session, foldStyle, row, forceMultiline) {
        var line = session.getLine(row);
        if (this.startRegionRe.test(line))
            return this.getCommentRegionBlock(session, line, row);
        var match = line.match(this.foldingStartMarker);
        if (match) {
            var i = match.index;
            if (match[1])
                return this.openingBracketBlock(session, match[1], row, i);
            var range = session.getCommentFoldRange(row, i + match[0].length, 1);
            if (range && !range.isMultiLine()) {
                if (forceMultiline) {
                    range = this.getSectionRange(session, row);
                }
                else if (foldStyle != "all")
                    range = null;
            }
            return range;
        }
        if (foldStyle === "markbegin")
            return;
        var match = line.match(this.foldingStopMarker);
        if (match) {
            var i = match.index + match[0].length;
            if (match[1])
                return this.closingBracketBlock(session, match[1], row, i);
            return session.getCommentFoldRange(row, i, -1);
        }
    };
    this.getSectionRange = function (session, row) {
        var line = session.getLine(row);
        var startIndent = line.search(/\S/);
        var startRow = row;
        var startColumn = line.length;
        row = row + 1;
        var endRow = row;
        var maxRow = session.getLength();
        while (++row < maxRow) {
            line = session.getLine(row);
            var indent = line.search(/\S/);
            if (indent === -1)
                continue;
            if (startIndent > indent)
                break;
            var subRange = this.getFoldWidgetRange(session, "all", row);
            if (subRange) {
                if (subRange.start.row <= startRow) {
                    break;
                }
                else if (subRange.isMultiLine()) {
                    row = subRange.end.row;
                }
                else if (startIndent == indent) {
                    break;
                }
            }
            endRow = row;
        }
        return new Range(startRow, startColumn, endRow, session.getLine(endRow).length);
    };
    this.getCommentRegionBlock = function (session, line, row) {
        var startColumn = line.search(/\s*$/);
        var maxRow = session.getLength();
        var startRow = row;
        var re = /^\s*(?:\/\*|\/\/|--)#?(end)?region\b/;
        var depth = 1;
        while (++row < maxRow) {
            line = session.getLine(row);
            var m = re.exec(line);
            if (!m)
                continue;
            if (m[1])
                depth--;
            else
                depth++;
            if (!depth)
                break;
        }
        var endRow = row;
        if (endRow > startRow) {
            return new Range(startRow, startColumn, endRow, line.length);
        }
    };
}).call(FoldMode.prototype);

});

ace.define("ace/mode/game_maker_language",["require","exports","module","ace/lib/oop","ace/mode/text","ace/mode/game_maker_language_highlight_rules","ace/mode/folding/cstyle"], function(require, exports, module){/* ***** BEGIN LICENSE BLOCK *****
 * Distributed under the BSD license:
 *
 * Copyright (c) 2012, Ajax.org B.V.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of Ajax.org B.V. nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL AJAX.ORG B.V. BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * ***** END LICENSE BLOCK ***** */
"use strict";
var oop = require("../lib/oop");
var TextMode = require("./text").Mode;
var GameMakerLanguageHighlightRules = require("./game_maker_language_highlight_rules").GameMakerLanguageHighlightRules;
var FoldMode = require("./folding/cstyle").FoldMode;
var Mode = function () {
    this.HighlightRules = GameMakerLanguageHighlightRules;
    this.foldingRules = new FoldMode();
};
oop.inherits(Mode, TextMode);
(function () {
    this.$id = "ace/mode/game_maker_language";
}).call(Mode.prototype);
exports.Mode = Mode;

});
                (function() {
                    ace.require(["ace/mode/game_maker_language"], function(m) {
                        if (typeof module == "object" && typeof exports == "object" && module) {
                            module.exports = m;
                        }
                    });
                })();
            