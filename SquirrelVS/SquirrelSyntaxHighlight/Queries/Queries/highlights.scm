; Special identifiers
;--------------------

(
(call_expression 
    function: (deref_expression) @function.builtin)
    (#match? @function.builtin "^(::)?(array|seterrorhandler|setdebughook|enabledebuginfo|getroottable|setroottable|assert|print|compilestring|collectgarbage|type|getstackinfos|newthread)$")
)

(
(call_expression
    function: (deref_expression) @function.builtin)
    (#match? @function.builtin "^(tofloat|tostring|tointeger|tochar|weakreaf|slice|find|tolower|toupper|len|rawget|rawset|rawdelete|rawin|clear|append|push|extend|pop|top|insert|remove|resize|sort|reverse|call|pcall|acall|pacall|bindenv|instance|getattribute|getattributes|getclass|getstatus|wakeup|getstatus|ref)$")
)

(
  (identifier) @variable.builtin
  (#match? @variable.builtin "^(_version_|_charsize_|_intsize_|_floatsize)$")
)

; Function and method definitions
;--------------------------------

(function_expression
  name: (namespaced_identifier) @function)

(method_definition
  name: (namespaced_identifier) @function.method)

(method_definition
  constructor: "constructor" @function.method)

; Function and method calls
;--------------------------

(call_expression
  function: (identifier) @function)

(call_expression
  function: (member_expression
    property: (property_identifier) @function.method))

; Literals
;---------

(this) @variable.builtin

[
  (true)
  (false)
  (null)
] @constant.builtin

[
  (vargc)
  (vargv)
] @varags

(comment) @comment

(string) @string

(number) @number

; Tokens
;-------

[
  "."
  ","
] @punctuation.delimiter

[
  "-"
  "--"
  "+"
  "++"
  "*"
  "/"
  "/="
  "%"
  "<"
  "<<"
  "="
  "=="
  "!"
  "!="
  ">"
  ">="
  ">>"
  ">>>"
  "~"
  "^"
  "&"
  "|"
  "&&"
  "||"
] @operator

[
  "("
  ")"
  "["
  "]"
  "{"
  "}"
  "<"
  ">"
] @punctuation.bracket

[
  "break"
  "case"
  "catch"
  "class"
  "clone"
  "continue"
  "const"
  "default"
  "delegate"
  "delete"
  "else"
  "enum"
  "extends"
  "for"
  "foreach"
  "function"
  "if"
  "in"
  "local"
  "resume"
  "return"
  "switch"
  "throw"
  "try"
  "typeof"
  "while"
  "yield"
  "instanceof"
  "static"
] @keyword

; Classes

(class_expression
  name: (identifier) @class)

(class_expression
  name: (member_expression) @class)

(class_heritage
  parent: (deref_expression) @class)