using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;

namespace IronC__Common.Syntax.Expressions
{
    public class SetArrayValueExpression: Expression
    {
        public Id Variable { get; private set; }
        public Expression Value { get; private set; }
        public Expression Index { get; private set; }

        public SetArrayValueExpression(Id variable, Expression value, Expression index)
        {
            Variable = variable;
            Value = value;
            Index = index;
            AddAttribute(new IdAttr(variable));
            AddChild(value);
            AddChild(index);
        }
    }
}
