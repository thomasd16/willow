using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.AST {

    class NodeVisitor {
        
        class Replacer<T> : INodeVisitor where T : AstNode {
            Func<T, AstNode> Function;
            public Replacer(AstNode n, Func<T, AstNode> fn) {
                Function = fn;
                Visit<AstNode>(n);
            }
            public E Visit<E>(AstNode N) where E : AstNode {
                N.Visit(this);
                if (N is T)
                    return (E)Function((T)N);
                return (E)N;
            }
        }

        class Visitor<T> : INodeVisitor where T : AstNode {
            Action<T> Function;
            public Visitor(AstNode n, Action<T> fn) {
                Function = fn;
                Visit<AstNode>(n);
            }
            public E Visit<E>(AstNode N) where E : AstNode {
                N.Visit(this);
                if (N is T) Function((T)N);
                return (E)N;
            }
        }

        AstNode WorkingNode;
        public NodeVisitor(AstNode n) {
            WorkingNode = n;
        }
        public void VisitNodes<T>(Action<T> Fn) where T: AstNode {
            new Visitor<T>(WorkingNode,Fn);
        }
        public void ReplaceNodes<T>(Func<T, AstNode> Fn) where T : AstNode {
            new Replacer<T>(WorkingNode, Fn);
        }
    }
}
