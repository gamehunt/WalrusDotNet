parser grammar DiceLangParser;
options { tokenVocab=DiceLangLexer; }

/*
 * Parser rules
 */

message: FORCE_CALC_MARKER? valid_expression label? EOF;

valid_expression
    : repeated_expr
    | expr
    | initiative
    | list
    ;

repeated_expr: amount=INTEGER REPEATER expr ;

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
    : amount=INTEGER? DICE sides=INTEGER dice_discard_group?
    ;

dice_discard_group
    : DICE discard=(LIST|DISCARD_HI)? amount=INTEGER
    ;
    
label
    : LABEL
    ;

list
    : amount=INTEGER? UNIQ? LIST LSQBRACE items+=STRING (LIST_SEP_ALT items+=STRING)* RSQBRACE
    ;

initiative: INIT LSQBRACE contestants+=initiative_contestant (LIST_SEP contestants+=initiative_contestant)* RSQBRACE ;
initiative_contestant: LBRACE name=STRING LIST_SEP_ALT mod_sign=(PLUS|MINUS) mod=INTEGER advantage=(PLUS|MINUS)? RBRACE ;
