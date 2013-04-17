using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Syntax;

namespace IronC__Generator
{
    class Function
    {
        public string Name { get; private set; }
        public MethodBuilder Info { get; private set; }
        public Type ReturnType { get; private set; }
        public Block Body { get; private set; }
        public ILGenerator ILGenerator { get; private set; }
        public FuncParam[] Params { get; private set; }

        public Function(string name, Type returnType, FuncParam[] parameters, Block body, MethodBuilder info)
        {
            Name = name;
            ReturnType = returnType;
            Params = parameters;
            Body = body;
            Info = info;
            ILGenerator = info.GetILGenerator();
        }
    }
}
