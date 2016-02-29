using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using REPL.AST;
using REPL.Types;

namespace REPL
{
    class Variable
    {
        public Variable(string name) {
            Name = name;
            Reference = Expression.Parameter(typeof(W_Type), name);
        }
        public ParameterExpression Reference;
        public string Name;
    }
    class Scope
    {
        public List<Variable> Arguments = new List<Variable>();
        public List<DataPair<Variable, Function>> Functions = new List<DataPair<Variable, Function>>();
        public List<Variable> Variables = new List<Variable>();
        public Scope ParrentScope;
        public ParameterExpression ThisReference = Expression.Parameter(typeof(W_Object));
        public ParameterExpression ArgsReference = Expression.Parameter(typeof(W_Type[]));

        public IEnumerable<string> Names {
            get {
                foreach (Variable x in FullScope)
                    yield return x.Name;
            }
        }

        public IEnumerable<Function> FunctionObjects {
            get {
                foreach (DataPair<Variable, Function> x in Functions)
                    yield return x.Value;
            }
        }

        public IEnumerable<ParameterExpression> VariableReferences {
            get {
                foreach (Variable v in Variables)
                    yield return v.Reference;
                foreach (DataPair<Variable, Function> v in Functions)
                    yield return v.Key.Reference;
                foreach (Variable v in Arguments)
                    yield return v.Reference;
            }
        }

        private IEnumerable<Variable> FullScope {
            get {
                foreach (Variable x in Arguments)
                    yield return x;
                foreach (Variable x in Variables)
                    yield return x;
                foreach (DataPair<Variable, Function> x in Functions)
                    yield return x.Key;
            }
        }

        public Scope() { ParrentScope = null; }
        public Scope(Scope parrent, IEnumerable<string> arguments) {
            ParrentScope = parrent;
            foreach (string argument in arguments)
                Arguments.Add(new Variable(argument));
        }

        public Variable Lookup(string name) {
            foreach (Variable x in FullScope)
                if (x.Name == name)
                    return x;
            if (ParrentScope == null)
                return null;
            return ParrentScope.Lookup(name);
        }

        public Variable Define(string name) {
            if (Names.Contains(name))
                return null;
            Variable ret = new Variable(name);
            Variables.Add(ret);
            return ret;
        }

        public Variable DefineFunction(string name, Function fn) {
            if (Names.Contains(name))
                return null;
            Variable ret = new Variable(name);
            Functions.Add(new DataPair<Variable, Function>(ret, fn));
            return ret;
        }
    }
}
