using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IronC__Common;
using IronC__Common.Grammars;
using IronC__Common.Lexis;

namespace IronC__Lexical
{
    public class LexicalAnalyzer
    {
        private readonly Grammar _grammar;
        private readonly string _output;
        private int _rowNumber;

        public LexicalAnalyzer(Grammar grammar, string output)
        {
            _grammar = grammar;
            _output = output;
        }

        public List<Symbol> Convert(string input)
        {
            Contract.Requires(input != null);
            Contract.Requires(_grammar != null);

            var ret = new List<Symbol>();

            try
            {
                _rowNumber = 1;
                string rab = "";
                string str = input.TrimStart(' ')+" ";

                str = Filter(str);

                foreach (var symbol in str)
                {
                    rab = Parse(rab, symbol, ret);
                }

                rab = Parse(rab, '.', ret);
            }
            catch (Exception e)
            {
                return null;
            }

            Serialize(ret);

            return ret;
        }

        #region Parse

        private string Parse(string rab, char symbol, List<Symbol> ret)
        {
            if (symbol == '\r' || symbol == '\t')
                return rab;
            if (symbol == '\n')
            {
                _rowNumber++;
                return rab;
            }

            if (symbol == ' ' && rab != "" && IsLetterOrDigit(rab))
            {
                ret.Add(GetSymbol(rab, ret));
                return "";
            }

            if (symbol == ' ')
                return rab;

            if (!IsLetterOrDigit(symbol) && IsLetterOrDigit(rab) && rab.Length > 0)
            {
                ret.Add(GetSymbol(rab, ret));
                rab = "";
            }
            if (IsLetterOrDigit(symbol) && !IsLetterOrDigit(rab) && rab.Length > 0)
            {
                ret.Add(GetSymbol(rab, ret));
                rab = "";
            }
            else if (!IsLetterOrDigit(symbol) && !IsLetterOrDigit(rab) && rab.Length > 0)
            {
                var value = GetSymbol(rab + symbol, ret);

                if (value != null)
                {
                    ret.Add(value);
                    return "";
                }
                else
                {
                    ret.Add(GetSymbol(rab, ret));
                    rab = "";
                }
            }

            if (symbol != ' ')
                rab += symbol;

            return rab;
        }

        private bool IsLetterOrDigit(char c)
        {
            return c=='_' || char.IsLetterOrDigit(c);
        }

        private bool IsLetterOrDigit(string str)
        {
            return str.Aggregate(true, (current, c) => current && IsLetterOrDigit(c));
        }

        private string Filter(string str)
        {
            var b = new StringBuilder();
            foreach (var c in str)
            {
                if (c == '_')
                {
                    b.Append(" " + c);
                }
                else if (!char.IsLetterOrDigit(c))
                {
                    b.Append(" " + c + " ");
                }
                else
                {
                    b.Append(c);
                }
            }
            return b.ToString();
        }

        public Symbol GetSymbol(string str, List<Symbol> current)
        {
            
            var element = _grammar.Terminals.Find(x => x.IsEqual(str));
            if (element == null)
                return null;

            var ret = element.GetCopy(str);
            ret.SetRowNumber(_rowNumber);
            return ret;
        }

        #endregion

        private void Serialize(List<Symbol> tokens)
        {
            var list = new[]
                {
                    typeof(Terminal),
                    typeof(NotTerminal),
                    typeof(Num),
                    typeof(Id),
                    typeof(Epsilon),
                    typeof(EndSymbol),
                    typeof(Symbol),
                };
            var serializer = new XmlSerializer(tokens.GetType(), list);
            var writer = new StreamWriter(_output);
            serializer.Serialize(writer,tokens);
            writer.Flush();
            writer.Close();
        }
    }
}
