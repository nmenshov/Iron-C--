using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Trees;

namespace IronC__Semantics.Validators
{
    public abstract class RecursionValidator<T>:IValidator
    {
        private T _start;
        protected List<string> _errors;

        #region StartValue

        protected abstract T GetStartValue();

        private void CreateFilterInner()
        {
            _start = GetStartValue();
        }

        #endregion

        public List<string> Validate(ITree tree)
        {
            _errors = new List<string>();
            CreateFilterInner();
            OperationBefore(tree.Root, tree.Root, ref _start);
            bool ret = InnerValidate(tree.Root, ref _start);
            OperationAfter(tree.Root, tree.Root, ref _start);
            return _errors;
        }

        private bool InnerValidate(INode node, ref T data)
        {
            foreach (var child in node.Children)
            {                
                OperationBefore(node, child, ref data);
                InnerValidate(child, ref data);
                OperationAfter(node, child, ref data);
            }
            return false;
        }

        protected abstract void OperationBefore(INode parent, INode current,ref T data);

        protected abstract void OperationAfter(INode parent, INode current, ref T data);
    }
}
