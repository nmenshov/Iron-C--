using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;

namespace IronC__Common
{
    public class Grammar
    {
        public readonly NotTerminal StartSymbol;
        public readonly List<Terminal> Terminals;
        public readonly List<NotTerminal> NotTerminals;
        //список правил

        public Grammar(NotTerminal startSymbol, List<Terminal> terminals, List<NotTerminal> notTerminals)
        {
            StartSymbol = startSymbol;
            Terminals = terminals;
            NotTerminals = notTerminals;
        }
    }
}
