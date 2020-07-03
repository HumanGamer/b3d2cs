using System;
using System.Collections.Generic;
using System.Text;

namespace Blitz3DToCSharp
{
    public class Tokenizer
    {
        private string _string;
        private int _index;

        public Tokenizer(string s)
        {
            _string = s;
        }

        public char Next()
        {
            return _string[_index++];
        }

        public bool HasNext()
        {
            return _index < _string.Length;
        }
    }
}
