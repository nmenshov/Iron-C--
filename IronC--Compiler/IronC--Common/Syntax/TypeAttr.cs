using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;
using IronC__Common.Trees;

namespace IronC__Common.Syntax
{
    class TypeAttr: IAttribute
    {
        public string Name { get; private set; }
        public bool IsEqual(string name, object value)
        {
            throw new NotImplementedException();
        }

        public Terminal Type { get; private set; }

        public TypeAttr(Terminal type)
        {
            Type = type;
        }
    }
}
