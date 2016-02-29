using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    class FunctionBlock : Block
    {
        public override void Output(CodeWriter s, int bp) {
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
