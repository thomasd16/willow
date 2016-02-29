using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class NewExpression : ExpressionNode
    {
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.NEW; }
        }
        public override void Visit(INodeVisitor Visitor)
        {
            Base = Visitor.Visit<ExpressionNode>(Base);
            if (Extend != null)
            {
                Extend = Visitor.Visit<ObjectLiteral>(Extend);
            }
        }
        public ExpressionNode Base;
        public ObjectLiteral Extend;
        public override void Output(CodeWriter s, int bp) {
            s.Write("new ");
            Base.Output(s, 13);
            if (Extend != null)
                Extend.Output(s, 0);
        }
    }
}
