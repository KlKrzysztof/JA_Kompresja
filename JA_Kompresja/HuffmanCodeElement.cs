using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JA_Kompresja
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct HuffmanCodeElement
    {
        public Int64 Word { get; set; }
        public char[] Code { get; set; }

        public HuffmanCodeElement(Int64 word, char[] code)
        {
            this.Word = word;
            this.Code = code;
        }

        public int codeLength() 
        { 
            if(Code == null) return 0;
            return Code.Length; 
        }
    }
}
