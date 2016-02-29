using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REPL.Types;
namespace REPL {
    static class CoreApi {

        public static W_Object StringPrototype = new W_Object();

        private static void StringFunction(string name, Func<string, W_Type[],W_Type> fn ) {
            StringPrototype[name] = new W_Function((thisArg,Arguments) => {
                W_String value = thisArg["value"] as W_String;
                if(value == null)
                    thisArg.Error("String object does not contain value");
                return
                    fn(value.String, Arguments);
            });
        }

        private static bool HasArg<T>(W_Type[] source, int index) where T : W_Type
        {
            if (index >= source.Length)
                return false;
            if (!(source[index] is T))
                return false;
            return true;
        }
        private static bool HasIntArg(W_Type[] source, int index)
        {
            if (index >= source.Length)
                return false;
            if (!(source[index] is W_Number))
                return false;
            if (Math.Floor(source[index].Number) != source[index].Number)
                return false;
            return true;
        }

        private static void RequierArg<T>(W_Type[] source, int index) where T : W_Type
        {
            if (index >= source.Length)
                throw new Exception("Missing arguments");
            if (!(source[index] is T))
                source[index].Error("Expected argument of type {0} instead saw {1}", W_Type.GetName<T>(), source[index].TypeName);
        }

        private static void RequireIntArg(W_Type[] source, int index)
        {
            if (index >= source.Length)
                throw new Exception("Missing arguments");
            if (!(source[index] is W_Number))
                source[index].Error("Expected argument of type {0} instead saw {1}", W_Type.GetName<W_Number>(), source[index].TypeName);
            if (Math.Floor(source[index].Number) != source[index].Number)
                source[index].Error("Expected a number with no decimal places");

        }

        static CoreApi()
        {
            StringFunction("charCodeAt", (val, args) => {
                RequireIntArg(args, 0);
                return new W_Number(val.ToCharArray()[(int)args[0].Number]);
            });
            StringFunction("subString", (val, args) => {
                RequireIntArg(args, 0);
                return new W_String(HasIntArg(args, 1) ?
                    val.Substring((int)args[0].Number, (int)args[1].Number) :
                    val.Substring((int)args[0].Number));
            });
            StringFunction("charAt", (val, args) => {
                RequireIntArg(args, 0);
                return new W_String(val.Substring((int)args[1].Number, 1));
            });
            StringFunction("indexOf", (val, args) => {
                RequierArg<W_String>(args, 0);
                return new W_Number(val.IndexOf(args[0].String));
            });
            StringFunction("split", (val, args) => {
                RequierArg<W_String>(args, 0);
                return new W_Array(val.Split(new string[] { args[0].String }, StringSplitOptions.None).Select(x => new W_String(x)));
            });

        }
    }
}
