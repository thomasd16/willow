using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    enum StatementType
    {
        BLOCK,
        CONTROL,
        EXPRESSION,
        FUNCTION,//whill not appear in compiletime tree
        IF,
        VAR,//will not appear in compiletime tree
        VOID,//will not appear in compiletime tree
        WHILE
    }
}
