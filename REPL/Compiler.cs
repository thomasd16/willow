using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;
using REPL.Types;
using REPL.AST;
namespace REPL
{
    class Compiler
    {

        private bool IsScript;
        private Scope FnScope;

        public static W_Script CompileScript(Function fn)
        {
            Compiler c = new Compiler();
            c.IsScript = true;
            c.FnScope = fn.FunctionScope;
            c.PushBlock();
            c.PrepareFunctions();
            c.CompileBlock(fn.Body);
            Expression block = c.FinalizeBlock(c.FnScope.VariableReferences);
            return Expression.Lambda<W_Script>(block, c.FnScope.ThisReference).Compile();
        }

        public static Expression<W_Delegate> CompileFunction(Function fn)
        {
            Compiler c = new Compiler();
            c.IsScript = false;
            c.FnScope = fn.FunctionScope;
            c.PushBlock();
            c.PrepareFunctions();
            c.PrepareVariables();
            c.CompileBlock(fn.Body);
            c.AddExpression(Expression.Label(c.ReturnLabel, Expression.Constant(W_Type.NULL)));
            Expression block = c.FinalizeBlock(c.FnScope.VariableReferences);
            return Expression.Lambda<W_Delegate>(block, c.FnScope.ThisReference, c.FnScope.ArgsReference);
        }

        #region FunctionInfo
       
        #endregion

        #region State
        private LabelTarget ReturnLabel = Expression.Label(typeof(W_Type));
        private Stack<List<Expression>> Blocks = new Stack<List<Expression>>();
        private List<Expression> CurrentBlock { get { return Blocks.Peek(); } }
        private void PushBlock() { Blocks.Push(new List<Expression>()); }
        private List<Expression> PopBlock() { return Blocks.Pop(); }
        private void AddExpression(Expression e) { CurrentBlock.Add(e); }
        private Stack<LabelTarget> BreakTargets = new Stack<LabelTarget>();
        private LabelTarget CurrentBreakTarget { get { return BreakTargets.Peek(); } }
        private Stack<LabelTarget> ContinueTargets = new Stack<LabelTarget>();
        private LabelTarget CurrentContinueTarget { get { return ContinueTargets.Peek(); } }
        private Expression FinalizeBlock() { return Expression.Block(PopBlock()); }
        private Expression FinalizeBlock(IEnumerable<ParameterExpression> Scope) { return Scope.Count() == 0 ? FinalizeBlock() : Expression.Block(Scope, PopBlock()); }
        #endregion

        #region TypeManagement

        private enum ValueTypes
        {
            BOXED,
            STRING,
            DOUBLE,
            DELEGATE,
            OBJECT,
            BOOL,
            VOID
        }

        private Expression UnboxExpression(Expression BaseExpression, ValueTypes type)
        {
            MethodInfo Acessor = null;
            switch (type)
            {
                case ValueTypes.BOOL: Acessor = ReflectionConstants.MethodBoxToBool; break;
                case ValueTypes.DELEGATE: Acessor = ReflectionConstants.MethodBoxToFunction; break;
                case ValueTypes.DOUBLE: Acessor = ReflectionConstants.MethodBoxToNumber; break;
                case ValueTypes.OBJECT: Acessor = ReflectionConstants.MethodBoxToObject; break;
                case ValueTypes.STRING: Acessor = ReflectionConstants.MethodBoxToString; break;
            }
            return Expression.Call(BaseExpression, Acessor);
        }

        private Expression BoxExpression(Expression BaseExpression)
        {
            ValueTypes type = GetExpressionType(BaseExpression);
            if (type == ValueTypes.BOXED)
                return BaseExpression;
            if (type == ValueTypes.OBJECT) //Downcase Essential .NET will not implicitly downcase on conditionals
                return Expression.Convert(BaseExpression, typeof(W_Type));

            ConstructorInfo constructor = null;
            switch (type)
            {
                case ValueTypes.BOOL: constructor = ReflectionConstants.BoolConstructor; break;
                case ValueTypes.DELEGATE: constructor = ReflectionConstants.FunctionConstructor; break;
                case ValueTypes.DOUBLE: constructor = ReflectionConstants.NumberConstructor; break;
                case ValueTypes.STRING: constructor = ReflectionConstants.StringConstructor; break;
            }
            return Expression.New(constructor, BaseExpression);
        }

        private Expression SwitchExpressionType(Expression BaseExpression, ValueTypes outType)
        {
            ValueTypes inType = GetExpressionType(BaseExpression);
            if (inType == outType)
                return BaseExpression;
            if (inType == ValueTypes.BOXED)
                return UnboxExpression(BaseExpression, outType);
            if (outType == ValueTypes.BOXED)
                return BoxExpression(BaseExpression);
            return UnboxExpression(BoxExpression(BaseExpression), outType);
        }

        private ValueTypes GetExpressionType(Expression exp)
        {
            Type type = exp.Type;
            if (type == typeof(W_Object))
                return ValueTypes.OBJECT;
            if (type == typeof(W_Type)
                || type == typeof(W_Null))
                return ValueTypes.BOXED;
            if (type == typeof(bool))
                return ValueTypes.BOOL;
            if (type == typeof(W_Delegate))
                return ValueTypes.DELEGATE;
            if (type == typeof(double))
                return ValueTypes.DOUBLE;
            if (type == typeof(string))
                return ValueTypes.STRING;
            throw new Exception("Encounterd an impossible return type");
        }
        #endregion

        #region Expressions

        #region Assignment

        private Expression CompileAssignmentInfix(TokenType token, Expression l, Expression r)
        {
            switch (token)
            {
                case TokenType.ASSIGN_ADD:
                    return Expression.Add(l, r);
                case TokenType.ASSIGN_DIV:
                    return Expression.Divide(l, r);
                case TokenType.ASSIGN_MOD:
                    return Expression.Modulo(l, r);
                case TokenType.ASSIGN_MUL:
                    return Expression.Modulo(l, r);
                case TokenType.ASSIGN_SUB:
                    return Expression.Subtract(l, r);
                default:
                    throw new Exception("Unexpected assignment token this shouldn't happen.");
            }
        }

        private Expression CompileVariableAssignment(TokenType token, VariableExpression var, ExpressionNode value)
        {
            if (token == TokenType.ASSIGN)
                return Expression.Assign(var.ScopeReference.Reference, CompileExpression(value, ValueTypes.BOXED));
            Expression Right = CompileExpression(value, ValueTypes.DOUBLE);
            Expression Left = UnboxExpression(var.ScopeReference.Reference, ValueTypes.DOUBLE);
            return Expression.Assign(var.ScopeReference.Reference, BoxExpression(CompileAssignmentInfix(token, Left, Right)));
        }

        private Expression CompileObjectLookupAssignment(TokenType token, ObjectLookupExpression var, ExpressionNode value)
        {
            ParameterExpression valueReference = Expression.Parameter(typeof(W_Type));
            
            if (token == TokenType.ASSIGN)
            {
                /* Expression is of the following form
                 * {
                 *     W_Type val = Right;
                 *     Left.Object[key] = val;
                 *     return val;
                 * }
                 */
                return Expression.Block(typeof(W_Type), new ParameterExpression[] { valueReference },
                    Expression.Assign(valueReference, CompileExpression(value, ValueTypes.BOXED)),
                    Expression.Call(CompileExpression(var.Object, ValueTypes.OBJECT), ReflectionConstants.MethodSetObjProp, Expression.Constant(var.Lookup), valueReference),
                    valueReference
                );
            }


            ParameterExpression objectReference = Expression.Parameter(typeof(W_Object));

            Expression Right = CompileExpression(value, ValueTypes.DOUBLE);
            
            //Expression of the following form
            //object["key"].Number
            Expression Left = UnboxExpression(
                    Expression.Call(
                        objectReference,
                        ReflectionConstants.MethodGetObjProp,
                        Expression.Constant(var.Lookup)
                   ),
               ValueTypes.DOUBLE
            );
            /* Expression is of the following form
             * {
             *     W_Object obj = Left.Object;
             *     W_Type val = obj[key].Number (op) Right.Number;
             *     obj[key] = val;
             *     return val;
             *  }
             */     
            Expression retval =  Expression.Block(
                typeof(W_Type),
                new ParameterExpression[] { objectReference, valueReference },
                Expression.Assign(
                    objectReference,
                    CompileExpression(var.Object, ValueTypes.OBJECT)
                ),
                Expression.Assign(
                    valueReference,
                    BoxExpression(CompileAssignmentInfix(token, Left, Right))
                ),
                Expression.Call(
                    objectReference,
                    ReflectionConstants.MethodSetObjProp,
                    Expression.Constant(var.Lookup),
                    valueReference
                ),
                valueReference
            );
            Console.WriteLine(retval.Type);
            return retval;
        }

        private Expression CompileAssignmentExpression(AssignmentExpression node)
        {
            switch (node.Left.Type)
            {
                case ExpressionNodeType.VARIABLE:
                    return CompileVariableAssignment(node.Token, (VariableExpression)node.Left, node.Right);
                case ExpressionNodeType.OBJECT_LOOKUP:
                    return CompileObjectLookupAssignment(node.Token, (ObjectLookupExpression)node.Left, node.Right);
            }
            throw new Exception("Invalid lefthand assignment at compiletime this shouldn't happen");
        }

        #endregion

        private Expression CompileInfixExpression(InfixExpression node)
        {
            ValueTypes NeededType;
            switch (node.Token)
            {

                case TokenType.EQ:
                case TokenType.NE:
                    NeededType = ValueTypes.BOXED;
                    break;
                case TokenType.COMMA:
                    NeededType = ValueTypes.VOID;
                    break;

                case TokenType.OR:
                case TokenType.AND:
                    NeededType = ValueTypes.BOOL;
                    break;

                case TokenType.ADD:
                case TokenType.SUB:
                case TokenType.MUL:
                case TokenType.DIV:
                case TokenType.MOD:
                case TokenType.LT:
                case TokenType.GT:
                case TokenType.LTE:
                case TokenType.GTE:
                    NeededType = ValueTypes.DOUBLE;
                    break;
                default:
                    throw new Exception(String.Format("Encounterd an impossible infix operator {0}", node.Token));
            }
            Expression Left = CompileExpression(node.Left, NeededType);
            Expression Right = CompileExpression(node.Right, NeededType);
            switch (node.Token)
            {
                case TokenType.COMMA:
                    return Expression.Block(Left, Right);
                case TokenType.OR:
                    return Expression.OrElse(Left, Right);
                case TokenType.AND:
                    return Expression.AndAlso(Left, Right);
                case TokenType.ADD:
                    return Expression.Add(Left, Right);
                case TokenType.SUB:
                    return Expression.Subtract(Left, Right);
                case TokenType.DIV:
                    return Expression.Divide(Left, Right);
                case TokenType.MUL:
                    return Expression.Multiply(Left, Right);
                case TokenType.MOD:
                    return Expression.Modulo(Left, Right);
                case TokenType.EQ:
                    return Expression.Equal(Left, Right);
                case TokenType.NE:
                    return Expression.NotEqual(Left, Right);
                case TokenType.LT:
                    return Expression.LessThan(Left, Right);
                case TokenType.GT:
                    return Expression.GreaterThan(Left, Right);
                case TokenType.LTE:
                    return Expression.LessThanOrEqual(Left, Right);
                case TokenType.GTE:
                    return Expression.GreaterThanOrEqual(Left, Right);
                default:
                    throw new Exception(String.Format("Encounterd an impossible infix operator {0}", node.Token));
            }
        }

        private Expression CompileBoundInvocationExpression(ExpressionNode source, string key, Expression[] arguments)
        {
            ParameterExpression objectReference = Expression.Parameter(typeof(W_Object));
            /* expression is of the following form 
             * {
             *     W_Object source = source;
             *     return source[key].Function(source,new W_Type(arguments));
             * }
             */
            return Expression.Block(typeof(W_Type), new ParameterExpression[]{objectReference},
                Expression.Assign(objectReference,CompileExpression(source,ValueTypes.OBJECT)),
                Expression.Invoke(
                    SwitchExpressionType(Expression.Call(objectReference, ReflectionConstants.MethodGetObjProp, Expression.Constant(key)), ValueTypes.DELEGATE),
                    objectReference,
                    Expression.NewArrayInit(typeof(W_Type),arguments)
                )
            );
        }
        private Expression CompileInvocationExpression(InvocationExpressionNode node)
        {
            Expression[] Arguments = new Expression[node.Arguments.Length];
            for (int i = 0; i < Arguments.Length; i++)
                Arguments[i] = CompileExpression(node.Arguments[i], ValueTypes.BOXED);

            if (node.Callee.Type == ExpressionNodeType.OBJECT_LOOKUP)
                return CompileBoundInvocationExpression((node.Callee as ObjectLookupExpression).Object, (node.Callee as ObjectLookupExpression).Lookup, Arguments);

            Expression left = CompileExpression(node.Callee, ValueTypes.DELEGATE);
            return Expression.Invoke(left, Expression.Constant(null, typeof(W_Object)), Expression.NewArrayInit(typeof(W_Type), Arguments));
        }

        private Expression CompileLookupExpression(ObjectLookupExpression node)
        {
            Expression Object = CompileExpression(node.Object, ValueTypes.OBJECT);
            return Expression.Call(Object, ReflectionConstants.MethodGetObjProp, Expression.Constant(node.Lookup));
        }

        private Expression ParseUnaryExpression(UnaryExpressionNode node)
        {
            if (node.Token == TokenType.NOT)
                return Expression.Not(CompileExpression(node.Value, ValueTypes.BOOL));
            throw new Exception("hello");
        }

        private Expression CompileExpression(ExpressionNode node, ValueTypes type)
        {
            Expression ret = null;
            switch (node.Type)
            {
                case ExpressionNodeType.ASSIGNMENT:
                    ret = CompileAssignmentExpression((AssignmentExpression)node);
                    break;
                case ExpressionNodeType.FUNCTION:
                    ret = Compiler.CompileFunction((Function)node);
                    break;
                case ExpressionNodeType.INFIX:
                    ret = CompileInfixExpression((InfixExpression)node);
                    break;
                case ExpressionNodeType.INVOCATION:
                    ret = CompileInvocationExpression((InvocationExpressionNode)node);
                    break;
                case ExpressionNodeType.OBJECT_LOOKUP:
                    ret = CompileLookupExpression((ObjectLookupExpression)node);
                    break;
                //case ExpressionNodeType.ARRAY_LOOKUP:
                //    ret = CompileDynamicLookupExpression((ArrayLookupExpression)node);
                //    break;
                case ExpressionNodeType.VARIABLE:
                    ret = (node as VariableExpression).ScopeReference.Reference;
                    break;
                case ExpressionNodeType.THIS:
                    ret = FnScope.ThisReference;
                    break;
                case ExpressionNodeType.NUMBER_LITERAL:
                    ret = Expression.Constant((node as NumberLiteralNode).Value);
                    break;
                case ExpressionNodeType.STRING_LITERAL:
                    ret = Expression.Constant((node as StringLiteral).Value);
                    break;
                case ExpressionNodeType.NULL_LITERAL:
                    //must be of type W_Type .NET doesn't implicitly downcase
                    ret = Expression.Constant(W_Type.NULL,typeof(W_Type));
                    break;
                case ExpressionNodeType.BOOL_LITERAL:
                    ret = Expression.Constant((node as BoolLiteral).Value);
                    break;
                case ExpressionNodeType.UNARY:
                    ret = ParseUnaryExpression(node as UnaryExpressionNode);
                    break;
                default:
                    throw new Exception(String.Format("Expressions of type {0} no supported yet", node.Type));
            }
            if (type == ValueTypes.VOID)
                return ret;
            return SwitchExpressionType(ret, type);
        }

        #endregion

        #region Statements

        public Expression CompileIfStatement(IfStatement stmnt)
        {
            Expression res;
            Expression condition = CompileExpression(stmnt.Condition, ValueTypes.BOOL);
            PushBlock();
            CompileBlock(stmnt.True);
            Expression TrueResult = FinalizeBlock();
            if (stmnt.False != null)
            {
                Expression FalseResult;
                PushBlock();
                CompileBlock(stmnt.False);
                FalseResult = FinalizeBlock();
                res = Expression.IfThenElse(condition, TrueResult, FalseResult);
            }
            else
            {
                res = Expression.IfThen(condition, TrueResult);
            }
            return res;
        }

        public Expression CompileControlStatement(ControllStatement stmnt)
        {
            switch (stmnt.Token)
            {
                case TokenType.RETURN:
                    return Expression.Return(ReturnLabel, stmnt.Value != null ? CompileExpression(stmnt.Value, ValueTypes.BOXED) : Expression.Constant(W_Type.NULL));
                default:
                    throw new Exception(String.Format("controll statement {0} not supported", stmnt.Token));
            }
        }

        public Expression CompileLoop(WhileStatement stmnt)
        {
            PushBlock();
            BreakTargets.Push(Expression.Label());
            ContinueTargets.Push(Expression.Label());
            AddExpression(Expression.IfThen(Expression.Not(CompileExpression(stmnt.Condition, ValueTypes.BOOL)), Expression.Return(CurrentBreakTarget)));
            CompileBlock(stmnt.Loop);
            return Expression.Loop(FinalizeBlock(), BreakTargets.Pop(), ContinueTargets.Pop());

        }

        public void CompileBlock(Block source)
        {
            foreach (Statement stmnt in source.Statements)
            {
                switch (stmnt.Type)
                {
                    case StatementType.IF:
                        AddExpression(CompileIfStatement(stmnt as IfStatement));
                        break;
                    case StatementType.EXPRESSION:
                        AddExpression(CompileExpression((stmnt as ExpressionStatement).Expression, ValueTypes.VOID));
                        break;
                    case StatementType.CONTROL:
                        AddExpression(CompileControlStatement(stmnt as ControllStatement));
                        break;
                    case StatementType.WHILE:
                        AddExpression(CompileLoop(stmnt as WhileStatement));
                        break;
                }
            }
        }

        #endregion

        #region scopeInitialization
        public void PrepareFunctions()
        {

            foreach (DataPair<Variable, Function> function in FnScope.Functions)
                AddExpression(Expression.Assign(function.Key.Reference, CompileExpression(function.Value, ValueTypes.BOXED)));
        }

        public void PrepareVariables()
        {
            AddExpression(
                Expression.IfThen(
                    Expression.Equal(
                        FnScope.ThisReference,
                        Expression.Constant(null)
                    ),
                    Expression.Assign(
                        FnScope.ThisReference,
                        FnScope.ParrentScope.ThisReference
                    )
                )
            );
            for (int i = 0; i < FnScope.Arguments.Count; i++)
            {
                AddExpression(
                    Expression.Assign(
                        FnScope.Arguments[i].Reference,
                        Expression.Condition(Expression.GreaterThan(
                                Expression.ArrayLength(FnScope.ArgsReference),
                                Expression.Constant(i)
                             ),
                            Expression.ArrayIndex(FnScope.ArgsReference, Expression.Constant(i)),
                            Expression.Constant(W_Type.NULL, typeof(W_Type))
                        )
                    )
                );
            }
        }
        #endregion
    }
}
