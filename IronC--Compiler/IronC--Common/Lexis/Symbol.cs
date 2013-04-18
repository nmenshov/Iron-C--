using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IronC__Common.Grammars;

namespace IronC__Common.Lexis
{    
    public abstract class Symbol
    {
        public string Name { get; set; }
        public readonly SymbolType Type;

        public Symbol()
        {
        }

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

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Name.Equals((obj as Symbol).Name);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(Symbol a, Symbol b)
        {
            object oa = a;
            object ob = b;
            if (oa == null && ob == null)
                return true;
            if (oa == null || ob == null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Symbol a, Symbol b)
        {
            object oa = a;
            object ob = b;
            if (oa == null && ob == null)
                return false;
            if (oa == null || ob == null)
                return true;

            return !a.Equals(b);
        }
    }
}
