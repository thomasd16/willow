using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    abstract class ExpressionNode : AstNode
    {
        public virtual int Precedence { get { return TokenHelper.Precedence(Token); } }
        public virtual bool IsLeftHand { get { return false; } }
        public abstract ExpressionNodeType Type {get;}
    }
}
