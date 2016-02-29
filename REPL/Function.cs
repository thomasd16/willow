using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REPL.AST;
namespace REPL
{
 
    class Function : ExpressionNode
    {
        public override ExpressionNodeType Type { get { return ExpressionNodeType.FUNCTION; } }

        enum FunctionType
        {
            ARROW,
            STATEMENT,
            SCRIPT
        }
        public Scope FunctionScope;
        public Block Body;
        public NodeVisitor Visitor;
        private FunctionType FnType;
        private string Name;

        public bool IsScript { get { return FnType == FunctionType.SCRIPT; } }

        public Function(Script body)
        {
            FnType = FunctionType.SCRIPT;
            Body = body;
            Visitor = new NodeVisitor(body);
            FunctionScope = new Scope();
        }

        public Function(FunctionStatement fn, Function parent)
        {
            FnType = FunctionType.STATEMENT;
            Name = fn.Name;
            Body = fn.Body;
            FunctionScope = new Scope(parent.FunctionScope, fn.Arguments);
            Visitor = new NodeVisitor(Body);
        }

        public Function(ArrowFunction fn, Function parent)
        {
            FnType = FunctionType.ARROW;
            FunctionScope = new Scope(parent.FunctionScope, fn.Arguments);
            Body = fn.Body;
            if (Body == null)
            {
                Body = new Block();
                Body.Statements.Add(new ControllStatement
                {
                    Token = TokenType.RETURN,
                    Value = fn.Expression
                });
            }
            Visitor = new NodeVisitor(Body);
        }

        private void RebuildBody()
        {
            Block newBody = new InlineBlock();
            foreach (Statement s in Body.Statements)
            {
                if (s is VoidStatement)
                    continue;
                if (s is InlineBlock)
                    foreach (Statement t in ((InlineBlock)s).Statements)
                        newBody.Statements.Add(t);
                else
                    newBody.Statements.Add(s);
            }
            Body = newBody;
        }

        public void Process()
        {
            Visitor.ReplaceNodes<FunctionStatement>(HoistFunction);
            Visitor.ReplaceNodes<ArrowFunction>(RefactorArrowFunction);
            Visitor.ReplaceNodes<VarStatement>(HoistVariables);
            Visitor.VisitNodes<VariableExpression>(ResolveScope);
            //Visitor.ReplaceNodes<StaticLookupExpression>(RefactorStaticLookups);
            RebuildBody();
            Visitor.VisitNodes<Function>((x) => x.Process());
            foreach (Function fn in FunctionScope.FunctionObjects)
                fn.Process();
        }

        public AstNode RefactorStaticLookups(ObjectLookupExpression exp)
        {
            return new ArrayLookupExpression
            {
                Object = exp.Object,
                Lookup = new StringLiteral
                {
                    Value = exp.Lookup
                }
            };
        }

        public void ResolveScope(VariableExpression var)
        {
            Variable ret = FunctionScope.Lookup(var.Value);
            if (ret == null)
                var.Error("Unresolved scope lookup");
            var.ScopeReference = ret;
        }
        public AstNode RefactorArrowFunction(ArrowFunction fn)
        {
            return new Function(fn, this);
        }

        public AstNode HoistVariables(VarStatement stmnt)
        {
            Block ret = new InlineBlock();
            foreach (DataPair<string, ExpressionNode> Var in stmnt.Vars)
            {
                Variable ScopeVar = FunctionScope.Define(Var.Key);
                if (ScopeVar == null)
                    stmnt.Error("Double declaration of variable");
                if (Var.Value != null)
                    ret.Statements.Add(new ExpressionStatement
                    {
                        Expression = new AssignmentExpression
                        {
                            Token = TokenType.ASSIGN,
                            Left = new VariableExpression
                            {
                                Value = Var.Key
                            },
                            Right = Var.Value
                        }
                    });
            }
            return ret;
        }

        public AstNode HoistFunction(FunctionStatement fn)
        {
            if (FunctionScope.DefineFunction(fn.Name, new Function(fn, this)) == null)
                fn.Error("Duble declaration of variable");
            return new VoidStatement();
        }

        private string OutputArgumentList()
        {
            return "(" + FunctionScope.Arguments.Select(x => x.Name).DefaultIfEmpty("").Aggregate((l, r) => l + "," + r) + ")";
        }

        private void OutputBody(CodeWriter s)
        {
            if (FnType != FunctionType.SCRIPT)
            {
                s.Write('{');
                s.Indent();
                if (FunctionScope.Variables.Count != 0)
                    s.NewLine();
            }
            if (FunctionScope.Variables.Count != 0)
                s.Write("var " + FunctionScope.Variables.Select(x => x.Name).DefaultIfEmpty("").Aggregate((l, r) => l + "," + r) + ";");
            foreach (Function fn in FunctionScope.FunctionObjects)
            {
                s.NewLine();
                fn.Output(s, 0);
            }
            Body.Output(s, 0);
            if (FnType != FunctionType.SCRIPT)
            {
                s.Outdent();
                s.NewLine();
                s.Write('}');
            }
        }
        public override void Output(CodeWriter s, int bp)
        {
            switch (FnType)
            {
                case FunctionType.STATEMENT:
                    s.Write("fn ");
                    s.Write(Name);
                    s.Write(OutputArgumentList());
                    OutputBody(s);
                    break;
                case FunctionType.ARROW:
                    if (bp > 1) s.Write('(');
                    s.Write(OutputArgumentList());
                    s.Write("=>");
                    OutputBody(s);
                    if (bp > 1) s.Write(')');
                    break;
                case FunctionType.SCRIPT:
                    OutputBody(s);
                    break;
            }
        }
    }
}
