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
        public Id Variable { get; set; }
        public Expression Value { get; set; }
        public Expression Index { get; set; }

        public SetArrayValueExpression(Id variable, Expression value, Expression index)
        {
            Variable = variable;
            Value = value;
            Index = index;
            AddAttribute(new IdAttr(variable));
            AddChild(value);
            AddChild(index);
        }

        public SetArrayValueExpression()
        {}
    }
}
