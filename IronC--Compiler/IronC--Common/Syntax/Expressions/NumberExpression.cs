using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;

namespace IronC__Common.Syntax.Expressions
{
    public class NumberExpression: Expression
    {
        public Num Num { get; private set; }

        public NumberExpression(Num num)
        {
            Num = num;
            AddAttribute(new NumAttr(num));
        }
    }
}
