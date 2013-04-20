using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;
using IronC__Common.Syntax;
using IronC__Common.Trees;

namespace IronC__Semantics.Validators
{
    public class ParamUsage : RecursionValidator<List<Id>>
    {
        protected override List<Id> GetStartValue()
        {
            return new List<Id>();
        }

        protected override void OperationBefore(INode parent, INode current, ref List<Id> data)
        {
            if (current.GetType() == typeof(Program) || current.GetType() == typeof(Block))
            {
                var param =
                    current.Children.Where(x => x.GetType() == typeof(VarDeclaration))
                           .Select(x => x as VarDeclaration)
                           .ToList();

                foreach (var declaration in param)
                {
                    if (data.Count(x => x.IsEqual(declaration.Id.Value)) == 0)
                        data.Add(declaration.Id);
                    else
                    {
                        _errors.Add(string.Format("Повторное объявление переменной {0}, строка {1}", declaration.Id.Value, declaration.Id.GetRowNumber()));
                    }
                }
            }

            if (current.GetType() == typeof(FuncDeclaration))
            {
                var param =
                    current.Attribute.Where(x => x.GetType() == typeof(ParamDeclaration))
                           .Select(x => x as ParamDeclaration)
                           .ToList();
                data.Add(((IdAttr)current.Attribute.First(x=>x.GetType()==typeof(IdAttr))).Id);
                foreach (var declaration in param)
                {
                    if (data.Count(x => x.IsEqual(declaration.Id.Value)) == 0)
                        data.Add(declaration.Id);
                    else
                    {
                        _errors.Add(string.Format("Повторное объявление переменной {0}, строка {1}", declaration.Id.Value, declaration.Id.GetRowNumber()));
                    }
                }

                var ret = current.Children[0].Children.FirstOrDefault(x => x.GetType() == typeof (ReturnStatement));
                if (ret == null)
                {
                    var id = current.Children.First(x => x.GetType() == typeof(IdAttr)) as IdAttr;
                    _errors.Add(string.Format("Функция не возвращает значение {0}, строка {1}", id.Id.Value, id.Id.GetRowNumber()));
                }
            }
            if (current.GetType() != typeof(FuncDeclaration) && current.GetType() != typeof(VarDeclaration))
            {
                var find = current.Attribute.FirstOrDefault(x => x.GetType() == typeof(IdAttr)) as IdAttr;
                if (find != null)
                {

                    if (data.Count(x=>x.Value==find.Id.Value)==0)
                    {
                        _errors.Add(string.Format("Использование необъявленной переменной {0}, строка {1}",
                                                find.Id.Value, find.Id.GetRowNumber()));
                    }                    
                }
            }
        }

        protected override void OperationAfter(INode parent, INode current, ref List<Id> data)
        {
            if (current.GetType() == typeof(Program) || current.GetType() == typeof(Block))
            {
                var param = current.Children.Where(x => x.GetType() == typeof(VarDeclaration)).Select(x => x as VarDeclaration).ToList();

                foreach (var declaration in param)
                {
                    if (data.Count(x => x.IsEqual(declaration.Id.Value)) != 0)
                        data.RemoveAll(x => x.Value == declaration.Id.Value);
                }
            }
            if (current.GetType() == typeof(FuncDeclaration))
            {
                var param = current.Attribute.Where(x => x.GetType() == typeof(ParamDeclaration)).Select(x => x as ParamDeclaration).ToList();

                foreach (var declaration in param)
                {
                    if (data.Count(x => x.IsEqual(declaration.Id.Value)) != 0)
                        data.RemoveAll(x => x.Value == declaration.Id.Value);
                }
            }
        }
    }
}
