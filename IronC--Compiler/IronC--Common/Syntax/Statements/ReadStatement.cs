using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;

namespace IronC__Common.Syntax
{
    public class ReadStatement: Statement
    {
        public Id Id { get; private set; }

        public ReadStatement(Id id)
        {
            Id = id;
            AddAttribute(new IdAttr(id));
        }
    }
}
