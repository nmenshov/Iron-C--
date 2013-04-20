using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;

namespace IronC__Common.Syntax
{
    public class ArrayDeclaration: VarDeclaration
    {
        public Num Size { get; set; }

        public ArrayDeclaration(Terminal type, Id id, Num size)
            :base(type, id)
        {
            Size = size;
            AddAttribute(new NumAttr(size));
        }

        public ArrayDeclaration()
        {}
    }
}
