using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class VariableExpression: ExpressionNode
    {
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.VARIABLE; }
        }
        public Variable ScopeReference;
        public override bool IsLeftHand { get { return true; } }
        public string Value;
        public override void Output(CodeWriter s, int bp) {
            s.Write(Value??ScopeReference.Name);
        } 
    }
}
