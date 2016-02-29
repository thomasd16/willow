using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class VarStatement : Statement
    {
        public override StatementType Type { get { return StatementType.VAR; } }
        public List<DataPair<string,ExpressionNode>> Vars = new List<DataPair<string,ExpressionNode>>();
        public override void Visit(INodeVisitor Visitor)
        {
            for (int i = 0; i < Vars.Count; i++)
            {
                if (Vars[i].Value != null)
                    Vars[i].Value = Visitor.Visit<ExpressionNode>(Vars[i].Value);
            }
        }

        public override void Output(REPL.AST.CodeWriter s, int bp) {
            s.Write("var ");
            bool first = true;
            foreach (DataPair<string,ExpressionNode> v in Vars) {
                if (first) {
                    first = false;
                }
                else {
                    s.Write(", ");
                }
                s.Write(v.Key);
                if (v.Value != null) {
                    s.Write(" = ");
                    v.Value.Output(s, 1);
                }
            }
            s.Write(';');
        }

    }
}
