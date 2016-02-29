using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST {
    class InlineBlock : Block {
        public override void Output(CodeWriter s, int bp) {

            foreach (Statement Stmt in Statements) {
                s.NewLine();
                Stmt.Output(s, 0);
            }
        }
    }
}
