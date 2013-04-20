using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Syntax
{
    public abstract class Expression: Statement
    {
        public bool IsTypeChecked { get; set; }
    }
}
