﻿//Asembly Language - Project
//Krzyszkof Klecha
//section 11
//semester 5
//year 2024/25
//Huffman coding compresion
//version 1.1
//
//Class which is application's controler from MVC pattern

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace JA_Kompresja
{
    internal static class Controler
    {
        private static appView? view;

        private static string timesPath = "Times.csv";

        private static String fileHeader = "File compressed by Huffman Compresion Application made by Krzysztof Klecha\nApp version: " + appView.ApplicationVer.ToString() + "\n";
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            view = new appView();
            Application.Run(view);
        }


        //Compres
        //method which choose whitch dll library should operate and then call compresion
        //
        //enter param:
        //path - string representing a path to file
        //threadsString - string representing how many threads should compress files
        //cppCheck, asmCheck - flags which tell which  library will compress files
        //
        //return value: void
        public static void Compress(string paths, string threadsString, bool cppCheck, bool asmCheck)
        {
            string[] pathsArray;//array of paths
            int filesCounter = 1;// count size of the array to initialize
            int startingPoint = 0;// starting point of new file path substr
            int endPoint = -1; // end point of file path substr
            String filePath = "";
            int threads = 1;
            Stopwatch sw = new();
            try
            {
                threads = int.Parse(threadsString); //try to parse threadsString to number
            }
            catch (FormatException e)
            {
                threads = 1; //if cannot set default to 1
            }

            foreach (char c in paths)
            { // count size of the array by counting ';'
                if (c == ';')
                {
                    filesCounter++;
                }
            }

            if (filesCounter < threads)
            {
                pathsArray = new string[threads]; //initialize array
            }
            else
            {
                pathsArray = new string[filesCounter]; //initialize array
            }

            {//loop scope
                int i = 0; //array iterator
                do
                {
                    startingPoint = endPoint + 1; // set starting point
                    endPoint = paths.IndexOf(';', startingPoint);//search for ';'
                    if (endPoint != -1)
                    {
                        pathsArray[i] = paths.Substring(startingPoint, endPoint - startingPoint); //if found take substr at [i]
                        ++i; //continue
                    }
                    else
                    {
                        pathsArray[i] = paths.Substring(startingPoint, paths.Length - startingPoint); //else take the rest of string at [filesCounter - 1]
                    }
                } while (endPoint != -1); // do while string end isn't reach

            }

            List<Tuple<byte[], char[][], long>> compressionResult = [];
            List<Thread> threadList = [];
            List<byte[]?> compressedFile = [];
            int pathsArrayLength = pathsArray.Length;

            sw.Start();
            if (cppCheck)
            {
                for (int i = 0; i < pathsArrayLength; i++)
                {
                    var tempPath = new String(pathsArray[i]);

                    Thread t = new(new ThreadStart(() =>
                    {
                        var localPath = tempPath;
                        var m = new ModelCpp();
                        var res = m.Compress(localPath);
                        lock (compressionResult)
                        {
                            compressionResult.Add(res);
                        }
                    }));

                    threadList.Add(t);
                    t.Start();
                }
            }

            else if (asmCheck)
            {
                for (int i = 0; i < pathsArrayLength; i++)
                {

                    var tempPath = pathsArray[i];

                    Thread t = new(new ThreadStart(() =>
                    {
                        var m = new ModelAsm();
                        var res = m.Compress(tempPath);
                        lock (compressionResult)
                        {
                            compressionResult.Add(res);
                        }
                    }));

                    threadList.Add(t);
                    t.Start();
                }
            }

            for (int i = 0; i < pathsArrayLength; ++i)
            {
                threadList[i].Join();
                if (compressionResult[i] != null)
                    compressedFile.Add(compressionResult[i].Item1);
                else
                    compressedFile.Add(null);
            }
            sw.Stop();


            var dialog = new SaveFileDialog()
            {
                InitialDirectory = @"C:\Users", //start in User directory
                Filter = "All Files (*.*) | *.*", //search for all file
                RestoreDirectory = false, //restore file to previously chosed while closing
            };

            //User didn't select a file so return a default value 
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                filePath = "";
            }
            //Return the files the user selected  
            else
            {
                filePath = dialog.FileName;

                if (File.Exists(filePath))
                    File.Delete(filePath);


                //fill file with compressed data
                using (var fileStream = File.Create(filePath))
                {
                    fileStream.Write(Encoding.ASCII.GetBytes(fileHeader));

                    for (int i = 0; i < pathsArray.Length; i++)
                    {
                        if (compressionResult[i] != null)
                        {
                            fileStream.Write(Encoding.ASCII.GetBytes(pathsArray[0] + "{\n"));

                            fileStream.Write(compressedFile[i]!);

                            fileStream.Write(Encoding.ASCII.GetBytes("\n}\n"));
                        }
                    }

                    fileStream.Close();
                }
            }

            

            view!.showTime(((double)sw.Elapsed.Milliseconds / 1000.0).ToString());

            compressionResult = null;
            
        }

        
    }
}
