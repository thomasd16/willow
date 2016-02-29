using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
     class Block : Statement
    {
        public override StatementType Type {get { return StatementType.BLOCK; }}
        public List<Statement> Statements = new List<Statement>();
        public override void Visit(INodeVisitor Visitor)
        {
            for (int i = 0; i < Statements.Count; i++)
            {
                Statements[i] = Visitor.Visit<Statement>(Statements[i]);
            }
        }
        public override void Output(CodeWriter s, int bp) {

            if (Statements.Count() == 0) {
                s.Write("{}");
                s.NewLine();
                return;
            }
            s.Write('{');
            s.Indent();
            foreach (Statement Stmt in Statements) {
                s.NewLine();
                Stmt.Output(s, 0);
            }
            s.Outdent();
            s.NewLine();
            s.Write('}');
        }
    }
}
