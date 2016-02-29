using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/* A name of internal types must be accessable from bowth instanced and generic paramaters
 * that differs from C# internal type names for debugging purposes
 */
namespace REPL.Types {
    [AttributeUsage(AttributeTargets.Class)]
    class TypeName: Attribute {
        public string Name;
        public TypeName(string name) { Name = name; }
    }
}
