using System;
using System.Collections.Generic;
using System.IO;
using IronC__Common.Grammars;
using IronC__Common.Lexis;

namespace IronC__Common
{
    public class Reader
    {
        private readonly string _filePath;
        private StreamReader _reader;
        private StreamWriter _writer;

        public Reader(string path)
        {
            _filePath = path;
        }

        public Grammar ReadGrammar()
        {
            try
            {
                var grammar = new Grammar();
                _reader = new StreamReader(_filePath);

                grammar.SetNotTerminals(ReadNotTerminals());
                grammar.SetTerminals(ReadTerminals());
                grammar.SetRules(ReadRules(grammar));
                grammar.SetStart(ReadStart());

                _reader.Close();
                return grammar;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #region Read

        private List<NotTerminal> ReadNotTerminals()
        {
            var ret = new List<NotTerminal>();
            int count = int.Parse(_reader.ReadLine());

            for (int i = 0; i < count; i++)
            {
                ret.Add(new NotTerminal(_reader.ReadLine()));
            }
            return ret;
        }

        private List<Terminal> ReadTerminals()
        {
            var ret = new List<Terminal>();
            int count = int.Parse(_reader.ReadLine());

            for (int i = 0; i < count; i++)
            {
                string str = _reader.ReadLine();
                switch (str)
                {
                    case "letter":
                        ret.Add(new Id());
                        break;
                    case "number":
                        ret.Add(new Num());
                        break;
                    default:
                        ret.Add(new Terminal(str));
                        break;
                }                
            }
            return ret;
        }

        private List<Rule> ReadRules(Grammar grammar)
        {
            var rules = new List<Rule>();
            int count = int.Parse(_reader.ReadLine());

            for (int i = 0; i < count; i++)
            {
                string str = _reader.ReadLine();
                rules.Add(CreateRule(grammar, str));
            }
            return rules;
        }

        private NotTerminal ReadStart()
        {
            return new NotTerminal(_reader.ReadLine());
        }

        private Rule CreateRule(Grammar grammar, string str)
        {
            Rule rule = null;
            string res = "";
            bool flag = false;
            foreach (var c in str)
            {
                if (c != ' ')
                    res += c;
                else
                {
                    if (!flag)
                    {
                        rule = new Rule(new NotTerminal(res));
                        flag = true;
                    }
                    else
                        rule.AddRightPart(grammar.GetSymbol(res));
                    res = "";
                }
            }
            rule.AddRightPart(grammar.GetSymbol(res));
            return rule;
        }

        #endregion
    }
}

