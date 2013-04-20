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
    class OperatorAttr: IAttribute
    {
        [XmlIgnore]
        public string Name { get; set; }
        public bool IsEqual(string name, object value)
        {
            throw new NotImplementedException();
        }

        public Terminal Operator { get; set; }

        public OperatorAttr(Terminal op)
        {
            Operator = op;
        }

        public OperatorAttr()
        {}
    }
}
