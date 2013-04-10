using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Trees
{
    public class Tree<T> : ITree<T>
    {
        protected INode<T> _root;

        public INode<T> Root { get { return _root; } }

        public void AddRoot(INode<T> root)
        {
            _root = root;
        }
    }
}
