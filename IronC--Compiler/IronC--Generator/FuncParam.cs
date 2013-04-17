using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Generator
{
    class FuncParam
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public int Index { get; private set; }
        public bool IsArray { get; private set; }

        public FuncParam(string name, Type type, int index)
        {
            Name = name;
            Type = type;
            Index = index;
            IsArray = type == typeof (int[]) || type == typeof (byte[]);
        }
    }
}
