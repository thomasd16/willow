using System;
using System.Collections.Generic;
using System.Linq;
using REPL;
using REPL.Types;
using System.Reflection;
using System.Linq.Expressions;

namespace REPL.Reflection {
    class ReflectedFunction : Attribute {
        public string Name;
        public ReflectedFunction(string name)
        {
            Name = name;
        }
        public ReflectedFunction()
        {
            Name = null;
        }
    }
    static class Reflection {
        private static Dictionary<Type, W_Object> Prototypes = new Dictionary<Type, W_Object>();

        static Expression GetType(Expression source, Type ret)
        {
            List<KeyValuePair<string, Expression>> checks = new List<KeyValuePair<string, Expression>>();
            Type wTypeNeeded =
                ret == typeof(int) ? typeof(W_Number) :
                ret == typeof(double) ? typeof(W_Number) :
                ret == typeof(string) ? typeof(W_String) :
                typeof(W_Briefcase<>).MakeGenericType(ret);
            checks.Add(new KeyValuePair<string,Expression>("Wrong Type passed to function",Expression.Not(Expression.TypeIs(source,wTypeNeeded))));
            
            
            if(ret == typeof(int)){
                /* expression of the following form
                 * source.Number%1 != 0
                 */
                checks.Add(new KeyValuePair<string,Expression>(
                    "number is not an int",
                    Expression.NotEqual(
                        Expression.Modulo(
                            Expression.Call(
                                source,
                                ReflectionConstants.MethodBoxToNumber
                            ),
                            Expression.Constant(1)
                        ),
                        Expression.Constant(0,typeof(double))
                    )
                ));
            }


        }

        static W_Function CreateFunction<T>(MethodInfo method)
        {
            Type source = typeof(T);
            Type[] Arguments = method.GetParameters().Select(x => x.ParameterType).ToArray();

        }

        static W_Object GetReflectedPrototype<T>()
        {
            Type source = typeof(T);
            if (Prototypes.ContainsKey(source))
                return Prototypes[source];
            W_Object prototype = new W_Object();
            foreach (MethodInfo method in source.GetMethods()) {
                if (Attribute.GetCustomAttribute(method, typeof(ReflectedFunction)) == null)
                    continue;
                string FunctionName = method.GetCustomAttributes<ReflectedFunction>().First().Name ?? method.Name;
                prototype[FunctionName] = CreateFunction<T>(method);
            }
            Prototypes[source] = prototype;
            return prototype;
        }
    }
}
