using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IronC__Common.Lexis;
using IronC__Common.Trees;

namespace IronC__Common.Syntax
{
    public class TypeAttr: IAttribute
    {
        [XmlIgnore]
        public string Name { get; set; }
        public bool IsEqual(string name, object value)
        {
            throw new NotImplementedException();
        }

        public Terminal Type { get; set; }

        public TypeAttr(Terminal type)
        {
            Type = type;
        }

        public TypeAttr()
        {}
    }
}
