using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Common.Trees
{
    public class Tree : ITree
    {
        protected INode _root;

        public INode Root { get { return _root; } }

        public void AddRoot(INode root)
        {
            _root = root;
        }
    }
}
