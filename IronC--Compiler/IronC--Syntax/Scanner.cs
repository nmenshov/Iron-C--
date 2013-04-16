using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronC__Common.Lexis;

namespace IronC__Syntax
{
    public class Scanner
    {
        private readonly IList<Symbol> _buffer;
        private int _position;
        private int _peekPos;

        public Scanner(IList<Symbol> tokens)
        {
            _buffer = tokens;
            _position = 0;
            _peekPos = 0;
        }

        public Symbol Scan()
        {
            if (_buffer.Count == _position)
                return null;
            return _buffer[_position++];
        }

        public Symbol Peek()
        {
            if (_peekPos < _position)
                _peekPos = _position;
            if (_peekPos == _buffer.Count)
                return null;
            return _buffer[_peekPos++];
        }

        public void ResetPeek()
        {
            _peekPos = _position;
        }

    }
}
