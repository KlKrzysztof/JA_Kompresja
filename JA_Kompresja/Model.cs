//Asembly Language - Project
//Krzyszkof Klecha
//section 11
//semester 5
//year 2024/25
//Huffman coding compresion
//version 0.1
//
//Class which manages compression

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JA_Kompresja
{
    abstract internal class Model
    {

        public abstract void Compress((string, string, bool, bool) settings);
        
    }

    class ModelAsm: Model
    {
        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_asem.dll")]
        static extern void doSth();
        override public void Compress((string, string, bool, bool) settings)
        {

        }
    }

    class ModelCpp : Model
    {
        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_cpp.dll")]
        static extern void doSth();
        override public void Compress((string, string, bool, bool) settings)
        {
            doSth();
        }
    }
}
