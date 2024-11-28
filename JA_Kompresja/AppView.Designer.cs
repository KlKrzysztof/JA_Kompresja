namespace JA_Kompresja
{
    partial class appView
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            openExplorer = new Button();
            filePathTextbox = new TextBox();
            enterPathLabel = new Label();
            threadsLabel = new Label();
            chooseThreadsNumber = new ComboBox();
            cppCheckbox = new CheckBox();
            asmCheckbox = new CheckBox();
            tabControl = new TabControl();
            compresionTab = new TabPage();
            compressButton = new Button();
            StatisticsTab = new TabPage();
            statisticsPanel = new FlowLayoutPanel();
            tabControl.SuspendLayout();
            compresionTab.SuspendLayout();
            StatisticsTab.SuspendLayout();
            SuspendLayout();
            // 
            // openExplorer
            // 
            openExplorer.Location = new Point(600, 110);
            openExplorer.Name = "openExplorer";
            openExplorer.Size = new Size(112, 23);
            openExplorer.TabIndex = 0;
            openExplorer.Text = "Choose Files";
            openExplorer.UseVisualStyleBackColor = true;
            openExplorer.Click += openExplorerButton_Click;
            // 
            // filePathTextbox
            // 
            filePathTextbox.Location = new Point(75, 110);
            filePathTextbox.Name = "filePathTextbox";
            filePathTextbox.Size = new Size(519, 23);
            filePathTextbox.TabIndex = 1;
            filePathTextbox.TextChanged += filePathTextbox_TextChanged;
            // 
            // enterPathLabel
            // 
            enterPathLabel.AutoSize = true;
            enterPathLabel.Location = new Point(8, 113);
            enterPathLabel.Name = "enterPathLabel";
            enterPathLabel.Size = new Size(61, 15);
            enterPathLabel.TabIndex = 2;
            enterPathLabel.Text = "Enter path";
            // 
            // threadsLabel
            // 
            threadsLabel.AutoSize = true;
            threadsLabel.Location = new Point(8, 146);
            threadsLabel.Name = "threadsLabel";
            threadsLabel.Size = new Size(48, 15);
            threadsLabel.TabIndex = 3;
            threadsLabel.Text = "Threads";
            // 
            // chooseThreadsNumber
            // 
            chooseThreadsNumber.FormattingEnabled = true;
            chooseThreadsNumber.Location = new Point(75, 143);
            chooseThreadsNumber.Name = "chooseThreadsNumber";
            chooseThreadsNumber.Size = new Size(62, 23);
            chooseThreadsNumber.TabIndex = 4;
            // 
            // cppCheckbox
            // 
            cppCheckbox.AutoSize = true;
            cppCheckbox.Location = new Point(8, 172);
            cppCheckbox.Name = "cppCheckbox";
            cppCheckbox.Size = new Size(89, 19);
            cppCheckbox.TabIndex = 5;
            cppCheckbox.Text = "C++ Library";
            cppCheckbox.UseVisualStyleBackColor = true;
            cppCheckbox.CheckedChanged += cppCheckbox_CheckedChanged;
            // 
            // asmCheckbox
            // 
            asmCheckbox.AutoSize = true;
            asmCheckbox.Location = new Point(8, 197);
            asmCheckbox.Name = "asmCheckbox";
            asmCheckbox.Size = new Size(111, 19);
            asmCheckbox.TabIndex = 6;
            asmCheckbox.Text = "Asembly Library";
            asmCheckbox.UseVisualStyleBackColor = true;
            asmCheckbox.CheckedChanged += asmCheckbox_CheckedChanged;
            // 
            // tabControl
            // 
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl.Controls.Add(compresionTab);
            tabControl.Controls.Add(StatisticsTab);
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 7;
            tabControl.Size = new Size(807, 456);
            tabControl.TabIndex = 0;
            // 
            // compresionTab
            // 
            compresionTab.Controls.Add(compressButton);
            compresionTab.Controls.Add(asmCheckbox);
            compresionTab.Controls.Add(enterPathLabel);
            compresionTab.Controls.Add(cppCheckbox);
            compresionTab.Controls.Add(filePathTextbox);
            compresionTab.Controls.Add(chooseThreadsNumber);
            compresionTab.Controls.Add(openExplorer);
            compresionTab.Controls.Add(threadsLabel);
            compresionTab.Location = new Point(4, 24);
            compresionTab.Name = "compresionTab";
            compresionTab.Padding = new Padding(3);
            compresionTab.Size = new Size(799, 428);
            compresionTab.TabIndex = 1;
            compresionTab.Text = "Compresion";
            compresionTab.UseVisualStyleBackColor = true;
            // 
            // compressButton
            // 
            compressButton.Location = new Point(8, 222);
            compressButton.Name = "compressButton";
            compressButton.Size = new Size(129, 23);
            compressButton.TabIndex = 7;
            compressButton.Text = "Start Compresion";
            compressButton.UseVisualStyleBackColor = true;
            compressButton.Click += compressButton_Click;
            // 
            // StatisticsTab
            // 
            StatisticsTab.Controls.Add(statisticsPanel);
            StatisticsTab.Location = new Point(4, 24);
            StatisticsTab.Name = "StatisticsTab";
            StatisticsTab.Padding = new Padding(3);
            StatisticsTab.Size = new Size(799, 428);
            StatisticsTab.TabIndex = 0;
            StatisticsTab.Text = "Statistics";
            StatisticsTab.UseVisualStyleBackColor = true;
            // 
            // statisticsPanel
            // 
            statisticsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            statisticsPanel.Location = new Point(0, 0);
            statisticsPanel.Name = "statisticsPanel";
            statisticsPanel.Size = new Size(799, 425);
            statisticsPanel.TabIndex = 0;
            // 
            // appView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tabControl);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "appView";
            Text = "Huffman Compresion";
            TopMost = false;
            Load += appView_Load;
            tabControl.ResumeLayout(false);
            compresionTab.ResumeLayout(false);
            compresionTab.PerformLayout();
            StatisticsTab.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button openExplorer;
        private TextBox filePathTextbox;
        private Label enterPathLabel;
        private Label threadsLabel;
        private ComboBox chooseThreadsNumber;
        private CheckBox cppCheckbox;
        private CheckBox asmCheckbox;
        private TabControl tabControl;
        private TabPage StatisticsTab;
        private TabPage compresionTab;
        private FlowLayoutPanel statisticsPanel;
        private Button compressButton;
    }
}
