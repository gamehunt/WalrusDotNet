lexer grammar DiceLangLexer;
/*
 * Lexer rules
 */


/* Operands */
FLOAT: INTEGER DOT INTEGER ;
INTEGER: [0-9]+ ;

fragment DOT: '.' ;
DICE: 'd' ;
DISCARD_HI: 'h' ;
FORCE_CALC_MARKER: 'r' | '&';
LIST: 'l';
UNIQ: 'u';

/* Operations */
PLUS: '+' ;
MINUS: '-' ;
DIV: '/' ;
MUL: '*' ;
POW: '^' ;

LBRACE: '(' ;
RBRACE: ')' ;

REPEATER: '#' ;

INIT: 'init' ;

LSQBRACE: '[' ;
RSQBRACE: ']' ;

LQUOTE : '"' -> more, mode(STR) ;

LIST_SEP: ',' ;
LIST_SEP_ALT: ';' ;

WHITESPACE: [ \r\n\t]+ -> skip ;
ANY: . -> more, mode(GARBAGE) ;

mode STR;
STRING : '"' -> mode(DEFAULT_MODE) ;
TEXT : . -> more ;

mode GARBAGE;
LABEL : EOF -> mode(DEFAULT_MODE) ;
LABEL_TEXT : . -> more;