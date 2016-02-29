using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST
{
    interface INodeVisitor
    {
         E Visit<E>(AstNode Node) where E: AstNode;
    }
}
