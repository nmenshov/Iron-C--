using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Grammars;

namespace IronC__Common.Lexis
{
    public class EndSymbol:Symbol
    {

        public EndSymbol() : base(Constants.EndSymbolName, SymbolType.EndSymbol)
        {
        }

        public override Symbol GetCopy(string str)
        {
            return new EndSymbol();
        }
    }
}
