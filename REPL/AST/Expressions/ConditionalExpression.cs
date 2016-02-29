using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class ConditionalExpression : ExpressionNode
    {
        public override ExpressionNodeType Type
        {
            get { return ExpressionNodeType.CONDITIONAL; }
        }
        public override void Visit(INodeVisitor Visitor)
        {
            Condition = Visitor.Visit <ExpressionNode>(Condition);
            TrueResult = Visitor.Visit<ExpressionNode>(TrueResult);
            FalseResult = Visitor.Visit<ExpressionNode>(FalseResult);
        }
        public ExpressionNode Condition;
        public ExpressionNode TrueResult;
        public ExpressionNode FalseResult;
        public override void Output(CodeWriter s, int bp)
        {
            if (Precedence <= bp)
                s.Write('(');
            Condition.Output(s,Precedence);
            s.Write("?");
            TrueResult.Output(s,Precedence - 1);
            s.Write(":");
            FalseResult.Output(s, Precedence - 1);
            if (Precedence <= bp)
                s.Write(')');
        }
    }
}
