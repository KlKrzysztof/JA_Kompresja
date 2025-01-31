//Asembly Language - Project
//Krzyszkof Klecha
//section 11
//semester 5
//year 2024/25
//Huffman coding compresion
//version 0.1
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
            byte[][] compressedFile = null;
            String filePath = "";
            Tuple<byte[], char[][], long>[] compressionResult = null;
            int threads = 1;
            Stopwatch sw = new Stopwatch();
            Thread[] threadsArray = null;
            int fileIndex = 0;

            try
            {
                threads = int.Parse(threadsString); //try to parse threadsString to number
            }
            catch (FormatException e)
            {
                threads = 1; //if cannot set default to 1
            }

            compressionResult = new Tuple<byte[], char[][], long>[threads];

            threadsArray = new Thread[threads];

            models = new Model[threads];

            compressedFile = new byte[threads][];

            foreach (char c in paths) { // count size of the array by counting ';'
                if (c == ';')
                {
                    filesCounter++;
                }
            }

            pathsArray = new string[filesCounter]; //initialize array

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
                        pathsArray[i] = paths.Substring(startingPoint, paths.Length); //else take the rest of string at [filesCounter - 1]
                    }
                } while (endPoint != -1); // do while string end isn't reach

            }




            if (asmCheck) // make right model and call compression
            {
                sw.Start();
                while(fileIndex < threads) // creating threads
                {
                    for (int i = 0; i < threads; ++i)
                    {

                        threadsArray[i] = new Thread(() =>
                        {

                            models[fileIndex] = new ModelAsm();

                            if (i < pathsArray.Length)
                                compressionResult[i] = models[i].Compress(pathsArray[i]);

                        });
                        threadsArray[i].Start();
                    }
                }
                sw.Stop();
            }
            else if (cppCheck)
            {
                sw.Start();
                for (int i = 0; i < threads; ++i) // creating threads
                {
                    threadsArray[i] = new Thread(() =>
                    {
                        models[i] = new ModelCpp();

                        if (i < pathsArray.Length)
                            compressionResult[i] = models[i].Compress(pathsArray[i]);
                        
                    });
                    threadsArray[i].Start();
                }
                sw.Stop();
            }

            for (int i = 0; i < threads; ++i)
            { //merging threads
                threadsArray[i].Join();
                //compressedFile[i] = new byte[1];
                compressedFile[i] = compressionResult[i].Item1;
            }

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

                if(File.Exists(filePath))
                    File.Delete(filePath);


                //fill file with compressed data
                using (var fileStream = File.Create(filePath))
                {
                    string fileHufmanCode = "[";

                    fileStream.Write(Encoding.ASCII.GetBytes(fileHeader));

                    

                    for(int i = 0;i<pathsArray.Length;i++)
                    {
                        foreach (var item in compressionResult[i].Item2)
                        {
                            fileHufmanCode += "[";
                            if (item != null)
                                foreach (var character in item)
                                    fileHufmanCode += character.ToString();
                            fileHufmanCode += "]";
                        }

                        fileHufmanCode += "]\n\n";

                        var code = Encoding.ASCII.GetBytes(fileHufmanCode);

                        fileStream.Write(Encoding.ASCII.GetBytes( pathsArray[0] + "(" + code.Length + "," + compressionResult[i].Item3.ToString() + "){\n" )); // nie code.length tylko d³ugoœæ kodu bitowego

                        //wpisaæ kod huffmana
                        fileStream.Write(code);

                        fileStream.Write(compressedFile[i]);

                        fileStream.Write(Encoding.ASCII.GetBytes("\n}"));
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

                    line += lib + ","+ threadsString + "," + time + "," + DateTime.Now.ToLongDateString() + "\n";

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
            foreach (char c in path)
            { // count size of the array by counting ';'
                if (c == ';')
                {
                    MessageBox.Show("Decompression requires one file Only!", "Too many files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if(asmCheck) models[0] = new ModelAsm();
            else models[0] = new ModelCpp();

            if (File.Exists(path))
            {
                using (var fileStream = File.Open(path, FileMode.Open))
                {
                    byte[] file = null;
                    string fileText = null;

                    fileStream.Read(file);

                    if(file != null)
                    {
                        fileText = Encoding.ASCII.GetString(file);

                        if (fileText.Contains(fileHeader))
                        {
                            var fileNameRegex = new Regex("");
                            int fileCodeBeginIndex = fileText.IndexOf('{');
                            int fileNameEndIndex = fileText.IndexOf('(');
                            String fileName = string.Empty;
                            string fileCode = string.Empty;
                            string nodeCodeString = "0";

                            if (fileCodeBeginIndex != -1)
                            {
                                int fileNameBeginIndex = fileText.LastIndexOf('\n', 0, fileNameEndIndex);
                                if(fileNameBeginIndex != -1)
                                    fileName = fileText.Substring(fileNameBeginIndex, fileNameEndIndex);
                            }

                            while(!(fileName == string.Empty))
                            {
                                var propertiesComaIndex = fileText.IndexOf(',', fileNameEndIndex, fileCodeBeginIndex);
                                long CodeLength = long.Parse(fileText.Substring(fileNameEndIndex + 1, propertiesComaIndex));
                                long decompressedFileLength = long.Parse(fileText.Substring(propertiesComaIndex+1, fileCodeBeginIndex));
                                TreeNode[] huffmanTree = null;
                                Tuple<String, int>[] validFields = new Tuple<string, int>[256];
                                String treeText = fileText.Substring(fileCodeBeginIndex+1, fileText.IndexOf('\n', fileCodeBeginIndex+1));

                                //odtworzenie drzewa
                                int iterator = fileCodeBeginIndex + 2;//iterator musi wskazywaæ koniec ostatniego pola
                                int fieldsCounter = 0;
                                for(int i = 0;i<256;++i)
                                {
                                    if (treeText.Substring(iterator, iterator+1) != "[]")
                                    {
                                        int endOfFiled = fileText.IndexOf(']', fileCodeBeginIndex + iterator);
                                        string field = fileText.Substring(iterator, endOfFiled);
                                        validFields[fieldsCounter] = new Tuple<string, int>(field, iterator);
                                        ++fieldsCounter;
                                    }
                                    ++iterator;
                                }
                                huffmanTree = new TreeNode[validFields.Length*2-1];
                                for(int i = 0;validFields.Length*2-1 > i; ++i)
                                {
                                    bool gotNode = false;
                                    //huffmanTree[i] = new TreeNode();

                                    for(int j = 0;j<validFields.Length; ++j)
                                    {
                                        unsafe
                                        {
                                            if (validFields[j].Item1 == "[" + nodeCodeString + "]")
                                            {
                                                huffmanTree[i] = new TreeNode(validFields[j].Item2, 0, null, null);
                                                gotNode = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (gotNode)
                                    {
                                        //wycofaæ ostatni node i zrobiæ odnogê w prawo o ile mo¿liwe
                                        if (nodeCodeString[nodeCodeString.Length - 1] == '1')
                                        {
                                            while (nodeCodeString[nodeCodeString.Length - 1] == '1')
                                                nodeCodeString = nodeCodeString.Substring(0, nodeCodeString.Length - 1);

                                            nodeCodeString = nodeCodeString.Substring(0, nodeCodeString.Length - 1);

                                            nodeCodeString += "1";
                                        }
                                        else
                                        {
                                            nodeCodeString = nodeCodeString.Substring(0, nodeCodeString.Length - 1);
                                            nodeCodeString += "1";
                                        }
                                    }
                                    else//zaimplementowaæ prawe odnogi drzewa
                                    {
                                        //ci¹gn¹æ node dalej w lewo i utworzyæ wêze³ rodzica
                                        unsafe
                                        {
                                            nodeCodeString += "0";
                                            huffmanTree[i] = new TreeNode();
                                            fixed(TreeNode* node = &huffmanTree[i])
                                                huffmanTree[i - 1].LeftNode = node;
                                        }
                                    }
                                }

                                fileCode = fileText.Substring(fileCodeBeginIndex + 2, (int) CodeLength);

                                models[0].Decompress(Encoding.ASCII.GetBytes(fileCode), huffmanTree, decompressedFileLength);
                            }
                        }
                        else
                        {
                            MessageBox.Show("File is wrong format", "Wrong file format", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("File is empty", "Empty file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("File do not exists", "Not a file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}