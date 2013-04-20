using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronC__Syntax
{
    public class SyntaxErrors: List<string>
    {
        private const string ErrMsgFormat = "-- line {0}: {1}";

        public virtual void SynErr(int line, int n)
        {
            string s;
            switch (n)
            {
                case 0: s = "EOF expected"; break;
                case 1: s = "ident expected"; break;
                case 2: s = "number expected"; break;
                case 3: s = "int expected"; break;
                case 4: s = "char expected"; break;
                case 5: s = "semicolon expected"; break;
                case 6: s = "lpar expected"; break;
                case 7: s = "rpar expected"; break;
                case 8: s = "assign expected"; break;
                case 9: s = "lbrace expected"; break;
                case 10: s = "rbrace expected"; break;
                case 11: s = "\",\" expected"; break;
                case 12: s = "\"{\" expected"; break;
                case 13: s = "\"}\" expected"; break;
                case 14: s = "\"return\" expected"; break;
                case 15: s = "\"read\" expected"; break;
                case 16: s = "\"write\" expected"; break;
                case 17: s = "\"writeln\" expected"; break;
                case 18: s = "\"break\" expected"; break;
                case 19: s = "\"if\" expected"; break;
                case 20: s = "\"else\" expected"; break;
                case 21: s = "\"while\" expected"; break;
                case 22: s = "\"-\" expected"; break;
                case 23: s = "\"!\" expected"; break;
                case 24: s = "\"+\" expected"; break;
                case 25: s = "\"*\" expected"; break;
                case 26: s = "\"/\" expected"; break;
                case 27: s = "\"==\" expected"; break;
                case 28: s = "\"!=\" expected"; break;
                case 29: s = "\"<\" expected"; break;
                case 30: s = "\"<=\" expected"; break;
                case 31: s = "\">\" expected"; break;
                case 32: s = "\">=\" expected"; break;
                case 33: s = "\"&&\" expected"; break;
                case 34: s = "\"||\" expected"; break;
                case 35: s = "??? expected"; break;
                case 36: s = "invalid Type"; break;
                case 37: s = "invalid Smth"; break;
                case 38: s = "invalid SimExpr"; break;
                case 39: s = "invalid BinaryOp"; break;
                case 40: s = "invalid UnaryOp"; break;

                default: s = "error " + n; break;
            }
            Add(string.Format(ErrMsgFormat, line, s));
        }

        public virtual void SemErr(int line, string s)
        {
            Add(string.Format(ErrMsgFormat, line, s));
        }

        public virtual void SemErr(string s)
        {
            Add(s);
        }

        public virtual void Warning(int line, string s)
        {
            Add(string.Format(ErrMsgFormat, line, s));
        }

        public virtual void Warning(string s)
        {
            Add(s);
        }
    }
}
