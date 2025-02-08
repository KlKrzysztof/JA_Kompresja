//Asembly Language - Project
//Krzyszkof Klecha
//section 11
//semester 5
//year 2024/25
//Huffman coding compresion
//version 1.1
//
//Class which manages compression

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
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
        [DllImport(@"../../../../../x64/Debug/Huffman_asem.dll")]
        unsafe static extern void countBytes(byte* file, int arraySize, Int64* countedBytes);
        //static extern void dllCompress(int a, int b);

        [DllImport(@"../../../../../x64/Debug/Huffman_asem.dll")]
        unsafe static extern void makeSortedArray(Int64* array, Int64* sortedArray, int sortedArraylength);//function rewrites array in -- order without 0
        
        [DllImport(@"../../../../../x64/Debug/Huffman_asem.dll")]
        unsafe static extern void makeHuffmanCodeTree(Int64* sortedInputArray, int sizeOfInputArray, TreeNode* TreeOutputArray);

        [DllImport(@"../../../../../x64/Debug/Huffman_asem.dll")]
        unsafe static extern void codeFile1(/*HuffmanCodeElement[]*/char** huffmanCode, byte* fileArray, byte* codedArray, Int64 fileLength);

        //Compress
        //method which starts and menages compression
        //
        //params:
        //settings - a tuple with 2 strings which is (path to file, threads as string to know how many threads should be created)
        //
        //return value: void
        override public Tuple<byte[], char[][], long> Compress(string path) //path, threadsString
        {
            if (path == null) return null;

            int notNullBytes = 0;
            int threads = 0;//number of threads to start;
            byte[] fileArray = File.ReadAllBytes(path); //file readed as bytes 
            Int64[] countedBytesArray = new Int64[256];
            Int64[] sortedArray;
            TreeNode[] nodes;
            //HuffmanCodeElement[] huffmanCode;
            char[][] huffmanCode;
            byte[] codedArray;
            long codedArrayLength=0;
            long[] CBA_copy = new long[256];

            unsafe
            {
                fixed (byte* fileArrayPtr = fileArray)
                fixed (long* countedBytesArrayPtr = countedBytesArray)
                {
                    Debug.Assert(countedBytesArrayPtr != null);
                    Debug.Assert(fileArrayPtr != null);
                    //wczytanie pliku i przesłanie do biblioteki
                    countBytes(fileArrayPtr, fileArray.Length, countedBytesArrayPtr);//pass file's bytes to library by array
                }
            }

            Console.WriteLine(countedBytesArray.ToString());

            foreach (Int64 counter in countedBytesArray)
            {
                if (counter > 0) ++notNullBytes;
            }

            sortedArray = new Int64[notNullBytes*2];

            
            Array.Copy(countedBytesArray, CBA_copy, countedBytesArray.Length);      //save data form the array

            unsafe
            {
                fixed (long* countedBytesArrayPtr = countedBytesArray)
                fixed (long* sortedArrayPtr = sortedArray)
                {
                    Debug.Assert(countedBytesArrayPtr != null);
                    Debug.Assert(sortedArrayPtr != null);
                    makeSortedArray(countedBytesArrayPtr, sortedArrayPtr, sortedArray.Length);
                }
            }

            nodes = new TreeNode[notNullBytes*2-1];

            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = new TreeNode();
            }

            //nodes = new Int64[notNullBytes * 8];
            unsafe
            {
                fixed(long* sortedArrayPtr =  sortedArray) 
                fixed (TreeNode* pNodes = nodes)
                {
                    //GC.KeepAlive(nodes);
                    Debug.Assert(sortedArrayPtr != null);
                    Debug.Assert(pNodes != null);
                    makeHuffmanCodeTree(sortedArrayPtr, sortedArray.Length, pNodes);
                    
                }
            }

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

            for(int i = 0; i < huffmanCode.Length; i++)
            {
                if (huffmanCode[i] != null)
                codedArrayLength += CBA_copy[i] * huffmanCode[i].Length;
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
                    fixed (byte* fileArrayPtr = fileArray)
                    fixed (byte* codedArrayPtr = codedArray)
                    {
                        Debug.Assert(fileArrayPtr != null);
                        Debug.Assert(codedArrayPtr != null);
                        codeFile1(pHuffmanCode, fileArrayPtr, codedArrayPtr, fileArray.Length);
                    }
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

        [DllImport(@"../../../../../x64/Debug/Huffman_cpp.dll")]
        unsafe static extern long* countBytes(byte* byteTable, int arraySize, long* ptr);

        [DllImport(@"../../../../../x64/Debug/Huffman_cpp.dll")]
        unsafe static extern IntPtr makeSortedArray(Int64* array, int sortedArraylength, ByteCounter* countersArray);

        [DllImport(@"../../../../../x64/Debug/Huffman_cpp.dll")]
        unsafe static extern IntPtr makeHuffmanCodeTree(ByteCounter* sortedInputArray, int sizeOfInputArray, TreeNode* huffmanTree);

        [DllImport(@"../../../../../x64/Debug/Huffman_cpp.dll")]
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
            if (path == null) return null;

            int threads = 0;//number of threads to start;
            byte[] fileArray = File.ReadAllBytes(path);
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
            
        }

        override public byte[] Decompress(byte[] code, TreeNode[] HuffmanTree, long fileLength)
        {

            return null;
        }
    }
}
