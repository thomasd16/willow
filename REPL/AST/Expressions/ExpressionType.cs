using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    enum ExpressionNodeType
    {
        ARRAY_LITERAL,
        ARROW_FUNCTION,
        ASSIGNMENT,
        CONDITIONAL,
        ARRAY_LOOKUP,
        INFIX,
        INVOCATION,
        NEW,
        NUMBER_LITERAL,
        OBJECT_LITERAL,
        OBJECT_LOOKUP,
        STRING_LITERAL,
        UNARY,
        THIS,
        NULL_LITERAL,
        VARIABLE,
        FUNCTION,
        BOOL_LITERAL
    }
}
