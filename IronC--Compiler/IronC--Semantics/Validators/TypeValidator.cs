using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;
using IronC__Common.Syntax;
using IronC__Common.Syntax.Expressions;
using IronC__Common.Trees;

namespace IronC__Semantics.Validators
{
    public class TypeValidator:RecursionValidator<object>
    {
        protected override object GetStartValue()
        {
            return new object();
        }

        protected override void OperationBefore(INode parent, INode current, ref object data)
        {
            if (current.GetType() == typeof (BinaryExpression))
            {
                var exp = current as BinaryExpression;
                if (!exp.IsTypeChecked)
                {
                    if (Check(current) == "")
                    {
                        _errors.Add(string.Format("Ошибка в выражении"));
                    }
                }
            }
            if (current.GetType() == typeof(SetValueExpression))
            {
                CheckSetValueExpresion(current as SetValueExpression);
            }
        }

        protected override void OperationAfter(INode parent, INode current, ref object data)
        {
           
        }

        private void CheckSetValueExpresion(SetValueExpression expr)
        {
            var attr = expr.Attribute.First(x => x.GetType() == typeof (TypeAttr)) as TypeAttr;
            var attr2 = expr.Attribute.First(x => x.GetType() == typeof(IdAttr)) as IdAttr;
            string check = Check(expr.Children[0]);
            if (!IsEqual(attr.Type.Name, check))
            {
                _errors.Add(string.Format("Ошибка в выражении, строка {0}", attr2.Id.GetRowNumber()));
            }
        }

        private string Check(INode current)
        {
            ((Expression) current).IsTypeChecked = true;

            if (current.Children.Count>0)
            {
                var list = current.Children.Select(Check).ToList();
                var dist = list.Distinct().ToList();

                if (dist.Count(x => x == "") != 0)
                    return "";

                if (dist.Count == 1)
                    return dist.First();

                if (dist.Count == 2 && dist.Count(x => x == "num") != 0)
                    return dist.Find(x => x != "num");

                return "";
            }

            var type = current.Attribute.FirstOrDefault(x => x.GetType() == typeof(TypeAttr)) as TypeAttr;
            if (type != null)
                return type.Type.Name;

            if (current.GetType() == typeof(NumberExpression))
            {
                return "num";
            }

            return null;
        }

        private bool IsEqual(string var1, string var2)
        {
            if (var1 == "num" || var2 == "num")
                return true;
            return var1 == var2;
        }
    }
}
