using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Trees;

namespace IronC__Common.Syntax
{
    public class IfStatement: Statement
    {
        public Expression Condition { get; set; }
        public Statement TrueStatement { get; set; }
        public Statement FalseStatement { get; set; }

        public IfStatement(Expression condition, Statement trueStatement, Statement falseStatement)
        {
            Condition = condition;
            TrueStatement = trueStatement;
            FalseStatement = falseStatement;

            AddChild(condition);
            AddChild(trueStatement);
            AddChild(falseStatement);
        }

        public IfStatement()
        {}
    }
}
