using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;
using IronC__Common.Trees;

namespace IronC__Common.Syntax
{
    public class IdAttr:IAttribute 
    {
        public string Name { get; private set; }
        public bool IsEqual(string name, object value)
        {
            throw new NotImplementedException();
        }

        public Id Id { get; private set; }

        public int NewId { get; set; }

        public string NewValue { get { return Id.Value + NewId; } }

        public IdAttr(Id id)
        {
            Id = id;
        }
    }
}
