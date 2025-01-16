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
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JA_Kompresja
{

    //base class for Models
    abstract internal class Model
    {

        public abstract void Compress((string[], string) settings);
        
    }

    //model class which operate on asembly library
    class ModelAsm : Model
    {
        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_asem.dll")]
        static extern void countBytes(byte[] file, int arraySize, Int64[] countedBytes);
        //static extern void dllCompress(int a, int b);

        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_asem.dll")]
        static extern void makeSortedArray(Int64[] array, Int64[] sortedArray, int sortedArraylength);//function rewrites array in -- order without 0
        
        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_asem.dll")]
        unsafe static extern void makeHuffmanCodeTree(Int64[] sortedInputArray, int sizeOfInputArray, TreeNode* TreeOutputArray);

        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_asem.dll")]
        unsafe static extern void codeFile(/*HuffmanCodeElement[]*/char** huffmanCode, byte[] fileArray, byte[] codedArray, Int64 fileLength);

        //Compress
        //method which starts and menages compression
        //
        //params:
        //settings - a tuple with 2 strings which is (path to file, threads as string to know how many threads should be created)
        //
        //return value: void
        override public void Compress((string[], string) settings) //path, threadsString
        {
            int notNullBytes = 0;
            string[] paths = settings.Item1; //string representing path to file or folder
            string threadsString = settings.Item2; //string representing number of threads to start
            int threads = 0;//number of threads to start;
            byte[] fileArray = {10, 4, 12, 5, 10, 4, 10, 5, 15, 10, 0, 0};//File.ReadAllBytes(paths[0]); //file readed as bytes //TO DO: zrobić w pętli dla wszystkich plików//
            Int64[] countedBytesArray = new Int64[256];
            Int64[] sortedArray;
            TreeNode[] nodes;
            //HuffmanCodeElement[] huffmanCode;
            char[][] huffmanCode;
            byte[] codedArray;
            long codedArrayLength=0;

            try
            {
                threads = int.Parse(threadsString); //try to parse threadsString to number
            }
            catch (FormatException e)
            {
                threads = 1; //if cannot set default to 1
            }

            //TO DO: starting threads and delegate jobs to them

            //wczytanie pliku i przesłanie do biblioteki
            countBytes(fileArray, fileArray.Length, countedBytesArray);//pass file's bytes to library by array

            Console.WriteLine(countedBytesArray.ToString());

            foreach (Int64 counter in countedBytesArray)
            {
                if (counter > 0) ++notNullBytes;
            }

            sortedArray = new Int64[notNullBytes*2];

            makeSortedArray(countedBytesArray, sortedArray, sortedArray.Length);

            Console.WriteLine(sortedArray.ToString());

            nodes = new TreeNode[notNullBytes*2-1];

            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = new TreeNode();
            }

            //nodes = new Int64[notNullBytes * 8];
            unsafe
            {
                fixed (TreeNode* pNodes = nodes)
                {

                    makeHuffmanCodeTree(sortedArray, sortedArray.Length, pNodes);

                }
            }

            Console.WriteLine(nodes.ToString());

            huffmanCode = new char[256][];//new HuffmanCodeElement[256];

            TreeNodeIterator iter = new TreeNodeIterator(nodes.Last());

            {
                long nodeByte = 0;

                do
                {
                    nodeByte = iter.Current.NodeByte;

                    huffmanCode[nodeByte] = iter.getCode();//new HuffmanCodeElement(nodeByte, iter.getCode());

                } while (iter.MoveNext());
            }
            Console.WriteLine(huffmanCode.ToString());

            for(int i = 0; i < huffmanCode.Length; i++)
            {
                if (huffmanCode[i] != null)
                codedArrayLength += countedBytesArray[i] * huffmanCode[i].Length;
            }

            codedArray = new byte[codedArrayLength];

            unsafe
            {
                fixed (char** pHuffmanCode = new char*[huffmanCode.Length])
                {
                    for (int i = 0; i < huffmanCode.Length; i++)
                    {
                        fixed (char* pSubArray = huffmanCode[i])
                        {
                            pHuffmanCode[i] = pSubArray;
                        }
                    }

                    codeFile(pHuffmanCode, fileArray, codedArray, fileArray.Length);
                }
            }

            Console.WriteLine(codedArray.ToString());
        }
    }
    //model class which operate on c++ library

    class ModelCpp : Model
    {
        [StructLayout(LayoutKind.Sequential)]
        struct ByteCounter
        {
            int Byte;
            Int64 Counter;
        }

        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_cpp.dll")]
        unsafe static extern long* countBytes(byte[] byteTable, int arraySize);

        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_cpp.dll")]
        unsafe static extern IntPtr makeSortedArray(Int64* array, int sortedArraylength);

        //Compress
        //method which starts and menages compression
        //
        //params:
        //settings - a tuple with 2 strings which is (array of paths to file, threads as string to know how many threads should be created)
        //
        //return value: void
        override public void Compress((string[], string) settings)//paths, threadsString
        {
            string[] paths = settings.Item1; //string representing path to file or folder
            string threadsString = settings.Item2; //string representing number of threads to start
            int threads = 0;//number of threads to start;
            byte[] fileArray = { 10, 4, 12, 5, 10, 4, 10, 5, 15, 10, 0, 0 };
            IntPtr sortedBytes;

            try
            {
                threads = int.Parse(threadsString); //try to parse threadsString to number
            }
            catch (FormatException e)
            {
                threads = 1; //if cannot set default to 1
            }

            unsafe
            {
                Int64* countedBytes;

                countedBytes = countBytes(fileArray, fileArray.Length);

                sortedBytes = makeSortedArray(countedBytes, 256);
            }

            //Console.WriteLine(sortedBytes.ToString());
        }
    }
}
