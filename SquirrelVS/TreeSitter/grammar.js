module.exports = grammar({
  name: 'squirrel',
  
  extras: $ => [
    $.comment,
    /[\s\p{Zs}\uFEFF\u2060\u200B]/,
  ],

  supertypes: $ => [
    $._statement,
    $.expression,
    $.primary_expression,
    $.pattern,
    $.deref_expression,
    $.expression_statement
  ],

  precedences: $ => [
    [
      'member',
      'call',
      $.update_expression,
      'unary_void',
      'binary_newslot',
      'binary_exp',
      'binary_times',
      'binary_plus',
      'binary_shift',
      'binary_compare',
      'binary_relation',
      'binary_equality',
      'bitwise_and',
      'bitwise_xor',
      'bitwise_or',
      'logical_and',
      'logical_or',
      'ternary',
      $.sequence_expression
    ],
    ['assign', 'newslot', $.primary_expression],
    ['member', 'new', $.expression],
    ['declaration', 'literal'],
  ],
  
  conflicts: $ => [
    [$.primary_expression, $.pattern],
    [$.assignment_expression, $.pattern],
    [$.newslot_expression, $.pattern],
    [$.binary_expression, $.initializer],
    [$.call_expression, $.initializer],
    [$.table_slot_assignment, $.assignment_expression],
  ],

  inline: $ => [
    $._expressions,
    $._lhs_expression,
    $._statement
  ],

  word: $ => $.identifier,

  rules: {
    program: $ => seq(
      repeat($._statement),
      optional($.semicolon)
    ),

    _statement: $ => choice(
      $.block_statement,
      $.if_statement,
      $.switch_statement,
      $.break_statement,
      
      $.return_statement,
      $.yield_statement,
      $.delete_statement,

      $.continue_statement,
      $.while_statement,
      $.do_statement,
      $.for_statement,
      $.foreach_statement,

      $.try_statement,
      $.throw_statement,

      $.local_declaration,
      $.const_declaration,
      $.enum_declaration,

      $.expression_statement
    ),

    block_statement: $ => seq(
      '{',
      repeat($._statement),
      '}'
    ),

    true:  $ => 'true',
    false: $ => 'false',
    null:  $ => 'null',
    this:  $ => 'this',
    
    vargc: $ => 'vargc',
    vargv: $ => 'vargv',

    identifier: $ => /[a-zA-Z_][a-zA-Z0-9_]*/,
    
    namespaced_identifier: $ => prec.right(1, choice(
      $.identifier,
      $.namespace_expression
    )),

    // Statements

    while_statement: $ => seq(
      'while',
      field('condition', $.parenthesized_expression),
      field('body', $._statement)
    ),

    do_statement: $ => seq(
      'do',
      field('body', $._statement),
      'while',
      field('condition', $.parenthesized_expression)
    ),

    if_statement: $ => prec.right(seq(
      'if',
      field('condition',            $.parenthesized_expression),
      field('consequence',          $._statement),
      optional(field('alternative', $.else_clause))
    )),

    switch_statement: $ => seq(
      'switch',
      field('value', $.parenthesized_expression),
      field('body',  $.switch_body)
    ),
    
    return_statement: $ => prec.right(seq(
      'return',
      optional($._expressions),
      optional($.semicolon)
    )),

    yield_statement: $ => prec.right(seq(
      'yield',
      optional($._expressions),
      optional($.semicolon)
    )),
    
    delete_statement: $ => prec.right(seq(
      'delete',
      $.deref_expression,
      optional($.semicolon)
    )),
    
    expression_statement: $ => prec.right(seq(
      $.expression
    )),

    for_field_statement: $ => prec.right(seq(
      optional($._expressions),
      $.semicolon
    )),

    continue_statement: $ => prec.right(seq(
      'continue',
      optional($.semicolon)
    )),

    break_statement: $ => prec.right(seq(
      'break',
      optional($.semicolon)
    )),

    for_statement: $ => prec.right(seq(
      'for',
      '(',
      field('initializer', choice($.for_field_statement, $.local_declaration)),
      field('condition', $.for_field_statement),
      field('increment', optional($.expression)),
      ')',
      field('body', $._statement)
    )),

    foreach_statement: $ => seq(
      'foreach',
      '(',
      field('index_id', $.identifier),
      optional(seq(',', field('value_id', $.identifier))),
      'in',
      field('iterable', $.expression),
      ')',
      field('body', $._statement)
    ),

    try_statement: $ => seq(
      'try',
      field('body', $.statement_block),
      field('handler', $.catch_clause),
    ),
    
    catch_clause: $ => seq(
      'catch',
      optional(seq('(', field('parameter', $.identifier), ')')),
      field('body', $.statement_block)
    ),
    
    throw_statement: $ => prec.right(seq(
      'throw',
      $._expressions,
      optional($.semicolon)
    )),

    // Expressions

    _lhs_expression: $ => choice(
      $.member_expression,
      $.namespaced_identifier,
      $.subscript_expression,
      $.variadic_expression
    ),

    deref_expression: $ => prec.right(2, choice(
      $.member_expression,
      $.namespace_expression,
      $.identifier,
      $.subscript_expression,
      $.vargv,
      $.vargc
    )),

    member_expression: $ => prec.right('member', seq(
      field('object', choice($.namespaced_identifier, $.expression)),
      '.',
      field('property', alias($.identifier, $.property_identifier)),
    )),

    subscript_expression: $ => prec.right('member', seq(
      field('object', $.deref_expression),
      '[',
      field('index', $.expression),
      ']'
    )),

    parenthesized_expression: $ => seq(
      '(',
      $._expressions,
      ')'
    ),

    _expressions: $ => choice(
      $.expression,
      $.sequence_expression
    ),

    expression: $ => choice(
      $.update_expression,
      $.primary_expression,
      $.assignment_expression,
      $.newslot_expression,
      $.augmented_assignment_expression,
      $.unary_expression,
      $.binary_expression,
      $.ternary_expression
    ),

    primary_expression: $ => choice(
      $.this,
      $.null,
      $.vargc,
      $.vargv,
      $.true,
      $.false,
      $.identifier,
      $.parenthesized_expression,
      $.number,
      $.string,
      $.array,
      $.function_expression,
      $.class_expression,
      $.member_expression,
      $.subscript_expression,
      $.call_expression,
      $.table_expression,
      $.delegate_expression,
      $.resume_expression
    ),

    table_expression: $ => prec.right(2, seq(
      '{',
      optional(
        repeat(seq(
          choice(
            $.table_slot_assignment,
            $.table_slot_expression
           ),
          )
        )),
      '}',
      optional($.semicolon)
    )),

    table_slot_expression: $ => prec.right(seq(
      '[',
      field('expression', $.expression),
      ']',
      field('initializer', $.initializer),
    )),
    
    table_slot_assignment: $ => prec.right(seq(
      field('left', $.namespaced_identifier),
      '=',
      field('right', $.expression),
    )),

    assignment_expression: $ => prec.right('assign', seq(
      field('left', choice($.parenthesized_expression, $._lhs_expression)),
      '=',
      field('right', $.expression),
      optional($.semicolon)
    )),
    
    newslot_expression: $ => prec.right('newslot', seq(
      field('left', choice($.parenthesized_expression, $._lhs_expression)),
      '<-',
      field('right', $.expression),
      optional($.semicolon)
    )),

    resume_expression: $ => prec.right(seq(
      'resume',
      $.expression,
      optional($.semicolon)
    )),

    augmented_assignment_expression: $ => prec.right('assign', seq(
      field('left', choice($.parenthesized_expression, $._lhs_expression)),
      field('operator', choice('+=', '-=', '*=', '/=', '%=', '^=', '&=', '|=', '<<<', '>>>')),
      field('right', $.expression),
      optional($.semicolon)
    )),

    sequence_expression: $ => seq(
      field('left', $.expression),
      ',',
      field('right', choice($.sequence_expression, $.expression))
    ),
    
    switch_body: $ => seq(
      '{',
      repeat(choice($.switch_case, $.switch_default)),
      '}'
    ),

    switch_case: $ => seq(
      'case',
      field('value', $._expressions),
      ':',
      field('body', repeat($._statement))
    ),

    switch_default: $ => seq(
      'default',
      ':',
      field('body', repeat($._statement))
    ),

    else_clause: $ => seq('else', $._statement),

    initializer: $ => seq(
      '=',
      field('value', $.expression),
    ),

    unary_expression: $ => prec.left('unary_void', seq(
      field('operator', choice('!', '~', '-', '+', 'typeof', 'clone')),
      field('argument', $.expression)
    )),

    update_expression: $ => prec.left(2, seq(choice(
      seq(
          field('argument', $.deref_expression),
          field('operator', choice('++', '--'))
        ),
      seq(
          field('operator', choice('++', '--')),
          field('argument', $.deref_expression)
        ),
      ), 
      optional($.semicolon)
    )),

    binary_expression: $ => choice(
      ...[
        ['&&', 'logical_and'],
        ['||', 'logical_or'],
        ['>>', 'binary_shift'],
        ['>>>', 'binary_shift'],
        ['<<', 'binary_shift'],
        ['&', 'bitwise_and'],
        ['^', 'bitwise_xor'],
        ['|', 'bitwise_or'],
        ['+', 'binary_plus'],
        ['-', 'binary_plus'],
        ['*', 'binary_times'],
        ['/', 'binary_times'],
        ['%', 'binary_times'],
        ['<', 'binary_relation'],
        ['<=', 'binary_relation'],
        ['==', 'binary_equality'],
        ['!=', 'binary_equality'],
        ['>=', 'binary_relation'],
        ['>', 'binary_relation'],
        ['instanceof', 'binary_relation'],
        ['in', 'binary_relation'],
        ['<-', 'binary_newslot']
      ].map(([operator, precedence]) =>
        prec.left(precedence, seq(
          field('left', $.expression),
          field('operator', operator),
          field('right', $.expression)
        ))
      )
    ),

    ternary_expression: $ => prec.right('ternary', seq(
      field('condition', $.expression),
      '?',
      field('consequence', $.expression),
      ':',
      field('alternative', $.expression),
      optional($.semicolon)
    )),

    delegate_expression: $ => prec.right(seq(
      'delegate',
      field('parent_expression', $.expression),
      ':',
      field('value', $.expression)
    )),

    // Declarations

    semicolon: $ => ';',

    local_declaration: $ => prec.right(seq(
      'local',
      commaSep1($.variable_declarator),
      optional($.semicolon)
    )),
    
    const_declaration: $ => prec.right(seq(
      'const',
      $.const_expression
    )),

    enum_declaration: $ => prec.right(seq(
      'enum',
      field('name', $.identifier),
      field('body', $.enum_statement_block)
    )),

    variable_declarator: $ => seq(
      field('name', $.identifier),
      optional($.initializer)
    ),

    number: $ => {
      const hex_literal = seq(
        choice('0x', '0X'),
        /[\da-fA-F](_?[\da-fA-F])*/
      )

      const decimal_digits = /\d(_?\d)*/
      const signed_integer = seq(optional(choice('-', '+')), decimal_digits)
      const exponent_part = seq(choice('e', 'E'), signed_integer)

      const binary_literal = seq(choice('0b', '0B'), /[0-1](_?[0-1])*/)

      const octal_literal = seq(choice('0o', '0O'), /[0-7](_?[0-7])*/)

      const decimal_integer_literal = choice(
        '0',
        seq(optional('0'), /[1-9]/, optional(seq(optional('_'), decimal_digits)))
      )

      const decimal_literal = choice(
        seq(decimal_integer_literal, '.', optional(decimal_digits), optional(exponent_part)),
        seq('.', decimal_digits, optional(exponent_part)),
        seq(decimal_integer_literal, exponent_part),
        seq(decimal_digits),
      )

      return token(choice(
        hex_literal,
        decimal_literal,
        binary_literal,
        octal_literal
      ))
    },

    string: $ => seq(
      optional('@'),
      choice(
        seq(
          '"',
          repeat(choice(
            alias($.unescaped_double_string_fragment, $.string_fragment),
            $.escape_sequence
          )),
          '"'
        ),
        seq(
          "'",
          choice(
            $.unescaped_single_string_fragment,
            $.escape_sequence
          ),
          "'"
        )
      )
    ),

    array: $ => seq(
      '[',
      commaSep($.expression),
      optional(','),
      ']'
    ),

    const_expression: $ => seq(
      field('id', $.identifier),
      '=',
      field('value', choice(
        $.number,
        $.string
      ))
    ),

    function_expression: $ => prec('literal', seq(
      'function',
      field('name', optional($.namespaced_identifier)),
      $._call_signature,
      field('body', $.statement_block)
    )),

    _call_signature:   $ => field('parameters', $.formal_parameters),
    _formal_parameter: $ => choice($.identifier, $.assignment_pattern),

    formal_parameters: $ => seq(
      '(',
      optional(seq(
        commaSep1($._formal_parameter),
        optional(',')
      )),
      ')'
    ),

    statement_block: $ => prec.right(seq(
      '{',
      repeat($._statement),
      '}',
      optional($.semicolon)
    )),
    
    enum_statement_block: $ => prec.right(seq(
      '{',
      commaSep($.const_expression),
      '}',
      optional($.semicolon)
    )),

    variadic_expression: $ => '...',

    assignment_pattern: $ => seq(
      field('left', $.identifier),
      '=',
      field('right', $.expression)
    ),

    pattern: $ => prec.dynamic(-1, choice(
      $._lhs_expression
    )),

    class_expression: $ => prec('literal', prec.right(seq(
      'class',
      field('name', optional(choice($.identifier, $.member_expression))),
      optional($.class_heritage),
      field('body', $.class_body),
      optional($.semicolon)
    ))),

    class_heritage: $ => seq('extends', field('parent', $.deref_expression)),

    class_body: $ => seq(
      '{',
      repeat(choice(
        seq(
            field('member', $.method_definition), 
            optional($.semicolon)
          ),
        seq(
            field('member', $.field_definition), 
            optional($.semicolon)
          )
      )),
      '}'
    ),

    field_definition: $ => seq(
      optional('static'),
      field('property', $.identifier),
      optional($.initializer)
    ),
    
    method_definition: $ => seq(
      optional('static'),
        choice(
            seq('function',
                field('name', $.namespaced_identifier)
              ),
            'constructor'
          ),
      field('parameters', $.formal_parameters),
      field('body', $.statement_block)
    ),
    
    namespace_expression: $ => prec.right(3, seq(
      optional(field('root', $.identifier)),
      '::',
      optional(repeat(seq(
        $.identifier,
        '::'
      ))),
      field('identifier', $.identifier)
    )),

    call_expression: $ => prec.left('call', seq(
        field('function',  $.deref_expression),
        field('arguments', $.arguments),
        optional($.semicolon)
      )
    ),

    arguments: $ => seq(
      '(',
      commaSep(optional($.expression)),
      ')'
    ),

    unescaped_double_string_fragment: $ => token.immediate(prec(1, /[^"\\]+/)),

    unescaped_single_string_fragment: $ => token.immediate(prec(1, /[^'\\]/)),

    escape_sequence: $ => token.immediate(seq(
      '\\',
      choice(
        /[^xu0-7]/,
        /[0-7]{1,3}/,
        /x[0-9a-fA-F]{2}/,
        /u[0-9a-fA-F]{4}/,
        /u{[0-9a-fA-F]+}/
      )
    )),

    comment: $ => token(choice(
      seq('//', /.*/),
      seq(
        '/*',
        /[^*]*\*+([^/*][^*]*\*+)*/,
        '/'
      )
    )),
  }
});

function commaSep1(rule) {
  return seq(rule, repeat(seq(',', rule)));
}

function commaSep(rule) {
  return optional(commaSep1(rule));
}