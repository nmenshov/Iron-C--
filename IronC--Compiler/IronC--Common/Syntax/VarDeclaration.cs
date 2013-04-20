using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;
using IronC__Common.Trees;

namespace IronC__Common.Syntax
{
    public class VarDeclaration: Node
    {
        public Terminal Type { get; set; }
        public Id Id { get; private set; }

        public VarDeclaration(Terminal type, Id id)
        {
            Type = type;
            Id = id;
            AddAttribute(new TypeAttr(type));
            AddAttribute(new IdAttr(id));
        }
    }
}
