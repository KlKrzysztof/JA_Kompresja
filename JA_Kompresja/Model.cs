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
    class ModelAsm: Model
    {
        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_asem.dll")]
        static extern void countBytes(byte[] file, int arraySize, Int64[] countedBytes);
        //static extern void dllCompress(int a, int b);

        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_asem.dll")]
        static extern void makeSortedArray(Int64[] array, Int64[] sortedArray, int sortedArraylength);//function rewrites array in -- order without 0

        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_asem.dll")]
        static extern void makeHuffmanCodeTree(Int64[] sortedInputArray, int sizeOfInputArray,TreeNode[] TreeOutputArray);

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
            byte[] fileArray = {10, 4, 8, 5, 10, 4, 10, 5, 1, 10, 0, 0};//File.ReadAllBytes(paths[0]); //file readed as bytes //TO DO: zrobić w pętli dla wszystkich plików//
            Int64[] countedBytesArray = new Int64[256];
            Int64[] sortedArray;
            TreeNode[] nodes;// = new TreeNode[256];

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

            nodes = new TreeNode[notNullBytes*2];

            makeHuffmanCodeTree(sortedArray, sortedArray.Length, nodes);

            //Console.WriteLine(nodes.ToString());
        }
    }
    //model class which operate on c++ library

    class ModelCpp : Model
    {
        [DllImport(@"..\..\..\..\..\x64\Debug\Huffman_cpp.dll")]
        static extern void doSth();

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

            try
            {
                threads = int.Parse(threadsString); //try to parse threadsString to number
            }
            catch (FormatException e)
            {
                threads = 1; //if cannot set default to 1
            }



            doSth();
        }
    }
}
