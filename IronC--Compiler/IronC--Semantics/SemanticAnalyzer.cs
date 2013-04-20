using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Trees;
using IronC__Semantics.Validators;

namespace IronC__Semantics
{
    public class SemanticAnalyzer
    {
        private ITree _tree;
        private List<IValidator> _validators;
        public List<string> Errors { get; private set; }

        public SemanticAnalyzer(ITree tree)
        {
            _tree = tree;
            Errors = new List<string>();
            _validators = new List<IValidator>()
                {                    
                    new UniqueValidator(),
                    new NewIdSetter(),
                    new ParamUsage(),
                    new TypeValidator()
                };
        }

        public void DecorateAndValidateTree()
        {            
            if(_tree!=null)
                _validators.ForEach(x=>Errors.AddRange(x.Validate(_tree)));
        }
    }
}
