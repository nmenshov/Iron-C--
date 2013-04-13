using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Grammars;

namespace IronC__Common.Lexis
{
    public class NotTerminal:Symbol
    {
        public NotTerminal(string name) : base(name, SymbolType.NotTerminal)
        {
        }

        public override Symbol GetCopy(string str)
        {
            return new NotTerminal(Name);
        }
    }
}
