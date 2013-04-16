using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;
using IronC__Common.Trees;

namespace IronC__Common.Syntax
{
    class OperatorAttr: IAttribute
    {
        public string Name { get; private set; }
        public bool IsEqual(string name, object value)
        {
            throw new NotImplementedException();
        }

        public Terminal Operator { get; private set; }

        public OperatorAttr(Terminal op)
        {
            Operator = op;
        }
    }
}
