using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;

namespace IronC__Common.Syntax.Expressions
{
    public class GetValueExpression: Expression
    {
        public Id Variable { get; private set; }

        public GetValueExpression(Id variable)
        {
            Variable = variable;
            AddAttribute(new IdAttr(variable));
        }
    }
}
