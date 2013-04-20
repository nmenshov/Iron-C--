using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;

namespace IronC__Common.Syntax.Expressions
{
    public class BinaryExpression: Expression
    {
        public Terminal Operator { get; set; }
        public Expression LeftOperand { get; set; }
        public Expression RightOperand { get; set; }

        public BinaryExpression(Expression expression1, Terminal op, Expression expression2)
        {
            Operator = op;
            LeftOperand = expression1;
            RightOperand = expression2;
            AddAttribute(new OperatorAttr(op));
            AddChild(expression1);
            AddChild(expression2);
        }

        public BinaryExpression()
        {}
    }
}
