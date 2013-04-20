using System;
using System.Collections.Generic;
using IronC__Common.Trees;

namespace IronC__Semantics.Validators
{
    public abstract class ValidatorFilter:IValidator
    {
        private List<Type> _typesFilter;
        private List<Type> _attributeFilter;
        protected List<string> _errors;

        protected ValidatorFilter()
        {
        }

        #region CreateFilterType

        protected abstract List<Type> CreateFilterType();

        protected abstract List<Type> CreateFilterAttribute();

        private void CreateFilterInner()
        {
            _typesFilter = CreateFilterType();
            _attributeFilter = CreateFilterAttribute();
        }

        #endregion

        public List<string> Validate(ITree tree)
        {
            _errors = new List<string>();
            CreateFilterInner();
            InnerValidate(tree.Root);
            return _errors;
        }

        private bool InnerValidate(INode node)
        {
            foreach (var child in node.Children)
            {
                if(_typesFilter.Contains(child.GetType()))
                    Operation(node,child, child.GetType());
                InnerValidate(child);
            }
            return false;
        }

         protected abstract void Operation(INode parent, INode current, Type type);
    }
}
