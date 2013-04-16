using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Trees;

namespace IronC__Common.Syntax
{
    public class WhileStatement: Statement
    {
        public Expression Condition { get; private set; }
        public Statement Statement { get; private set; }

        public WhileStatement(Expression condition, Statement statement)
        {
            Condition = condition;
            Statement = statement;

            AddChild(condition);
            AddChild(statement);
        }
    }
}
