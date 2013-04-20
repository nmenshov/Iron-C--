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
        public Terminal ReturnType { get; set; }
        public Id Name { get; set; }
        public ParamDeclaration[] Params { get; set; }
        public Block Body { get; set; }

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

        public FuncDeclaration()
        {}
    }
}
