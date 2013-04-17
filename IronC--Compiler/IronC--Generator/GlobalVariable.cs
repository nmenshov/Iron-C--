using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Generator
{
    class GlobalVariable
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public FieldBuilder FieldInfo { get; private set; }

        public GlobalVariable(string name, FieldBuilder info, Type type)
        {
            Name = name;
            FieldInfo = info;
            Type = type;
        }

        public virtual void Init(ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stsfld, FieldInfo);
        }
    }
}
