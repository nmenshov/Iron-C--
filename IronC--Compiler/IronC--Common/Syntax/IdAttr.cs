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
    public class IdAttr:IAttribute 
    {
        [XmlIgnore]
        public string Name { get; set; }
        public bool IsEqual(string name, object value)
        {
            throw new NotImplementedException();
        }

        public Id Id { get; set; }

        public int NewId { get; set; }

        public string NewValue { get { return Id.Value + NewId; } }

        public IdAttr(Id id)
        {
            Id = id;
        }

        public IdAttr()
        {}
    }
}
