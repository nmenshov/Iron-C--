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
using IronC__Common.Syntax;
using IronC__Common.Syntax.Expressions;
using IronC__Common.Trees;

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
            errors = new Errors();
        }

        public ITree Analyze()
        {
            _la = null;
            Get();
            var p = C__();
            Expect(Terminals.EOF);

            if (errors.count == 0)
            {
                var tree = new Tree();
                tree.AddRoot(p);
                return tree;
            }
            return null;
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

            d.Add(Terminals.EOF, new[]{true, false, false, false});
            d.Add(Terminals.Id, new[]{false, true, false, true});
            d.Add(Terminals.Num, new[]{false, true, false, true});
            d.Add(Terminals.Int, new[]{false, false, false, false});

            d.Add(Terminals.Char, new[]{false, false, false, false});
            d.Add(Terminals.Semicolon, new[]{false, false, false, false});
            d.Add(Terminals.LPar, new[]{false, true, false, true});
            d.Add(Terminals.RPar, new[]{false, false, false, false});

            d.Add(Terminals.Assign, new[]{false, false, false, false});
            d.Add(Terminals.LBrace, new[]{false, false, false, false});
            d.Add(Terminals.RBrace, new[]{false, false, false, false});
            d.Add(Terminals.Comma, new[]{false, false, false, false});

            d.Add(Terminals.Start, new[]{false, true, false, false});
            d.Add(Terminals.End, new[]{false, false, false, false});
            d.Add(Terminals.Return, new[]{false, true, false, false});
            d.Add(Terminals.Read, new[]{false, true, false, false});

            d.Add(Terminals.Write, new[]{false, true, false, false});
            d.Add(Terminals.Writeln, new[]{false, true, false, false});
            d.Add(Terminals.Break, new[]{false, true, false, false});
            d.Add(Terminals.If, new[]{false, true, false, false});

            d.Add(Terminals.Else, new[]{false, false, false, false});
            d.Add(Terminals.While, new[]{false, true, false, false});
            d.Add(Terminals.Minus, new[]{false, true, true, true});
            d.Add(Terminals.Inv, new[]{false, true, false, true});
            
            d.Add(Terminals.Plus, new[]{false, false, true, false});
            d.Add(Terminals.Mul, new[]{false, false, true, false});
            d.Add(Terminals.Div, new[]{false, false, true, false});
            d.Add(Terminals.Equal, new[]{false, false, true, false});
            
            d.Add(Terminals.NotEqual, new[]{false, false, true, false});
            d.Add(Terminals.Less, new[]{false, false, true, false});
            d.Add(Terminals.LessOrEqual, new[]{false, false, true, false});
            d.Add(Terminals.Great, new[]{false, false, true, false});
           
            d.Add(Terminals.GreatOrEqual, new[]{false, false, true, false});
            d.Add(Terminals.And, new[]{false, false, true, false});
            d.Add(Terminals.Or, new[]{false, false, true, false});
            //d.Add(_35, new[]{false, false, false});
            
            //d.Add(_36, new[]{false, false, false});4

            return d;
        }

        private bool StartOf(int s)
        {
            return _set[_la][s];
        }

        #endregion

        #region Errors

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

        #endregion

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

        private Symbol Expect(Symbol n)
        {
            Symbol r = null;
            if (_la == n)
            {
                r = _la;
                Get();
            }
            else
                SynErr(/*n*/0);
            return r;
        }

        private Program C__()
        {
            var vars = new List<VarDeclaration>();
            var funs = new List<FuncDeclaration>();

            while (IsDecl())
            {
                vars.Add(VarDecl());
            }
            while (_la == Terminals.Int || _la == Terminals.Char)
            {
                funs.Add(FunDecl());
            }
            return new Program(vars.ToArray(), funs.ToArray());
        }

        private VarDeclaration VarDecl()
        {
            var type = Type();
            var id = Expect(Terminals.Id) as Id;
            Num size = null;
            if (_la == Terminals.LBrace)
            {
                Get();
                size = Expect(Terminals.Num) as Num;
                Expect(Terminals.RBrace);
            }
            Expect(Terminals.Semicolon);

            if (size == null)
                return new VarDeclaration(type, id);
            return new ArrayDeclaration(type, id, size);
        }

        private FuncDeclaration FunDecl()
        {
            var rType = Type();
            var name = Expect(Terminals.Id) as Id;
            var parameters = new ParamDeclaration[0];
            Expect(Terminals.LPar);
            if (_la == Terminals.Int || _la == Terminals.Char)
            {
                parameters = ParamDecl();
            }
            Expect(Terminals.RPar);
            var body = Block();
            return new FuncDeclaration(rType, name, parameters, body);
        }

        private Terminal Type()
        {
            if (_la == Terminals.Int)
            {
                Get();
                return Terminals.Int as Terminal;
            }
            else if (_la == Terminals.Char)
            {
                Get();
                return Terminals.Char as Terminal;
            }
            else SynErr(36);
            return null;
        }

        private ParamDeclaration[] ParamDecl()
        {
            var parameters = new List<ParamDeclaration>();

            var type = Type();
            var id = Expect(Terminals.Id) as Id;
            bool isArray = false;
            if (_la == Terminals.LBrace)
            {
                Get();
                Expect(Terminals.RBrace);
                isArray = true;
            }
            parameters.Add(new ParamDeclaration(type, id, isArray));

            while (_la == Terminals.Comma)
            {
                Get();
                type = Type();
                id = Expect(Terminals.Id) as Id;
                if (_la == Terminals.LBrace)
                {
                    Get();
                    Expect(Terminals.RBrace);
                    isArray = true;
                }
                else
                    isArray = false;
                parameters.Add(new ParamDeclaration(type, id, isArray));
            }

            return parameters.ToArray();
        }

        private Block Block()
        {
            var vars = new List<VarDeclaration>();
            var stats = new List<Statement>();

            Expect(Terminals.Start);
            while (_la == Terminals.Int || _la == Terminals.Char)
            {
                vars.Add(VarDecl());
            }
            while (StartOf(1))
            {
                stats.Add(Smth());
            }
            Expect(Terminals.End);

            return new Block(vars.ToArray(), stats.ToArray());
        }

        private Statement Smth()
        {
            Statement st = null;
            if (_la == Terminals.Id || _la == Terminals.Num || _la == Terminals.LPar || _la == Terminals.Minus ||
                _la == Terminals.Inv)
            {
                st = Expr();
                Expect(Terminals.Semicolon);
            }
            else if (_la == Terminals.Return)
            {
                Get();
                var r = Expr();
                Expect(Terminals.Semicolon);
                st = new ReturnStatement(r);
            }
            else if (_la == Terminals.Read)
            {
                Get();
                var v = Expect(Terminals.Id) as Id;
                Expect(Terminals.Semicolon);
                st = new ReadStatement(v);
            }
            else if (_la == Terminals.Write)
            {
                Get();
                var r = Expr();
                Expect(Terminals.Semicolon);
                st = new WriteStatement(r);
            }
            else if (_la == Terminals.Writeln)
            {
                Get();
                Expect(Terminals.Semicolon);
                st = new WritelnStatement();
            }
            else if (_la == Terminals.Break)
            {
                Get();
                Expect(Terminals.Semicolon);
                st = new BreakStatement();
            }
            else if (_la == Terminals.If)
            {
                Get();
                Expect(Terminals.LPar);
                var c = Expr();
                Expect(Terminals.RPar);
                var t = Smth();
                Expect(Terminals.Else);
                var f = Smth();
                st = new IfStatement(c, t, f);
            }
            else if (_la == Terminals.While)
            {
                Get();
                Expect(Terminals.LPar);
                var c = Expr();
                Expect(Terminals.RPar);
                var s = Smth();
                st = new WhileStatement(c, s);
            }
            else if (_la == Terminals.Start)
            {
                st = Block();
            }
            else
                SynErr(37);
            return st;
        }

        private Expression Expr()
        {
            var ex = SimExpr();
            while (StartOf(2))
            {
                var op = BinaryOp();
                var ex2 = Expr();

                ex = new BinaryExpression(ex, op ,ex2);
            }
            return ex;
        }

        private Expression SimExpr()
        {
            Expression ex = null;
            if (_la == Terminals.Minus || _la == Terminals.Inv)
            {
                var op = UnaryOp();
                var e = Expr();
                ex = new UnaryExpression(op, e);
            }
            else if (_la == Terminals.LPar)
            {
                Get();
                ex = Expr();
                Expect(Terminals.RPar);
            }
            else if (_la == Terminals.Num)
            {
                var num = _la as Num;
                Get();
                ex = new NumberExpression(num);
            }
            else if (_la == Terminals.Id)
            {
                if (IsAssign())
                {
                    ex = Assign();
                }
                else
                {
                    ex = Access();
                }
            }
            else SynErr(38);
            return ex;
        }

        private Terminal BinaryOp()
        {
            Terminal r = null;
            if (_la == Terminals.Plus || _la == Terminals.Minus || _la == Terminals.Div || _la == Terminals.Mul || _la == Terminals.Equal ||
                _la == Terminals.NotEqual || _la == Terminals.Less || _la == Terminals.LessOrEqual || _la == Terminals.Great || 
                _la == Terminals.GreatOrEqual || _la == Terminals.And || _la == Terminals.Or)
            {
                r = _la as Terminal;
                Get();
            }
            else
                SynErr(39);
            return r;
        }

        private Terminal UnaryOp()
        {
            Terminal op = null;
            if (_la == Terminals.Minus || _la == Terminals.Inv)
            {
                op = _la as Terminal;
                Get();
            }
            else SynErr(40);
            return op;
        }

        private Expression Assign()
        {
            var id = Expect(Terminals.Id) as Id;
            Expression index = null;
            if (_la == Terminals.LBrace)
            {
                Get();
                index = Expr();
                Expect(Terminals.RBrace);
            }
            Expect(Terminals.Assign);
            var value = Expr();
            
            if (index == null)
                return new SetValueExpression(id, value);
            return new SetArrayValueExpression(id, value, index);
        }

        private Expression Access()
        {
            var id = Expect(Terminals.Id) as Id;
            if (_la == Terminals.LBrace || _la == Terminals.LPar)
            {
                if (_la == Terminals.LPar)
                {
                    var parameters = new List<Expression>();

                    Get();
                    if (StartOf(3))
                    {
                        var e = Expr();
                        parameters.Add(e);
                        while (_la == Terminals.Comma)
                        {
                            Get();
                            e = Expr();
                            parameters.Add(e);
                        }
                    }
                    Expect(Terminals.RPar);

                    return new FuncCallExpression(id, parameters.ToArray());
                }
                else
                {
                    Get();
                    var e = Expr();
                    Expect(Terminals.RBrace);

                    return new GetArrayValueExpression(id, e);
                }
            }
            return new GetValueExpression(id);
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
