using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;
using IronC__Common.Trees;

namespace IronC__Common.Syntax
{
    public class FuncDeclaration: Node
    {
        public Terminal ReturnType { get; private set; }
        public Id Name { get; private set; }
        public ParamDeclaration[] Params { get; private set; }
        public Block Body { get; private set; }

        public FuncDeclaration(Terminal returnType, Id name, ParamDeclaration[] parameters, Block body)
        {
            ReturnType = returnType;
            Params = parameters;
            Body = body;
            Name = name;

            AddAttribute(new TypeAttr(returnType));
            AddAttribute(new IdAttr(name));
            AddAttribute(parameters);
            AddChild(body);
        }
    }
}
