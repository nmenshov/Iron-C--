using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common;
using IronC__Common.Lexis;

namespace IronC__Lexical
{
    public class LexicalAnalyzer
    {
        private readonly Grammar _grammar;

        public LexicalAnalyzer(Grammar grammar)
        {
            _grammar = grammar;
        }

        public List<Symbol> Convert(string input)
        {
            Contract.Requires(input != null);
            Contract.Requires(_grammar != null);

            var ret = new List<Symbol>();

            string rab = "";
            string str = input.TrimStart(' ');
            input += " ";

            foreach (var symbol in str)
            {
                if (symbol == ' ')
                {
                    ret.Add(GetSymbol(rab));
                    rab = "";
                    continue;
                }

                if (!char.IsLetterOrDigit(symbol) && !IsLetterorDigit(rab))
                {
                    ret.Add(GetSymbol(rab));
                    rab = "";
                }

                rab += symbol;
            }
            return ret;
        }

        public bool IsLetterorDigit(string str)
        {
            return str.All(char.IsLetterOrDigit) && str.Length < 2;
        }

        public Symbol GetSymbol(string str)
        {
            var element = _grammar.Terminals.Find(x => x.IsEqual(str));
            var ret = element.GetCopy(str);
            return ret;
        }
    }
}
