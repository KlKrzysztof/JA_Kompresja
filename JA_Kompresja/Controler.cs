//Asembly Language - Project
//Krzyszkof Klecha
//section 11
//semester 5
//year 2024/25
//Huffman coding compresion
//version 0.1
//
//Class which is application's controler from MVC pattern

using System.Reflection;

namespace JA_Kompresja
{
    internal static class Controler
    {
        private static appView view;
        private static Model model;
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

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static System.Reflection.Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            string path = @".\Ja_Kompresja.dll";
            if (File.Exists(path))
            {
                return Assembly.LoadFrom(path);
            }
            else
            {
                return null;
            }
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

            var compressParams = (pathsArray, threadsString); //create a tuple

            if (asmCheck) // make right model and call compression
            {
                model = new ModelAsm();

                model.Compress(compressParams);
            }
            else if(cppCheck)
            {
                model = new ModelCpp();

                model.Compress(compressParams);
            }
        }
    }
}