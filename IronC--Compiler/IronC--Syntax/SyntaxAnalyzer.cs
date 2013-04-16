using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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

        private static Symbol _eof = new EndSymbol();
        private static Symbol _id = new Id();
        private static Symbol _num = new Num();
        private static Symbol _int = new Terminal("int");
        private static Symbol _char = new Terminal("char");
        private static Symbol _semicolon = new Terminal(";");
        private static Symbol _lpar = new Terminal("(");
        private static Symbol _rpar = new Terminal(")");
        private static Symbol _assign = new Terminal("=");
        private static Symbol _lbrace = new Terminal("[");
        private static Symbol _rbrace = new Terminal("]");
        private const int maxT = 35;

        public Errors errors;
        const int minErrDist = 2;
        int errDist = minErrDist;

        private Symbol _la;
        private Symbol _t;

        public SyntaxAnalyzer(IList<Symbol> input)
        {
            _scanner = new Scanner(input);
        }

        public void Analyze()
        {
        }

        #region Conflict resovlers

        bool IsDecl()
        {
            var id = _scanner.Peek();
            var sc = _scanner.Peek();
            if ((_la == _int || _la == _char) && id == _id && sc == _semicolon)
                return true;
            if ((_la == _int || _la == _char) && id == _id && sc == _lpar)
                return false;
            return true;
        }

        bool IsAssign()
        {
            var next = _scanner.Peek();
            if (next == _assign)
                return true;
            if (next != _lbrace)
                return false;
            int braces = 1;
            while (braces != 0)
            {
                next = _scanner.Peek();
                if (next == _lbrace)
                    braces++;
                else if (next == _rbrace)
                    braces--;
            }
            next = _scanner.Peek();
            return next == _assign;
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
            for (; ; )
            {
                _t = _la;
                _la = _scanner.Scan();
                if (true) { ++errDist; break; }
                _la = _t;
            }
        }

        void Expect(Symbol n)
        {
            if (_la == n) Get(); else { SynErr(n); }
        }

        bool StartOf(int s)
        {
            return set[s, _la];
        }
    }

    public class Parser
    {
        

        const bool T = true;
        const bool x = false;
        

        public Scanner scanner;
        

        public Token t;    // last recognized token
        public Token la;   // lookahead token
        

        const int // types
              undef = 0, integer = 1, boolean = 2;

        const int // object kinds
          var = 0, proc = 1;

        

        /*--------------------------------------------------------------------------*/


        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
        }

        

        

        

        

        void ExpectWeak(int n, int follow)
        {
            if (la.kind == n) Get();
            else
            {
                SynErr(n);
                while (!StartOf(follow)) Get();
            }
        }


        bool WeakSeparator(int n, int syFol, int repFol)
        {
            int kind = la.kind;
            if (kind == n) { Get(); return true; }
            else if (StartOf(repFol)) { return false; }
            else
            {
                SynErr(n);
                while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind]))
                {
                    Get();
                    kind = la.kind;
                }
                return StartOf(syFol);
            }
        }


        void C__()
        {
            while (IsDecl())
            {
                VarDecl();
            }
            while (la.kind == 3 || la.kind == 4)
            {
                FunDecl();
            }
        }

        void VarDecl()
        {
            Type();
            Expect(1);
            if (la.kind == 9)
            {
                Get();
                Expect(2);
                Expect(10);
            }
            Expect(5);
        }

        void FunDecl()
        {
            Type();
            Expect(1);
            Expect(6);
            if (la.kind == 3 || la.kind == 4)
            {
                ParamDecl();
            }
            Expect(7);
            Block();
        }

        void Type()
        {
            if (la.kind == 3)
            {
                Get();
            }
            else if (la.kind == 4)
            {
                Get();
            }
            else SynErr(36);
        }

        void ParamDecl()
        {
            Type();
            Expect(1);
            if (la.kind == 9)
            {
                Get();
                Expect(10);
            }
            while (la.kind == 11)
            {
                Get();
                Type();
                Expect(1);
                if (la.kind == 9)
                {
                    Get();
                    Expect(10);
                }
            }
        }

        void Block()
        {
            Expect(12);
            while (la.kind == 3 || la.kind == 4)
            {
                VarDecl();
            }
            while (StartOf(1))
            {
                Smth();
            }
            Expect(13);
        }

        void Smth()
        {
            switch (la.kind)
            {
                case 1:
                case 2:
                case 6:
                case 22:
                case 23:
                    {
                        Expr();
                        Expect(5);
                        break;
                    }
                case 14:
                    {
                        Get();
                        Expr();
                        Expect(5);
                        break;
                    }
                case 15:
                    {
                        Get();
                        Expect(1);
                        Expect(5);
                        break;
                    }
                case 16:
                    {
                        Get();
                        Expr();
                        Expect(5);
                        break;
                    }
                case 17:
                    {
                        Get();
                        Expect(5);
                        break;
                    }
                case 18:
                    {
                        Get();
                        Expect(5);
                        break;
                    }
                case 19:
                    {
                        Get();
                        Expect(6);
                        Expr();
                        Expect(7);
                        Smth();
                        Expect(20);
                        Smth();
                        break;
                    }
                case 21:
                    {
                        Get();
                        Expect(6);
                        Expr();
                        Expect(7);
                        Smth();
                        break;
                    }
                case 12:
                    {
                        Block();
                        break;
                    }
                default: SynErr(37); break;
            }
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
            if (la.kind == 22 || la.kind == 23)
            {
                UnaryOp();
                Expr();
            }
            else if (la.kind == 6)
            {
                Get();
                Expr();
                Expect(7);
            }
            else if (la.kind == 2)
            {
                Get();
            }
            else if (la.kind == 1)
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

        void BinaryOp()
        {
            switch (la.kind)
            {
                case 24:
                    {
                        Get();
                        break;
                    }
                case 22:
                    {
                        Get();
                        break;
                    }
                case 25:
                    {
                        Get();
                        break;
                    }
                case 26:
                    {
                        Get();
                        break;
                    }
                case 27:
                    {
                        Get();
                        break;
                    }
                case 28:
                    {
                        Get();
                        break;
                    }
                case 29:
                    {
                        Get();
                        break;
                    }
                case 30:
                    {
                        Get();
                        break;
                    }
                case 31:
                    {
                        Get();
                        break;
                    }
                case 32:
                    {
                        Get();
                        break;
                    }
                case 33:
                    {
                        Get();
                        break;
                    }
                case 34:
                    {
                        Get();
                        break;
                    }
                default: SynErr(39); break;
            }
        }

        void UnaryOp()
        {
            if (la.kind == 22)
            {
                Get();
            }
            else if (la.kind == 23)
            {
                Get();
            }
            else SynErr(40);
        }

        void Assign()
        {
            Expect(1);
            if (la.kind == 9)
            {
                Get();
                Expr();
                Expect(10);
            }
            Expect(8);
            Expr();
        }

        void Access()
        {
            Expect(1);
            if (la.kind == 6 || la.kind == 9)
            {
                if (la.kind == 6)
                {
                    Get();
                    Expr();
                    Expect(7);
                }
                else
                {
                    Get();
                    Expr();
                    Expect(10);
                }
            }
        }



        public void Parse()
        {
            la = new Token();
            la.val = "";
            Get();
            C__();
            Expect(0);

        }

        static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,T,T,x, x,x,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,T,T, T,T,T,T, T,T,T,x, x}

	};
    } // end Parser


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


    public class FatalError : Exception
    {
        public FatalError(string m) : base(m) { }
    }


    public class Token
    {
        public int kind;    // token kind
        public int pos;     // token position in bytes in the source text (starting at 0)
        public int charPos;  // token position in characters in the source text (starting at 0)
        public int col;     // token column (starting at 1)
        public int line;    // token line (starting at 1)
        public string val;  // token value
        public Token next;  // ML 2005-03-11 Tokens are kept in linked list
    }

    //-----------------------------------------------------------------------------------
    // Buffer
    //-----------------------------------------------------------------------------------
    public class Buffer
    {
        // This Buffer supports the following cases:
        // 1) seekable stream (file)
        //    a) whole stream in buffer
        //    b) part of stream in buffer
        // 2) non seekable stream (network, console)

        public const int EOF = char.MaxValue + 1;
        const int MIN_BUFFER_LENGTH = 1024; // 1KB
        const int MAX_BUFFER_LENGTH = MIN_BUFFER_LENGTH * 64; // 64KB
        byte[] buf;         // input buffer
        int bufStart;       // position of first byte in buffer relative to input stream
        int bufLen;         // length of buffer
        int fileLen;        // length of input stream (may change if the stream is no file)
        int bufPos;         // current position in buffer
        Stream stream;      // input stream (seekable)
        bool isUserStream;  // was the stream opened by the user?

        public Buffer(Stream s, bool isUserStream)
        {
            stream = s; this.isUserStream = isUserStream;

            if (stream.CanSeek)
            {
                fileLen = (int)stream.Length;
                bufLen = Math.Min(fileLen, MAX_BUFFER_LENGTH);
                bufStart = Int32.MaxValue; // nothing in the buffer so far
            }
            else
            {
                fileLen = bufLen = bufStart = 0;
            }

            buf = new byte[(bufLen > 0) ? bufLen : MIN_BUFFER_LENGTH];
            if (fileLen > 0) Pos = 0; // setup buffer to position 0 (start)
            else bufPos = 0; // index 0 is already after the file, thus Pos = 0 is invalid
            if (bufLen == fileLen && stream.CanSeek) Close();
        }

        protected Buffer(Buffer b)
        { // called in UTF8Buffer constructor
            buf = b.buf;
            bufStart = b.bufStart;
            bufLen = b.bufLen;
            fileLen = b.fileLen;
            bufPos = b.bufPos;
            stream = b.stream;
            // keep destructor from closing the stream
            b.stream = null;
            isUserStream = b.isUserStream;
        }

        ~Buffer() { Close(); }

        protected void Close()
        {
            if (!isUserStream && stream != null)
            {
                stream.Close();
                stream = null;
            }
        }

        public virtual int Read()
        {
            if (bufPos < bufLen)
            {
                return buf[bufPos++];
            }
            else if (Pos < fileLen)
            {
                Pos = Pos; // shift buffer start to Pos
                return buf[bufPos++];
            }
            else if (stream != null && !stream.CanSeek && ReadNextStreamChunk() > 0)
            {
                return buf[bufPos++];
            }
            else
            {
                return EOF;
            }
        }

        public int Peek()
        {
            int curPos = Pos;
            int ch = Read();
            Pos = curPos;
            return ch;
        }

        // beg .. begin, zero-based, inclusive, in byte
        // end .. end, zero-based, exclusive, in byte
        public string GetString(int beg, int end)
        {
            int len = 0;
            char[] buf = new char[end - beg];
            int oldPos = Pos;
            Pos = beg;
            while (Pos < end) buf[len++] = (char)Read();
            Pos = oldPos;
            return new String(buf, 0, len);
        }

        public int Pos
        {
            get { return bufPos + bufStart; }
            set
            {
                if (value >= fileLen && stream != null && !stream.CanSeek)
                {
                    // Wanted position is after buffer and the stream
                    // is not seek-able e.g. network or console,
                    // thus we have to read the stream manually till
                    // the wanted position is in sight.
                    while (value >= fileLen && ReadNextStreamChunk() > 0) ;
                }

                if (value < 0 || value > fileLen)
                {
                    throw new FatalError("buffer out of bounds access, position: " + value);
                }

                if (value >= bufStart && value < bufStart + bufLen)
                { // already in buffer
                    bufPos = value - bufStart;
                }
                else if (stream != null)
                { // must be swapped in
                    stream.Seek(value, SeekOrigin.Begin);
                    bufLen = stream.Read(buf, 0, buf.Length);
                    bufStart = value; bufPos = 0;
                }
                else
                {
                    // set the position to the end of the file, Pos will return fileLen.
                    bufPos = fileLen - bufStart;
                }
            }
        }

        // Read the next chunk of bytes from the stream, increases the buffer
        // if needed and updates the fields fileLen and bufLen.
        // Returns the number of bytes read.
        private int ReadNextStreamChunk()
        {
            int free = buf.Length - bufLen;
            if (free == 0)
            {
                // in the case of a growing input stream
                // we can neither seek in the stream, nor can we
                // foresee the maximum length, thus we must adapt
                // the buffer size on demand.
                byte[] newBuf = new byte[bufLen * 2];
                Array.Copy(buf, newBuf, bufLen);
                buf = newBuf;
                free = bufLen;
            }
            int read = stream.Read(buf, bufLen, free);
            if (read > 0)
            {
                fileLen = bufLen = (bufLen + read);
                return read;
            }
            // end of stream reached
            return 0;
        }
    }

    //-----------------------------------------------------------------------------------
    // UTF8Buffer
    //-----------------------------------------------------------------------------------
    public class UTF8Buffer : Buffer
    {
        public UTF8Buffer(Buffer b) : base(b) { }

        public override int Read()
        {
            int ch;
            do
            {
                ch = base.Read();
                // until we find a utf8 start (0xxxxxxx or 11xxxxxx)
            } while ((ch >= 128) && ((ch & 0xC0) != 0xC0) && (ch != EOF));
            if (ch < 128 || ch == EOF)
            {
                // nothing to do, first 127 chars are the same in ascii and utf8
                // 0xxxxxxx or end of file character
            }
            else if ((ch & 0xF0) == 0xF0)
            {
                // 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
                int c1 = ch & 0x07; ch = base.Read();
                int c2 = ch & 0x3F; ch = base.Read();
                int c3 = ch & 0x3F; ch = base.Read();
                int c4 = ch & 0x3F;
                ch = (((((c1 << 6) | c2) << 6) | c3) << 6) | c4;
            }
            else if ((ch & 0xE0) == 0xE0)
            {
                // 1110xxxx 10xxxxxx 10xxxxxx
                int c1 = ch & 0x0F; ch = base.Read();
                int c2 = ch & 0x3F; ch = base.Read();
                int c3 = ch & 0x3F;
                ch = (((c1 << 6) | c2) << 6) | c3;
            }
            else if ((ch & 0xC0) == 0xC0)
            {
                // 110xxxxx 10xxxxxx
                int c1 = ch & 0x1F; ch = base.Read();
                int c2 = ch & 0x3F;
                ch = (c1 << 6) | c2;
            }
            return ch;
        }
    }
}
