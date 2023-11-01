lexer grammar GameMakerLanguageLexer;

channels { ERROR }

@lexer::members {
    bool ignoreNewline = true;
    int lastTokenType = -4;
    int templateDepth = 0;
    public override IToken NextToken() {
        var next = base.NextToken();
        if (next.Channel == Lexer.DefaultTokenChannel) {
            lastTokenType = next.Type;
        }
        return next;
    }
}

MultiLineComment:               '/*' .*? '*/'             -> channel(HIDDEN);
SingleLineComment:              '//' ~[\r\n\u2028\u2029]* -> channel(HIDDEN);

OpenBracket: '[';
ListAccessor: '[|'; 
MapAccessor: '[?'; 
GridAccessor: '[#';
ArrayAccessor: '[@';
StructAccessor: '[$'; 
CloseBracket: ']';

OpenParen:                      '(';
CloseParen:                     ')';
OpenBrace:                      '{';
TemplateStringEndExpression:    '}' {templateDepth > 0}? -> popMode;
CloseBrace:                     '}';
Begin:                          'begin';
End:                            'end';
SemiColon:                      ';';
Comma:                          ',';
Assign:                         '=' | ':=';
Colon:                          ':';
Dot:                            '.';
PlusPlus:                       '++';
MinusMinus:                     '--';
Plus:                           '+';
Minus:                          '-';
BitNot:                         '~';
Not:                            '!' | 'not';
Multiply:                       '*';
Divide:                         '/';
IntegerDivide:                  'div';
Modulo:                         '%' | 'mod';
Power:                          '**';
QuestionMark:                   '?';
NullCoalesce:                   '??';
NullCoalescingAssign:           '??=';
RightShiftArithmetic:           '>>';
LeftShiftArithmetic:            '<<';
LessThan:                       '<';
MoreThan:                       '>';
LessThanEquals:                 '<=';
GreaterThanEquals:              '>=';
Equals_:                        '==';
NotEquals:                      '!=' | '<>';
BitAnd:                         '&';
BitXOr:                         '^';
BitOr:                          '|';
And:                            '&&' | 'and';
Or:                             '||' | 'or';
Xor:                            '^^' | 'xor';
MultiplyAssign:                 '*=';
DivideAssign:                   '/=';
PlusAssign:                     '+=';
MinusAssign:                    '-=';
ModulusAssign:                  '%=';
LeftShiftArithmeticAssign:      '<<=';
RightShiftArithmeticAssign:     '>>=';
BitAndAssign:                   '&=';
BitXorAssign:                   '^=';
BitOrAssign:                    '|=';
NumberSign:                     '#';
DollarSign:                     '$';
AtSign:                         '@';

/// Boolean Literals

BooleanLiteral:                 'true' | 'false';

/// Numeric Literals

IntegerLiteral:                 DecimalIntegerLiteral;
DecimalLiteral:                 DecimalIntegerLiteral? '.' [0-9] [0-9_]* ;
BinaryLiteral:                  '0b' ('_'* [01])+;
HexIntegerLiteral:              HexLiteralPrefix [0-9a-fA-F] HexDigit*;

/// Keywords

Undefined: 'undefined';
NoOne: 'noone';

Break:                          'break';
Exit:                           'exit';
Do:                             'do';
Case:                           'case';
Else:                           'else';
New:                            'new';
Var:                            'var';
GlobalVar:                      'globalvar';
Catch:                          'catch';
Finally:                        'finally';
Return:                         'return';
Continue:                       'continue';
For:                            'for';
Switch:                         'switch';
While:                          'while';
Until:                          'until';
Repeat:                         'repeat';
Function:                       'function';
With:                           'with';
Default:                        'default';
If:                             'if';
Then:                           'then';
Throw:                          'throw';
Delete:                         'delete';
Try:                            'try';
Enum:                           'enum';
Constructor:                    'constructor';
Static:                         'static';

Macro: '#macro' {ignoreNewline = false;};
EscapedNewLine: '\\';

Define: '#define' -> pushMode(REGION_NAME);
Region: '#region' -> pushMode(REGION_NAME);
EndRegion: '#endregion' -> pushMode(REGION_NAME);

/// Identifier Names and Identifiers

Identifier
    : IdentifierStart IdentifierPart*
    ;
    
/// String Literals

StringLiteral: '"' StringCharacter* '"';

TemplateStringStart
    : '$"' {templateDepth++;} -> pushMode(TEMPLATE_STRING)
    ;

VerbatimStringLiteral
    : '@"' (~'"' | '""')* '"'
    | '@\'' (~'\'' | '\'\'')* '\''
    ;
	
/// Misc

WhiteSpaces
    : [\t\u000B\u000C\u0020\u00A0]+ -> channel(HIDDEN)
    ;

LineTerminator
    : [\r\n\u2028\u2029] {
        if (ignoreNewline) {
            Channel = Lexer.Hidden;
        }
        if (lastTokenType == GameMakerLanguageLexer.EscapedNewLine) {
            Channel = Lexer.Hidden;
        } else {
            ignoreNewline = true;
        }
    }
    ;

UnexpectedCharacter
    : . -> channel(ERROR)
    ;

/// Fragment rules

fragment IdentifierStart
    : [\p{L}] | '_'
    ;
    
fragment IdentifierPart
    : IdentifierStart
    | [\p{Mn}]
    | [\p{Nd}]
    | [\p{Pc}]
    ;

fragment StringCharacter
    : ~["\\\r\n]
    | '\\' SingleEscapeCharacter
    ;

fragment HexLiteralPrefix
    : '0x' | '$' | '#'
    ;
    
fragment HexDigit
    : [_0-9a-fA-F]
    ;

fragment SingleEscapeCharacter
    : ['"\\bfnrtv]
    ;

fragment DecimalIntegerLiteral
    : '0'
    | [1-9] [0-9_]*
    ;

mode REGION_NAME;

RegionCharacters
    : ~[\r\n\u2028\u2029]+
    ;
    
RegionEOL
    : [\r\n\u2028\u2029] -> popMode
    ;

mode TEMPLATE_STRING;

TemplateStringEnd: '"' {templateDepth--;} -> popMode;
TemplateStringStartExpression: '{' -> pushMode(DEFAULT_MODE);
TemplateStringText: ~('{' | '\\' | '"' | [\r\n\u2028\u2029])+;