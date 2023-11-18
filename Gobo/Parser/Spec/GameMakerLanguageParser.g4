parser grammar GameMakerLanguageParser;

options {tokenVocab=GameMakerLanguageLexer;}

program
    : statementList? EOF
    ;

statementList
    : (statement | emptyStatement)+
    ;

statementNoSemicolon
    : (block
    | ifStatement
    | variableDeclarationList
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
    | globalVarStatement
    | assignmentExpression
    | incDecStatement
    | callStatement
    | functionDeclaration
    | deleteStatement
    )
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

assignmentExpression
    : lValueExpression typeAnnotation? assignmentOperator expressionOrFunction
    ;

variableDeclarationList
    : varModifier variableDeclaration (',' variableDeclaration)*
    ;

varModifier
    : Var+
    | Static
    ;

variableDeclaration
    : identifier typeAnnotation? (Assign expressionOrFunction)?
    ;

typeAnnotation
    : ':' identifier ('|' identifier)*
    ;

globalVarStatement
    : GlobalVar identifier (',' identifier)* SemiColon
    ;

lValueStartExpression
    : identifier
    | '(' expressionOrFunction ')'
    | newExpression
    ;

lValueExpression
    : lValueStartExpression (lValueChainOperator* lValueFinalOperator)?
    ;

lValueChainOperator
    : accessor expressionSequence ']' # MemberIndexLValue
    | '.' identifier # MemberDotLValue
    | arguments # CallLValue
    ;

lValueFinalOperator
    : accessor expressionSequence ']' # MemberIndexLValueFinal
    | '.' identifier # MemberDotLValueFinal
    ;

newExpression
    : New identifier? arguments
    ;

expressionSequence
    : expression (',' expression)*
    ;

expressionOrFunction
    : (expression | functionDeclaration)
    | '(' expressionOrFunction ')'
    ;

expression
    : incDecStatement # IncDecExpression
    | callStatement # CallExpression
    | lValueExpression # VariableExpression

    | <assoc=right> '+' expression # UnaryPlusExpression
    | <assoc=right> '-' expression # UnaryMinusExpression
    
    | <assoc=right> '~' expression # BitNotExpression
    | <assoc=right> Not expression # NotExpression
    
    // Binary operator precedence is purposely incorrect because it looks prettier that way :)
    | expression ('*' | '/' | Modulo | IntegerDivide) expression # MultiplicativeExpression
    | expression ('+' | '-') expression # AdditiveExpression
    | expression ('<<' | '>>') expression # BitShiftExpression
    | expression ('<' | '<=' | '>' | '>=') expression # RelationalExpression
    | expression ('==' | Assign) expression # EqualityExpression
    | expression NotEquals expression # InequalityExpression

    | expression Or expression # LogicalOrExpression
    | expression And expression # LogicalAndExpression
    | expression Xor expression # LogicalXorExpression
    | expression '??' expression # CoalesceExpression
    
    | expression '&' expression # BitAndExpression
    | expression '|' expression # BitOrExpression
    | expression '^' expression # BitXOrExpression

    | <assoc=right> expression '?' expression ':' expression # TernaryExpression
    | literal # LiteralExpression
    | '(' expression ')' # ParenthesizedExpression
    ;

callStatement
    : callableExpression arguments
    | callStatement arguments
    ;

callableExpression
    : lValueExpression
    | '(' (functionDeclaration | callableExpression) ')'
    ;

incDecStatement
    : ('++' | '--') lValueExpression # PreIncDecExpression
    | lValueExpression ('++' | '--') # PostIncDecExpression
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
    : ','* expressionOrFunction? (','+ expressionOrFunction)* ','*
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
    : '[' (expressionOrFunction (',' expressionOrFunction)* ','? )? ']'
    ;

structLiteral
    : openBlock (propertyAssignment (',' propertyAssignment)* ','?)? closeBlock
    ;

propertyAssignment
    : (propertyIdentifier | StringLiteral) ':' expressionOrFunction
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

// TODO: fix this name? lol
parameter
    : identifier typeAnnotation? (Assign expressionOrFunction)?
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