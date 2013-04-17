using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Generator
{
    class GlobalArrayVariable: GlobalVariable
    {
        public int Size { get; private set; }

        public GlobalArrayVariable(string name, Type type, FieldBuilder info, int size)
            :base(name, info, type)
        {
            Size = size;
        }

        public override void Init(ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4, Size);
            il.Emit(OpCodes.Newarr, Type);
            il.Emit(OpCodes.Stsfld, FieldInfo);
        }
    }
}
