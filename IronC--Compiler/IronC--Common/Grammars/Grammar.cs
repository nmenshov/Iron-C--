using System.Collections.Generic;
using IronC__Common.Lexis;

namespace IronC__Common.Grammars
{
    public class Grammar
    {
        public NotTerminal StartSymbol { get; private set; }
        public List<Terminal> Terminals { get; private set; }
        public List<NotTerminal> NotTerminals { get; private set; }
        //список правил

        public Grammar(NotTerminal startSymbol, List<Terminal> terminals, List<NotTerminal> notTerminals)
        {
            StartSymbol = startSymbol;
            Terminals = terminals;
            NotTerminals = notTerminals;
        }

        public Grammar()
        {           
            Terminals = new List<Terminal>();
            NotTerminals = new List<NotTerminal>();
        }

        #region Create

        public void SetNotTerminals(List<NotTerminal> nt)
        {
            NotTerminals = nt;
        }

        public void SetTerminals(List<Terminal> t)
        {
            Terminals = t;
        }

        public void SetStart(NotTerminal nt)
        {
            StartSymbol = nt;
        }

        public void SetRules(List<Rule> readRules)
        {
            //TODO тут храни как хочешь
        }

        public Symbol GetSymbol(string res)
        {
            Symbol nt = NotTerminals.Find(x => x.Name == res);
            if (nt != null)
                return nt;

            nt = Terminals.Find(x => x.Name == res);
            if (nt != null)
                return nt;

            if(res == Constants.EpsilonName)
                return new Epsilon();

            if (res == Constants.EndSymbolName)
                return new EndSymbol();

            return null;
        }

        #endregion        
    }
}
