//Asembly Language - Project
//Krzyszkof Klecha
//section 11
//semester 5
//year 2024/25
//Huffman coding compresion
//version 1.1
//
//Class which is application's view from MVC pattern

using System.Windows.Forms;

namespace JA_Kompresja
{
    public partial class appView : Form
    {
        public static float ApplicationVer = 0.6f;
        public appView()
        {
            InitializeComponent();
        }

        public void showTime(string time)
        {
            this.label1.Text = time;
        }

        //appView_load
        //initialize application Window and set GUI
        //
        //enter params:
        //sender - object which call method
        //e - event object
        //return value: void
        private void appView_Load(object sender, EventArgs e)
        {
            const int maxThreads = 64;//Hardcoded maximum threads
            String[] threadsString = new String[64];//Threads number as string for combobox 

            for (int i = 1; i <= maxThreads; ++i)//creating range for ChooseThreadsNumber
            {
                threadsString[i - 1] = i.ToString();
            }
            this.chooseThreadsNumber.Items.AddRange(threadsString);

            this.chooseThreadsNumber.Text = Environment.ProcessorCount.ToString();//processors counting and seting default number of threads

            this.cppCheckbox.Checked = true;//seting default library

            //Controler.loadStatistics();//loading statistics
        }

        //openExplorerButton_Click
        //On click open file explorer
        //
        //enter params:
        //sender - object which call method
        //e - event object
        //return value: void
        private void openExplorerButton_Click(object sender, EventArgs e)
        {
            string filePath = "";
            //open file dialog
            var dialog = new OpenFileDialog()
            {
                InitialDirectory = @"C:\Users", //start in User directory
                Filter = "All Files (*.*) | *.*", //search for all file
                RestoreDirectory = false, //restore file to previously chosed while closing
                Multiselect = true //can select multiple files

            };

            //User didn't select a file so return a default value 
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                filePath = "";
            }
            //Return the files the user selected  
            else
            {
                foreach (var file in dialog.FileNames)
                {
                    filePath += file + ';'; //merge all files in one string separate by ';'
                }

                filePath = filePath.Remove(filePath.Length - 1);//remove last ';'
            }

            this.filePathTextbox.Text = filePath;
        }


        //cppCheckbox_CheckedChanged
        //C++ checkbox setings
        //
        //enter params:
        //sender - object which call method
        //e - event object
        //return value: void
        private void cppCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            //if checkbox is check then uncheck other checkbox, else do opposite
            if (cppCheckbox.Checked)
            {
                this.asmCheckbox.Checked = false;
            }
            else
            {
                this.asmCheckbox.Checked = true;
            }
        }

        //asmCheckbox_CheckedChanged
        //asembly checkbox setings
        //
        //enter params:
        //sender - object which call method
        //e - event object
        //return value: void
        private void asmCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            //if checkbox is check then uncheck other checkbox, else do opposite
            if (asmCheckbox.Checked)
            {
                this.cppCheckbox.Checked = false;
            }
            else
            {
                this.cppCheckbox.Checked = true;
            }
        }

        //addStatistics
        //add new time to the statistics display
        //
        //enter params:
        //item - new data added to display
        //SplitContainer with nested SplitContainer with Labels
        //
        //return value: void
        public void addStatistics(SplitContainer item)
        {
            if (item != null)
            {
                this.statisticsPanel.Controls.Add(item);
            }
        }

        //compressButton_Click
        //method which pack settings and pass it to controler then it call Controler to start compression
        //
        //enter params:
        //sender - object which call method
        //e - event object
        //return value: void
        private void compressButton_Click(Object sender, EventArgs e)
        {
            string path = filePathTextbox.Text, threadsString = chooseThreadsNumber.SelectedItem!.ToString()!;
            bool cppCheck = cppCheckbox.Checked, asmCheck = asmCheckbox.Checked;

            Controler.Compress(path, threadsString, cppCheck, asmCheck);
        }

        private void filePathTextbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void DecompresionStart_Click(object sender, EventArgs e)
        {
            Controler.Decompress(filePathTextbox.Text, chooseThreadsNumber.SelectedText, cppCheckbox.Checked, asmCheckbox.Checked);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
