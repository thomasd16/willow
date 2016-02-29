using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class ArrowFunction: ExpressionNode
    {
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.ARROW_FUNCTION; }
        }
        public List<String> Arguments = new List<string>();
        public FunctionBlock Body;
        public ExpressionNode Expression;
        public override void Output(CodeWriter s, int bp) {
            if (bp > 1) s.Write('(');
            if (Arguments.Count != 1) s.Write('(');
            if(Arguments.Count != 0)
                s.Write(Arguments.Aggregate((l, r) => l + ',' + r));
            if (Arguments.Count != 1) s.Write(')');
            s.Write("=>");
            if (Body != null) {
                Body.Output(s,0);
            }
            else {
                Expression.Output(s, Precedence);
            }
            if (bp > 1) s.Write(')');
        }
    }
}
