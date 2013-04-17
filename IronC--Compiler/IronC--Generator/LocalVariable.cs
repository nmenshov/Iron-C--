using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Generator
{
    class LocalVariable
    {
        public string Name { get; private set; }
        public LocalBuilder Info { get; private set; }
        public Type Type { get; private set; }

        public LocalVariable(string name, LocalBuilder info, Type type)
        {
            Name = name;
            Type = type;
            Info = info;
        }
    }
}
