using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPL.Types
{
    [TypeName("Object")]
    class W_Object : W_Type
    {
        W_Object Prototype;
        Dictionary<string, W_Type> Properties;

        public W_Object() 
        {
            Properties = new Dictionary<string, W_Type>();
        }
        public W_Object(Dictionary<string, W_Type> values)
        {
            Properties = values;
        }
        public W_Object(W_Object proto)
        {
            Prototype = proto;
            Properties = new Dictionary<string, W_Type>();
        }
        public W_Object(W_Object proto, Dictionary<string, W_Type> values)
        {
            Properties = values;
            Prototype = proto;
        }

        public virtual W_Type this[string key]
        {
            get
            {
                if (Properties.ContainsKey(key))
                    return Properties[key];
                if (Prototype != null)
                    return Prototype[key];
                return W_Type.NULL;
            }
            set
            {
                Properties[key] = value;
            }
        }

        public override string ToString()
        {
            return "Object";   
        }
    }
}
