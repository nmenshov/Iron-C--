﻿using System.Collections.Generic;

namespace IronC__Common.Lexis
{
    public class Terminals
    {
        public static readonly Symbol Id = new Id();                    //1
        public static readonly Symbol Num = new Num();                  //2
        public static readonly Symbol Int = new Terminal("int");        //3
        public static readonly Symbol Char = new Terminal("char");      //4
        public static readonly Symbol Semicolon = new Terminal(";");    //5
        public static readonly Symbol LPar = new Terminal("(");         //6
        public static readonly Symbol RPar = new Terminal(")");         //7
        public static readonly Symbol Assign = new Terminal("=");       //8
        public static readonly Symbol LBrace = new Terminal("[");       //9
        public static readonly Symbol RBrace = new Terminal("]");       //10
        public static readonly Symbol Comma = new Terminal(",");        //11
        public static readonly Symbol Start = new Terminal("{");        //12
        public static readonly Symbol End = new Terminal("}");          //13
        public static readonly Symbol Return = new Terminal("return");  //14
        public static readonly Symbol Read = new Terminal("read");      //15
        public static readonly Symbol Write = new Terminal("write");    //16
        public static readonly Symbol Writeln = new Terminal("writeln");//17
        public static readonly Symbol Break = new Terminal("break");    //18
        public static readonly Symbol If = new Terminal("if");          //19
        public static readonly Symbol Else = new Terminal("else");      //20
        public static readonly Symbol While = new Terminal("while");    //21
        public static readonly Symbol Minus = new Terminal("-");        //22
        public static readonly Symbol Inv = new Terminal("!");          //23
        public static readonly Symbol Plus = new Terminal("+");         //24
        public static readonly Symbol Mul = new Terminal("*");          //25
        public static readonly Symbol Div = new Terminal("/");          //26
        public static readonly Symbol Equal = new Terminal("==");       //27
        public static readonly Symbol NotEqual = new Terminal("!=");    //28
        public static readonly Symbol Less = new Terminal("<");         //29
        public static readonly Symbol LessOrEqual = new Terminal("<="); //30
        public static readonly Symbol Great = new Terminal(">");        //31
        public static readonly Symbol GreatOrEqual = new Terminal(">=");//32
        public static readonly Symbol And = new Terminal("&&");         //33
        public static readonly Symbol Or = new Terminal("||");          //34
        public static readonly Symbol EOF = new EndSymbol();            //0

        public static readonly Dictionary<Symbol, int> OrderNums;

        static Terminals()
        {
            OrderNums = new Dictionary<Symbol, int>
                {
                    {EOF, 0},
                    {Id, 1},
                    {Num, 2},
                    {Int, 3},
                    {Char, 4},
                    {Semicolon, 5},
                    {LPar, 6},
                    {RPar, 7},
                    {Assign, 8},
                    {LBrace, 9},
                    {RBrace, 10},
                    {Comma, 11},
                    {Start, 12},
                    {End, 13},
                    {Return, 14},
                    {Read, 15},
                    {Write, 16},
                    {Writeln, 17},
                    {Break, 18},
                    {If, 19},
                    {Else, 20},
                    {While, 21},
                    {Minus, 22},
                    {Inv, 23},
                    {Plus, 24},
                    {Mul, 25},
                    {Div, 26},
                    {Equal, 27},
                    {NotEqual, 28},
                    {Less, 29},
                    {LessOrEqual, 30},
                    {Great, 31},
                    {GreatOrEqual, 32},
                    {And, 33},
                    {Or, 34}
                };
        }
    }
}