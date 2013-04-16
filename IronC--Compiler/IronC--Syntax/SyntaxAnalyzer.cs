using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Linq;
using IronC__Common.Lexis;

namespace IronC__Syntax
{
    public class SyntaxAnalyzer
    {
        private readonly Scanner _scanner;
        private const int maxT = 35;

        public Errors errors;
        const int minErrDist = 2;
        int errDist = minErrDist;

        private Symbol _la;
        private Symbol _t;

        public SyntaxAnalyzer(IList<Symbol> input)
        {
            Debug.Assert(input != null);
            _scanner = new Scanner(input);
        }

        public void Analyze()
        {
            _la = null;
            Get();
            C__();
            Expect(Terminals.EOF);
        }

        #region Conflict resovlers

        private bool IsDecl()
        {
            var id = _scanner.Peek();
            var sc = _scanner.Peek();
            if ((_la == Terminals.Int || _la == Terminals.Char) && id == Terminals.Id && sc == Terminals.Semicolon)
                return true;
            if ((_la == Terminals.Int || _la == Terminals.Char) && id == Terminals.Id && sc == Terminals.LPar)
                return false;
            return true;
        }

        private bool IsAssign()
        {
            var next = _scanner.Peek();
            if (next == Terminals.Assign)
                return true;
            if (next != Terminals.LBrace)
                return false;
            int braces = 1;
            while (braces != 0)
            {
                next = _scanner.Peek();
                if (next == Terminals.LBrace)
                    braces++;
                else if (next == Terminals.RBrace)
                    braces--;
            }
            next = _scanner.Peek();
            return next == Terminals.Assign;
        }

        #endregion

        #region StartOfTableSet

        private static readonly Dictionary<Symbol, bool[]> _set = InitSet();

        private static Dictionary<Symbol, bool[]> InitSet()
        {
            var d = new Dictionary<Symbol, bool[]>();

            d.Add(Terminals.EOF, new[]{true, false, false});
            d.Add(Terminals.Id, new[]{false, true, false});
            d.Add(Terminals.Num, new[]{false, true, false});
            d.Add(Terminals.Int, new[]{false, false, false});

            d.Add(Terminals.Char, new[]{false, false, false});
            d.Add(Terminals.Semicolon, new[]{false, false, false});
            d.Add(Terminals.LPar, new[]{false, true, false});
            d.Add(Terminals.RPar, new[]{false, false, false});

            d.Add(Terminals.Assign, new[]{false, true, false});
            d.Add(Terminals.LBrace, new[]{false, false, false});
            d.Add(Terminals.RBrace, new[]{false, true, false});
            d.Add(Terminals.Comma, new[]{false, true, false});

            d.Add(Terminals.Start, new[]{false, false, false});
            d.Add(Terminals.End, new[]{false, false, false});
            d.Add(Terminals.Return, new[]{false, false, false});
            d.Add(Terminals.Read, new[]{false, false, false});

            d.Add(Terminals.Write, new[]{false, true, false});
            d.Add(Terminals.Writeln, new[]{false, true, false});
            d.Add(Terminals.Break, new[]{false, true, false});
            d.Add(Terminals.If, new[]{false, true, false});

            d.Add(Terminals.Else, new[]{false, false, false});
            d.Add(Terminals.While, new[]{false, true, false});
            d.Add(Terminals.Minus, new[]{false, true, true});
            d.Add(Terminals.Inv, new[]{false, true, false});
            
            d.Add(Terminals.Plus, new[]{false, false, true});
            d.Add(Terminals.Mul, new[]{false, false, true});
            d.Add(Terminals.Div, new[]{false, false, true});
            d.Add(Terminals.Equal, new[]{false, false, true});
            
            d.Add(Terminals.NotEqual, new[]{false, false, true});
            d.Add(Terminals.Less, new[]{false, false, true});
            d.Add(Terminals.LessOrEqual, new[]{false, false, true});
            d.Add(Terminals.Great, new[]{false, false, true});
           
            d.Add(Terminals.GreatOrEqual, new[]{false, false, true});
            d.Add(Terminals.And, new[]{false, false, true});
            d.Add(Terminals.Or, new[]{false, false, true});
            //d.Add(_35, new[]{false, false, false});
            
            //d.Add(_36, new[]{false, false, false});

            return d;
        }

        private bool StartOf(int s)
        {
            return _set[_la][s];
        }

        #endregion

        void SynErr(int n)
        {
            if (errDist >= minErrDist) errors.SynErr(/*_la.line, _la.col*/0, 0, n);
            errDist = 0;
        }

        public void SemErr(string msg)
        {
            if (errDist >= minErrDist) errors.SemErr(/*t.line, t.col*/0, 0, msg);
            errDist = 0;
        }

        void Get()
        {
            //for (; ; )
            //{
            //    _t = _la;
            //    _la = _scanner.Scan();
            //    if (true) { ++errDist; break; }
            //    _la = _t;
            //}
            _la = _scanner.Scan();
        }

        void Expect(Symbol n)
        {
            if (_la == n) Get(); else { SynErr(/*n*/0); }
        }

        void C__()
        {
            while (IsDecl())
            {
                VarDecl();
            }
            while (_la == Terminals.Int || _la == Terminals.Char)
            {
                FunDecl();
            }
        }

        void VarDecl()
        {
            Type();
            Expect(Terminals.Id);
            if (_la == Terminals.LBrace)
            {
                Get();
                Expect(Terminals.Num);
                Expect(Terminals.RBrace);
            }
            Expect(Terminals.Semicolon);
        }

        void FunDecl()
        {
            Type();
            Expect(Terminals.Id);
            Expect(Terminals.LPar);
            if (_la == Terminals.Int || _la == Terminals.Char)
            {
                ParamDecl();
            }
            Expect(Terminals.RPar);
            Block();
        }

        void Type()
        {
            if (_la == Terminals.Int)
            {
                Get();
            }
            else if (_la == Terminals.Char)
            {
                Get();
            }
            else SynErr(36);
        }

        void ParamDecl()
        {
            Type();
            Expect(Terminals.Id);
            if (_la == Terminals.LBrace)
            {
                Get();
                Expect(Terminals.RBrace);
            }
            while (_la == Terminals.Comma)
            {
                Get();
                Type();
                Expect(Terminals.Id);
                if (_la == Terminals.LBrace)
                {
                    Get();
                    Expect(Terminals.RBrace);
                }
            }
        }

        void Block()
        {
            Expect(Terminals.Start);
            while (_la == Terminals.Int || _la == Terminals.Char)
            {
                VarDecl();
            }
            while (StartOf(1))
            {
                Smth();
            }
            Expect(Terminals.End);
        }

        private void Smth()
        {
            if (_la == Terminals.Id || _la == Terminals.Num || _la == Terminals.LPar || _la == Terminals.Minus ||
                _la == Terminals.Inv)
            {
                Expr();
                Expect(Terminals.Semicolon);
            }
            else if (_la == Terminals.Return)
            {
                Get();
                Expr();
                Expect(Terminals.Semicolon);
            }
            else if (_la == Terminals.Read)
            {
                Get();
                Expect(Terminals.Id);
                Expect(Terminals.Semicolon);
            }
            else if (_la == Terminals.Write)
            {
                Get();
                Expr();
                Expect(Terminals.Semicolon);
            }
            else if (_la == Terminals.Writeln)
            {
                Get();
                Expect(Terminals.Semicolon);
            }
            else if (_la == Terminals.Break)
            {
                Get();
                Expect(Terminals.Semicolon);
            }
            else if (_la == Terminals.If)
            {
                Get();
                Expect(Terminals.LPar);
                Expr();
                Expect(Terminals.RPar);
                Smth();
                Expect(Terminals.Else);
                Smth();
            }
            else if (_la == Terminals.While)
            {
                Get();
                Expect(Terminals.LPar);
                Expr();
                Expect(Terminals.RPar);
                Smth();
            }
            else if (_la == Terminals.Start)
            {
                Block();
            }
            else
                SynErr(37);
        }

        void Expr()
        {
            SimExpr();
            while (StartOf(2))
            {
                BinaryOp();
                Expr();
            }
        }

        void SimExpr()
        {
            if (_la == Terminals.Minus || _la == Terminals.Inv)
            {
                UnaryOp();
                Expr();
            }
            else if (_la == Terminals.LPar)
            {
                Get();
                Expr();
                Expect(Terminals.RPar);
            }
            else if (_la == Terminals.Num)
            {
                Get();
            }
            else if (_la == Terminals.Id)
            {
                if (IsAssign())
                {
                    Assign();
                }
                else
                {
                    Access();
                }
            }
            else SynErr(38);
        }

        private void BinaryOp()
        {
            if (_la == Terminals.Plus || _la == Terminals.Minus || _la == Terminals.Div || _la == Terminals.Mul || _la == Terminals.Equal ||
                _la == Terminals.NotEqual || _la == Terminals.Less || _la == Terminals.LessOrEqual || _la == Terminals.Great || 
                _la == Terminals.GreatOrEqual || _la == Terminals.And || _la == Terminals.Or)
            {
                Get();
            }
            else
                SynErr(39);
        }

        void UnaryOp()
        {
            if (_la == Terminals.Minus || _la == Terminals.Inv)
            {
                Get();
            }
            else SynErr(40);
        }

        void Assign()
        {
            Expect(Terminals.Id);
            if (_la == Terminals.LBrace)
            {
                Get();
                Expr();
                Expect(Terminals.RBrace);
            }
            Expect(Terminals.Assign);
            Expr();
        }

        void Access()
        {
            Expect(Terminals.Id);
            if (_la == Terminals.LBrace || _la == Terminals.LPar)
            {
                if (_la == Terminals.LPar)
                {
                    Get();
                    Expr();
                    Expect(Terminals.RPar);
                }
                else
                {
                    Get();
                    Expr();
                    Expect(Terminals.RBrace);
                }
            }
        }
    }

    public class Errors
    {
        public int count = 0;                                    // number of errors detected
        public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
        public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

        public virtual void SynErr(int line, int col, int n)
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
            errorStream.WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public virtual void SemErr(int line, int col, string s)
        {
            errorStream.WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public virtual void SemErr(string s)
        {
            errorStream.WriteLine(s);
            count++;
        }

        public virtual void Warning(int line, int col, string s)
        {
            errorStream.WriteLine(errMsgFormat, line, col, s);
        }

        public virtual void Warning(string s)
        {
            errorStream.WriteLine(s);
        }
    } // Errors
}
