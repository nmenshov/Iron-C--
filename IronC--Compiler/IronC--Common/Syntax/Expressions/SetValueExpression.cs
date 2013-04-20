using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;

namespace IronC__Common.Syntax.Expressions
{
    public class SetValueExpression: Expression
    {
        public Id Variable { get; set; }
        public Expression Value { get; set; }

        public SetValueExpression(Id variable, Expression value)
        {
            Variable = variable;
            Value = value;
            AddAttribute(new IdAttr(variable));
            AddChild(value);
        }

        public SetValueExpression()
        {}
    }
}
