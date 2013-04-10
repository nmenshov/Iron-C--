using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Lexis
{
    public class Epsilon:Symbol
    {
        public Epsilon() : base(Constants.EpsilonName, SymbolType.Epsilon)
        {
        }

        public override Symbol GetCopy(string str)
        {
            return new Epsilon();
        }
    }
}
