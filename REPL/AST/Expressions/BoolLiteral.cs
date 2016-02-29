using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class BoolLiteral:ExpressionNode
    {
        public bool Value;
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.BOOL_LITERAL; }
        }
        public override void Output(CodeWriter s, int bp)
        {
            s.Write(Value?"true":"false");
        }
    }
}
