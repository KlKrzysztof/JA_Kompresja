﻿//Asembly Language - Project
//Krzyszkof Klecha
//section 11
//semester 5
//year 2024/25
//Huffman coding compresion
//version 1.0
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
        private static Model?[] models;

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



        //loadStatistics
        //load times for StatisticsDisplay
        //
        //no enter params
        //return value: void
        public static void loadStatistics()
        {
            string path = @"../../../../Data/Times.csv";
            StreamReader file = new StreamReader(path);
            string line, listBoxItem, library = "", threads = "", time = "", date = "";
            int commentIndex = 0, startLineIndex = 0, startlistIndex = 0, comaIndex = 0;

            if (File.Exists(path))//check if file with times exists
            {
                line = file.ReadLine();//load first line
                while (line != null)//check if file ends
                {
                    //cut comments from line
                    commentIndex = line.IndexOf("//");
                    if (commentIndex >= 0)
                        listBoxItem = line.Substring(startLineIndex, commentIndex);
                    else
                        listBoxItem = line;

                    //continue if line is not empty
                    if (listBoxItem != "")
                    {
                        //separate value
                        comaIndex = listBoxItem.IndexOf(',');
                        if (comaIndex >= 0)
                            library = listBoxItem.Substring(startlistIndex, comaIndex);
                        listBoxItem = listBoxItem.Substring(comaIndex + 1);

                        comaIndex = listBoxItem.IndexOf(',');
                        if (comaIndex >= 0)
                            threads = listBoxItem.Substring(startlistIndex, comaIndex);
                        listBoxItem = listBoxItem.Substring(comaIndex + 1);

                        comaIndex = listBoxItem.IndexOf(',');
                        if (comaIndex >= 0)
                            time = listBoxItem.Substring(startlistIndex, comaIndex);
                        listBoxItem = listBoxItem.Substring(comaIndex + 1);

                        date = listBoxItem.Substring(startlistIndex);

                        //create and add new item to statistics panel
                        view.addStatistics(createStatisticsItem(library, threads, time, date));
                    }
                    line = file.ReadLine(); //read next line
                }
            }
        }

        //createStatisticsItem
        //method which create item for statisticsDisplay
        //
        //enter params:
        //library - string representing chosed library
        //threads - string representing number of threads used by application
        //time - string representing how long compression works
        //date - string representing when compresion occurred
        //return value: SplitContainer - item ready to add to display
        private static SplitContainer createStatisticsItem(string library, string threads, string time, string date)
        {
            const int windowWidth = 900, lineHeight = 15;//hardcoded default measure for displayed item
            Label libraryLabel = new Label(), threadsLabel = new Label(), timeLabel = new Label(), dateLabel = new Label();//texts for display on screen
            SplitContainer leftContainer = new SplitContainer(), rightContainer = new SplitContainer(), outerContainer = new SplitContainer();//containers for positioning text on screen

            //set text for labels
            libraryLabel.Text = library;
            threadsLabel.Text = threads;
            timeLabel.Text = time;
            dateLabel.Text = date;

            //nest labels in containers
            leftContainer.Panel1.Controls.Add(libraryLabel);
            leftContainer.Panel2.Controls.Add(threadsLabel);

            rightContainer.Panel1.Controls.Add(timeLabel);
            rightContainer.Panel2.Controls.Add(dateLabel);

            //nest containers in outer container and set for equal spaces
            outerContainer.Panel1.Controls.Add(leftContainer);
            outerContainer.Panel2.Controls.Add(rightContainer);

            leftContainer.SplitterDistance = windowWidth / 4;
            leftContainer.Width = windowWidth / 2;

            rightContainer.SplitterDistance = windowWidth / 4;
            rightContainer.Width = windowWidth / 2;

            //set width of conatiners 
            leftContainer.Width = windowWidth / 2;
            rightContainer.Width = windowWidth / 2;
            outerContainer.Width = windowWidth;

            //set label width, height and alignment 
            libraryLabel.TextAlign = ContentAlignment.MiddleLeft;
            libraryLabel.Height = lineHeight;
            libraryLabel.Width = windowWidth / 4;

            threadsLabel.TextAlign = ContentAlignment.MiddleLeft;
            threadsLabel.Height = lineHeight;
            threadsLabel.Width = windowWidth / 4;

            timeLabel.TextAlign = ContentAlignment.MiddleRight;
            timeLabel.Height = lineHeight;
            timeLabel.Width = windowWidth / 4;

            dateLabel.TextAlign = ContentAlignment.MiddleRight;
            dateLabel.Height = lineHeight;
            dateLabel.Width = windowWidth / 4;

            //set height of containers for good alignment
            outerContainer.Height = lineHeight;
            leftContainer.Height = lineHeight;
            rightContainer.Height = lineHeight;

            //return ready item
            return outerContainer;
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

            models = new Model[threads];

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
            List<byte[]> compressedFile = [];
            int pathsArrayLength = pathsArray.Length;

            sw.Start();
            if (cppCheck)
            {
                for (int i = 0; i < pathsArrayLength; i++)
                {
                    var tempPath = pathsArray[i];

                    Thread t = new(new ThreadStart(() =>
                    {
                        var m = new ModelCpp();
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

                            fileStream.Write(compressedFile[i]);

                            fileStream.Write(Encoding.ASCII.GetBytes("\n}"));
                        }
                    }

                    fileStream.Close();
                }
            }

            if (!File.Exists(timesPath))
            {
                using (FileStream fs = File.Create(timesPath))
                {
                    string lib = "";
                    string line = "";

                    string time = ((double)sw.Elapsed.Milliseconds / 1000.0).ToString();
                    time = time.Replace(',', '.');

                    if (cppCheck) lib = "Cpp";
                    else lib = "Asm";

                    line += lib + "," + threadsString + "," + time + "," + DateTime.Now.ToLongDateString() + "\n";

                    fs.Write(Encoding.ASCII.GetBytes(line));

                    fs.Close();
                }
            }
            else
            {

                using (FileStream fs = File.Open(timesPath, FileMode.Append))
                {
                    string lib = "";
                    string line = "";
                    string time = ((double)sw.Elapsed.Milliseconds / 1000.0).ToString();
                    time = time.Replace(',', '.');

                    if (cppCheck) lib = "Cpp";
                    else lib = "Asm";

                    line += lib + "," + threadsString + "," + time + "," + DateTime.Now.ToLongDateString() + "\n";

                    fs.Write(Encoding.ASCII.GetBytes(line));

                    fs.Close();
                }
            }
        }

        public static void Decompress(string path, string threadsString, bool cppCheck, bool asmCheck)
        {

        }
    }
}
