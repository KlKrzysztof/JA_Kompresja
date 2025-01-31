//Asembly Language - Project
//Krzyszkof Klecha
//section 11
//semester 5
//year 2024/25
//Huffman coding compresion
//version 0.6
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
using System.Xml.Linq;

namespace JA_Kompresja
{

    //base class for Models
    abstract internal class Model
    {

        public abstract Tuple<byte[], char[][], long> Compress(string path);

        public abstract byte[] Decompress(byte[] code, TreeNode[] HuffmanTree, long fileLength);
        
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
        override public Tuple<byte[], char[][], long> Compress(string path) //path, threadsString
        {
            int notNullBytes = 0;
            int threads = 0;//number of threads to start;
            byte[] fileArray = {10, 4, 12, 5, 10, 4, 10, 5, 15, 10, 0, 0};//File.ReadAllBytes(path); //file readed as bytes 
            Int64[] countedBytesArray = new Int64[256];
            Int64[] sortedArray;
            TreeNode[] nodes;
            //HuffmanCodeElement[] huffmanCode;
            char[][] huffmanCode;
            byte[] codedArray;
            long codedArrayLength=0;


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

            codedArray = new byte[(int)Math.Ceiling(((double)codedArrayLength / 8.0))];

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

            return new Tuple<byte[], char[][], long>( codedArray, huffmanCode, fileArray.LongLength);
        }

        public override byte[] Decompress(byte[] code, TreeNode[] HuffmanTree, long fileLength)
        {

            return null;
        }
    }
    //model class which operate on c++ library

    class ModelCpp : Model
    {
        [StructLayout(LayoutKind.Sequential)]
        struct ByteCounter
        {
            public int Byte { get; set; }
            public Int64 Counter { get; set; }
        }

        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_cpp.dll")]
        unsafe static extern long* countBytes(byte* byteTable, int arraySize, long* ptr);

        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_cpp.dll")]
        unsafe static extern IntPtr makeSortedArray(Int64* array, int sortedArraylength, ByteCounter* countersArray);

        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_cpp.dll")]
        unsafe static extern IntPtr makeHuffmanCodeTree(ByteCounter* sortedInputArray, int sizeOfInputArray, TreeNode* huffmanTree);

        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_cpp.dll")]
        unsafe static extern void codeFile(char** huffmanCode, byte* fileArray, byte* codedArray, long fileLength);

        unsafe static extern void decodeFile(TreeNode* treeRoot, byte* fileArray, byte* codedArray, long fileLength);

        //Compress
        //method which starts and menages compression
        //
        //params:
        //settings - a tuple with 2 strings which is (array of paths to file, threads as string to know how many threads should be created)
        //
        //return value: void
        override public Tuple<byte[], char[][], long> Compress(string path)//paths, threadsString
        {
            int threads = 0;//number of threads to start;
            byte[] fileArray = { 10, 4, 12, 5, 10, 4, 10, 5, 15, 10, 0, 0 };
            byte[] codedArray;
            long codedArrayLength = 0;
            int notNullBytes = 0;
            ByteCounter[] byteCounter;
            TreeNode[] nodesArray;
            char[][] huffmanCode;
            long[] countedBytes;

            unsafe
            {

                countedBytes = new long[256];

                fixed (byte* fileArrayPtr = fileArray)
                fixed (long* countedBytesPtr = countedBytes)
                {
                    countBytes(fileArrayPtr, fileArray.Length, countedBytesPtr);

                    for (int i = 0; i < 256; ++i)
                    {
                        if (countedBytes[i] != 0)
                        {
                            ++notNullBytes;
                        }
                    }
                }
                byteCounter = new ByteCounter[notNullBytes];

                fixed (ByteCounter* byteCounterPtr = byteCounter)
                {
                    fixed (long* xPtr = countedBytes)
                    {
                        makeSortedArray(xPtr, notNullBytes, byteCounterPtr);
                    }
                    nodesArray = new TreeNode[notNullBytes*2-1];

                    fixed (TreeNode* nodesArrayPtr = nodesArray)
                    {
                        makeHuffmanCodeTree(byteCounterPtr, notNullBytes, nodesArrayPtr);   
                    }
                }
            }

            huffmanCode = new char[256][];

            TreeNodeIterator iter = new TreeNodeIterator(nodesArray.Last());

            {
                long nodeByte = 0;

                do
                {
                    nodeByte = iter.Current.NodeByte;

                    huffmanCode[nodeByte] = iter.getCode();//new HuffmanCodeElement(nodeByte, iter.getCode());

                } while (iter.MoveNext());
            }

            for (int i = 0; i < huffmanCode.Length; i++)
            {
                if (huffmanCode[i] != null)
                    codedArrayLength += countedBytes[i] * huffmanCode[i].Length;
            }

            codedArray = new byte[(int)Math.Ceiling(((double)codedArrayLength / 8.0))];

            unsafe
            {
                fixed (char** pHuffmanCode = new char*[huffmanCode.Length])
                fixed(byte* fileArrayPtr = fileArray)
                fixed(byte* codedArrayPtr = codedArray)
                {
                    for (int i = 0; i < huffmanCode.Length; i++)
                    {
                        fixed (char* pSubArray = huffmanCode[i])
                        {
                            pHuffmanCode[i] = pSubArray;
                        }
                    }

                    codeFile(pHuffmanCode, fileArrayPtr, codedArrayPtr, fileArray.Length);
                }
            }

            return new Tuple<byte[], char[][], long>(codedArray, huffmanCode, fileArray.LongLength);
            //Console.WriteLine(sortedBytes.ToString());
        }

        override public byte[] Decompress(byte[] code, TreeNode[] HuffmanTree, long fileLength)
        {

            return null;
        }
    }
}
