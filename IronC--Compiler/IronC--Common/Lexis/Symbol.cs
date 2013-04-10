using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Lexis
{
    public abstract class Symbol
    {
        public readonly string Name;
        public readonly SymbolType Type;

        protected Symbol(string name, SymbolType type)
        {
            Name = name;
            Type = type;
        }

        public virtual bool IsEqual(string str)
        {
            return str == Name;
        }

        public abstract Symbol GetCopy(string str);
    }
}
