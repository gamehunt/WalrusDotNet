parser grammar DiceLangParser;
options { tokenVocab=DiceLangLexer; }

/*
 * Parser rules
 */

message: valid_expression label? EOF;

valid_expression
    : repeated_expr
    | expr
    | initiative
    ;

repeated_expr: INTEGER REPEATER expr ;

expr
    : expr POW expr # powExpr
    | expr (MUL | DIV) expr # mulExpr
    | expr (PLUS | MINUS) expr # addExpr
    | LBRACE expr RBRACE # parentExpr
    | (PLUS | MINUS)? operand # unaryExpr
    ;

operand
    : numeric
    | dice
    ;

numeric
    : FLOAT 
    | INTEGER
    ;

dice
    : amount=INTEGER? DICE_MARKER sides=INTEGER
    ;
    
label
    : LABEL
    ;

initiative: INIT LSQBRACE initiative_contestant (LIST_SEP initiative_contestant)* RSQBRACE ;
initiative_contestant: LBRACE STRING LIST_SEP_ALT (PLUS|MINUS) INTEGER (PLUS)? RBRACE ;
