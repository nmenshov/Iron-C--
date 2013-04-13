using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;

namespace IronC__Common.Grammars
{
    public class Rule
    {
        public readonly NotTerminal Left;
        public List<Symbol> Right { get; private set; } 

        public Rule(NotTerminal notTerminal)
        {
            Left = notTerminal;
            Right = new List<Symbol>();
        }

        public void AddRightPart(Symbol symbol)
        {
            Right.Add(symbol);
        }
    }
}
