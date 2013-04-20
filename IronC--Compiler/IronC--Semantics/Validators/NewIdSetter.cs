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
    public class NewIdSetter : RecursionValidator<object>
    {
        private Dictionary<string, VarParam> _currentId;

        public NewIdSetter()
        {
            _currentId = new Dictionary<string, VarParam>();
        }

        protected override object GetStartValue()
        {
            return new object();
        }

        protected override void OperationBefore(INode parent, INode current, ref object data)
        {
            if (current.GetType() == typeof (VarDeclaration))
            {
                var find = current.Attribute.FirstOrDefault(x => x.GetType() == typeof (IdAttr)) as IdAttr;
                var type = current.Attribute.FirstOrDefault(x => x.GetType() == typeof(TypeAttr)) as TypeAttr;

                if (_currentId.ContainsKey(find.Id.Value))
                {
                    _currentId[find.Id.Value].Value += 1;
                    find.NewId = _currentId[find.Id.Value].Value;
                }
                else
                {
                    _currentId.Add(find.Id.Value, new VarParam(type));
                    find.NewId = 0;
                }
            }

            if (current.GetType() == typeof (FuncDeclaration))
            {

                var find =
                    current.Attribute.FirstOrDefault(x => x.GetType() == typeof (ParamDeclaration)) as ParamDeclaration;
                var find2 =
                    current.Attribute.FirstOrDefault(x => x.GetType() == typeof(IdAttr)) as IdAttr;
                var find3 =
                    current.Attribute.FirstOrDefault(x => x.GetType() == typeof(TypeAttr)) as TypeAttr;

                if (_currentId.ContainsKey(find2.Id.Value))
                {
                    _errors.Add(string.Format("Повторное объявление функции {0}, строка {1}", find2.Id.Value, find2.Id.GetRowNumber()));
                }
                else
                {
                    _currentId.Add(find2.Id.Value, new VarParam(new TypeAttr(find3.Type)));
                    find2.NewId = 0;
                }
                
                if (find != null)
                {
                    if (_currentId.ContainsKey(find.Id.Value))
                    {
                        _currentId[find.Id.Value].Value += 1;
                        find.NewId = _currentId[find.Id.Value].Value;
                    }
                    else
                    {
                        _currentId.Add(find.Id.Value, new VarParam(new TypeAttr(find.Type)));
                        find.NewId = 0;
                    }
                }
            }

            if (current.GetType() != typeof (FuncDeclaration))
            {
                var find = current.Attribute.FirstOrDefault(x => x.GetType() == typeof (IdAttr)) as IdAttr;
                if (find != null)
                {
                    if (_currentId.ContainsKey(find.Id.Value))
                    {
                        find.NewId = _currentId[find.Id.Value].Value;
                        current.AddAttribute(_currentId[find.Id.Value].VType);
                    }
                }
            }
        }

        protected override void OperationAfter(INode parent, INode current, ref object data)
        {

        }
    }


    internal class VarParam
    {
        public int Value { get; set; }
        public TypeAttr VType { get; set; }

        public VarParam(TypeAttr type)
        {
            Value = 0;
            VType = type;
        }
    }
}


