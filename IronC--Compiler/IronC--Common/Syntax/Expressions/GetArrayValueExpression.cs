using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;

namespace IronC__Common.Syntax.Expressions
{
    public class GetArrayValueExpression: Expression
    {
        public Id Variable { get; private set; }
        public Expression Index { get; private set; }

        public GetArrayValueExpression(Id variable, Expression index)
        {
            Variable = variable;
            Index = index;
            AddAttribute(new IdAttr(variable));
            AddChild(index);
        }
    }
}
