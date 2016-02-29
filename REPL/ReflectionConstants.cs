using System;
using REPL.Types;
using System.Reflection;

namespace REPL {
    internal class ReflectionConstants {
        public static readonly MethodInfo MethodTypeEquels = typeof(W_Type).GetMethod("Equal");
        public static readonly MethodInfo MethodBoxToString = typeof(W_Type).GetMethod("get_String");
        public static readonly MethodInfo MethodBoxToNumber = typeof(W_Type).GetMethod("get_Number");
        public static readonly MethodInfo MethodBoxToFunction = typeof(W_Type).GetMethod("get_Function");
        public static readonly MethodInfo MethodBoxToObject = typeof(W_Type).GetMethod("get_Object");
        public static readonly MethodInfo MethodBoxToBool = typeof(W_Type).GetMethod("get_Bool");
        public static readonly MethodInfo MethodGetObjProp = typeof(W_Object).GetMethod("get_Item");
        public static readonly MethodInfo MethodSetObjProp = typeof(W_Object).GetMethod("set_Item");
        public static readonly ConstructorInfo BoolConstructor = typeof(W_Bool).GetConstructor(new Type[] { typeof(bool) });
        public static readonly ConstructorInfo NumberConstructor = typeof(W_Number).GetConstructor(new Type[] { typeof(double) });
        public static readonly ConstructorInfo StringConstructor = typeof(W_String).GetConstructor(new Type[] { typeof(string) });
        public static readonly ConstructorInfo FunctionConstructor = typeof(W_Function).GetConstructor(new Type[] { typeof(W_Delegate) });
        public static readonly ConstructorInfo BlankObjectConstructor = typeof(W_Object).GetConstructor(new Type[] { });
        public static readonly ConstructorInfo ProtoObjectConstructor = typeof(W_Object).GetConstructor(new Type[] { typeof(W_Object) });
    }
}
