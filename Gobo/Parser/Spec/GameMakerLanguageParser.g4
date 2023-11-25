parser grammar GameMakerLanguageParser;

options {tokenVocab=GameMakerLanguageLexer;}

program
    : statementList? EOF
    ;

statementList
    : (statement | emptyStatement)+
    ;

statementNoSemicolon
    : block
    | assignment
    | unaryExpression
    | ifStatement
    | iterationStatement
    | continueStatement
    | breakStatement
    | returnStatement
    | withStatement
    | switchStatement
    | tryStatement
    | throwStatement
    | exitStatement
    | macroStatement
    | defineStatement
    | regionStatement
    | enumeratorDeclaration
    | functionDeclaration
    | deleteStatement
    ;

statement 
    : statementNoSemicolon eos*
    ;

block
    : openBlock statementList? closeBlock
    ;

ifStatement
    : If expression Then? statement (Else statement)?
    ;

iterationStatement
    : Do statement Until expression # DoStatement
    | While expression statement # WhileStatement
    | For '(' 
        statementNoSemicolon? ';' 
        expression? ';' 
        statement? 
        ')' statement # ForStatement
    | Repeat expression statement # RepeatStatement
    ;

withStatement
    : With expression statement
    ;

switchStatement
    : Switch expression caseBlock
    ;

continueStatement
    : Continue
    ;

breakStatement
    : Break
    ;

exitStatement
    : Exit
    ;

emptyStatement
    : SemiColon
    ;

caseBlock
    : openBlock caseClause* closeBlock
    ;

caseClause
    : Default ':' statementList?
    | Case expression ':' statementList?
    ;

throwStatement
    : Throw expression
    ;

tryStatement
    : Try statement (catchProduction finallyProduction? | finallyProduction)
    ;

catchProduction
    : Catch ('(' identifier? ')')? statement
    ;

finallyProduction
    : Finally statement
    ;

returnStatement
    : Return expression?
    ;

deleteStatement
    : Delete expression
    ;

assignment
    : primaryExpression assignmentOperator expression # PrimaryAssignment
    | varModifier variableDeclaration (',' variableDeclaration)* # VariableDeclarationList
    ;

varModifier
    : Var+
    | Static
    | GlobalVar
    ;

variableDeclaration
    : identifier (Assign expression)?
    ;

expression
    : conditionalExpression
    | functionDeclaration
    // future arrow function syntax goes here
    ;

conditionalExpression
    : bitXorExpression ('?' expression ':' expression)?
    ;

bitXorExpression
    : bitOrExpression ('^' bitOrExpression)*
    ;

bitOrExpression
    : bitAndExpression ('|' bitAndExpression)*
    ;

bitAndExpression
    : nullCoalescingExpression ('&' nullCoalescingExpression)*
    ;

nullCoalescingExpression
    : xorExpression ('??' xorExpression)?
    ;

xorExpression
    : andExpression (Xor andExpression)*
    ;

andExpression
    : orExpression (And orExpression)*
    ;

orExpression
    : equalityExpression (Or equalityExpression)*
    ;

equalityExpression
    : relationalExpression (('==' | Assign | NotEquals) relationalExpression)*
    ;

relationalExpression
    : shiftExpression (('<' | '>' | '<=' | '>=') shiftExpression)*
    ;

shiftExpression
    : additiveExpression (('<<' | '>>') additiveExpression)*
    ;

additiveExpression
    : multiplicativeExpression (('+' | '-') multiplicativeExpression)*
    ;

multiplicativeExpression
    : unaryExpression (('*' | '/' | Modulo | IntegerDivide) unaryExpression)*
    ;

unaryExpression
    : primaryExpression
    | (
        '+' 
        | '-' 
        | Not 
        | '~' 
        | '++' 
        | '--'
    ) primaryExpression
    ;

primaryExpression 
	: primaryExpressionStart (memberIndex | memberDot | methodInvocation)* ('++' | '--')?
    ;

primaryExpressionStart
    : literal # LiteralExpression
    | identifier # SimpleNameExpression
    | '(' expression ')' # ParenthesizedExpression
    | New identifier? arguments # NewExpression
    ;

memberIndex
    : accessor expression (',' expression)* ']'
    ;

memberDot
    : '.' identifier
    ;

methodInvocation
    : arguments
    ;

accessor
    : OpenBracket
    | ListAccessor
    | MapAccessor
    | GridAccessor
    | ArrayAccessor
    | StructAccessor
    ;

arguments
    : '(' argumentList ')'
    ;

argumentList
    : ','* expression? (','+ expression)* ','*
    ;

assignmentOperator
    : '*='
    | '/='
    | '%='
    | '+='
    | '-='
    | '<<='
    | '>>='
    | '&='
    | '^='
    | '|='
    | '??='
    | Assign
    ;

literal
    : Undefined
    | NoOne
    | BooleanLiteral
    | StringLiteral
    | VerbatimStringLiteral
    | templateStringLiteral
    | HexIntegerLiteral
    | BinaryLiteral
    | DecimalLiteral
    | IntegerLiteral
    | arrayLiteral
    | structLiteral
    ;

templateStringLiteral
    : TemplateStringStart templateStringAtom* TemplateStringEnd
    ;

templateStringAtom
    : TemplateStringText
    | TemplateStringStartExpression expression TemplateStringEndExpression
    ;

arrayLiteral
    : '[' (expression (',' expression)* ','? )? ']'
    ;

structLiteral
    : openBlock (propertyAssignment (',' propertyAssignment)* ','?)? closeBlock
    ;

propertyAssignment
    : (propertyIdentifier | StringLiteral) ':' expression
    ;

propertyIdentifier
    : Identifier | softKeyword | propertySoftKeyword
    ;

functionDeclaration
    : Function Identifier? parameterList constructorClause? block
    ;

constructorClause
    : (':' Identifier arguments)? Constructor
    ;

parameterList
    : '(' (parameter (',' parameter)* ','?)? ')'
    ;

parameter
    : identifier (Assign expression)?
    ;

identifier
    : Identifier | softKeyword
    ;

enumeratorDeclaration
    : Enum identifier enumeratorBlock
    ;

enumeratorBlock
    : openBlock ( enumerator (',' enumerator)* ','? )? closeBlock
    ;

enumerator
    : identifier (Assign expression)?
    ;

macroStatement
    : Macro identifier macroToken+ (LineTerminator | EOF)
    ;

defineStatement
    : Define RegionCharacters (RegionEOL | EOF)
    ;

regionStatement
    : (Region | EndRegion) RegionCharacters? (RegionEOL | EOF)
    ;

// handles macros used as statements
identifierStatement
    : identifier
    ;

softKeyword
    : Constructor
    ;

propertySoftKeyword
    : NoOne
    ;

openBlock
    : OpenBrace | Begin
    ;

closeBlock
    : CloseBrace | End
    ;

eos
    : SemiColon
    ;

// every token except:
// WhiteSpaces, LineTerminator, Define, Macro, Region, EndRegion, UnexpectedCharacter
// includes EscapedNewLine
macroToken
    : EscapedNewLine | OpenBracket | CloseBracket | OpenParen | CloseParen
    | OpenBrace | CloseBrace | Begin | End | SemiColon | Comma | Assign | Colon
    | Dot | PlusPlus | MinusMinus | Plus | Minus | BitNot | Not | Multiply | Divide
    | IntegerDivide | Modulo | Power | QuestionMark | NullCoalesce
    | NullCoalescingAssign | RightShiftArithmetic | LeftShiftArithmetic
    | LessThan | MoreThan | LessThanEquals | GreaterThanEquals | Equals_ | NotEquals
    | BitAnd | BitXOr | BitOr | And | Or | Xor | MultiplyAssign | DivideAssign | PlusAssign
    | MinusAssign | ModulusAssign | LeftShiftArithmeticAssign | RightShiftArithmeticAssign
    | BitAndAssign | BitXorAssign | BitOrAssign | NumberSign | DollarSign | AtSign
    | Undefined | NoOne | BooleanLiteral | IntegerLiteral | DecimalLiteral
    | BinaryLiteral | HexIntegerLiteral | Break | Exit | Do | Case | Else | New
    | Var | GlobalVar | Catch | Finally | Return | Continue | For | Switch | While
    | Until | Repeat | Function | With | Default | If | Then | Throw | Delete
    | Try | Enum | Constructor | Static | Identifier | StringLiteral | VerbatimStringLiteral
    | TemplateStringStart | TemplateStringEnd | TemplateStringText | TemplateStringStartExpression
    | TemplateStringEndExpression | OpenBracket | ListAccessor | MapAccessor | GridAccessor | ArrayAccessor
    | StructAccessor
    ;