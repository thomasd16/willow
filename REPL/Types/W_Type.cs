using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.Types
{
    abstract class W_Type
    {
        //null type has only one value as such only one instance is needed
        public static W_Type NULL = new W_Null();

        //null function and null object needed to satisfy return obligation
        //even when errors are thrown
        private static W_Object NullObject = new W_Object();
        private static W_Delegate NullFunction = (thisarg, args) => NULL;

        public static string GetName<T>() where T : W_Type {
            return (Attribute.GetCustomAttribute(typeof(T),typeof(TypeName)) as TypeName).Name;
        }

        public string TypeName { get { return (Attribute.GetCustomAttribute(this.GetType(), typeof(TypeName)) as TypeName).Name; } }

        public static bool Equal(W_Type rhs, W_Type lhs)
        {
            if (rhs == lhs)
                return true;
            if (rhs is W_Null)
                return lhs is W_Null;
            if (rhs is W_Bool)
                return lhs is W_Bool && lhs.Bool == rhs.Bool;
            if (rhs is W_Function)
                return lhs is W_Function && lhs.Function == rhs.Function;
            if (rhs is W_Number)
                return lhs is W_Number && lhs.Number == rhs.Number;
            if (rhs is W_String)
                return lhs is W_String && lhs.String == rhs.String;
            return false;
        }

        public void Error(string format, params object[] data)
        {
            throw new Exception(String.Format(format, data));
        }

        public virtual W_Object Object {
            get {
                Error("{0} is not an object", this);
                return NullObject; 
            } 
        }

        public virtual string String
        {
            get
            {
                Error("{0} is not a string", this);
                return "";
            }
        }

        public virtual W_Delegate Function
        {
            get
            {
                Error("{0} is not a function", this);
                return NullFunction;
            }
        }

        public virtual double Number
        {
            get
            {
                Error("{0} is not a function", this);
                return 0d;
            }
        }

        public virtual bool Bool
        {
            get
            {
                Error("{0} is not a bool", this);
                return true;
            }
        }

    }
}
