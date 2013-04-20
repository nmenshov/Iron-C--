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
    public class NewIdSetter:RecursionValidator<object>
    {
        private Dictionary<string, int> _currentId;

        public NewIdSetter()
        {
            _currentId = new Dictionary<string, int>();
        }


        protected override object GetStartValue()
        {
            return new object();
        }

        protected override void OperationBefore(INode parent, INode current, ref object data)
        {
            if (current.GetType() == typeof(VarDeclaration))
            {
                var find = current.Attribute.FirstOrDefault(x => x.GetType() == typeof(IdAttr)) as IdAttr;
               
                if (_currentId.ContainsKey(find.Id.Value))
                {
                    _currentId[find.Id.Value]+=1;
                    find.NewId = _currentId[find.Id.Value];
                }
                else
                {
                    _currentId.Add(find.Id.Value, 0);
                    find.NewId = 0;
                }
            }

            if (current.GetType() == typeof(FuncDeclaration))
            {

                var find = current.Attribute.FirstOrDefault(x => x.GetType() == typeof(ParamDeclaration)) as ParamDeclaration;
                if (find != null)
                {
                    if (_currentId.ContainsKey(find.Id.Value))
                    {
                        _currentId[find.Id.Value] += 1;
                        find.NewId = _currentId[find.Id.Value];
                    }
                    else
                    {
                        _currentId.Add(find.Id.Value, 0);
                        find.NewId = 0;
                    }
                }
            }

            if (current.GetType() != typeof (FuncDeclaration))
            {
                var find = current.Attribute.FirstOrDefault(x => x.GetType() == typeof(IdAttr)) as IdAttr;
                if (find != null)
                {
                    if (_currentId.ContainsKey(find.Id.Value))
                    {
                        find.NewId = _currentId[find.Id.Value];
                        
                    }
                }
            }
        }

        protected override void OperationAfter(INode parent, INode current, ref object data)
        {
            
        }

        private int FindDeclaration(INode current)
        {
            return 0;
        }
    }
}
