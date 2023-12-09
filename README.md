## EBNF Grammar
```
block : '{' statement* '}'
      | statement     

statement : assignment_statement
          | if_statement
          | for_statement
          | while_statement
          | assignment_expression_statement

while_statement : while_kw '(' expression ')' block
if_statement : if_kw '(' expression ')' block [ else_kw block ]
for_statement : for_kw '(' (assignment_statement | assignment_expression_statement) ';' condition ';' assignment_expression_statement ')' block

assignment_statement : LET_KEYWORD name '=' expression
assignment_expression_statement : name '=' expression

expression : 
           | ('-' | '+') expression
           | expression op=('*' | '+') expression
           | expression op=('+' | '-') expression
           | '!' expression
           | expression op=('&&' | '||') expression
           | name "=" expression
           | primary
           ...
           
primary : '(' expression ')'
        | name
        | number
        | boolean
```