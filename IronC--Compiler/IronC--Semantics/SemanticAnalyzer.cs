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
        private List<string> _erros;

        public SemanticAnalyzer(ITree tree)
        {
            _tree = tree;
            _erros = new List<string>();
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
                _validators.ForEach(x=>_erros.AddRange(x.Validate(_tree)));
        }
    }
}
