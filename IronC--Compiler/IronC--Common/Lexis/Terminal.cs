﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Lexis
{
    public class Terminal:Symbol
    {
        public Terminal(string name) : base(name, SymbolType.Terminal)
        {
        }

        public override Symbol GetCopy(string str)
        {
            return new Terminal(Name);
        }
    }
}
