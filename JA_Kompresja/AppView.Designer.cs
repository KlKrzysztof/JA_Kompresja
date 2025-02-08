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
            DecompresionStart = new Button();
            compressButton = new Button();
            StatisticsTab = new TabPage();
            statisticsPanel = new FlowLayoutPanel();
            label1 = new Label();
            tabControl.SuspendLayout();
            compresionTab.SuspendLayout();
            StatisticsTab.SuspendLayout();
            SuspendLayout();
            // 
            // openExplorer
            // 
            openExplorer.Location = new Point(686, 147);
            openExplorer.Margin = new Padding(3, 4, 3, 4);
            openExplorer.Name = "openExplorer";
            openExplorer.Size = new Size(128, 31);
            openExplorer.TabIndex = 0;
            openExplorer.Text = "Choose Files";
            openExplorer.UseVisualStyleBackColor = true;
            openExplorer.Click += openExplorerButton_Click;
            // 
            // filePathTextbox
            // 
            filePathTextbox.Location = new Point(86, 147);
            filePathTextbox.Margin = new Padding(3, 4, 3, 4);
            filePathTextbox.Name = "filePathTextbox";
            filePathTextbox.Size = new Size(593, 27);
            filePathTextbox.TabIndex = 1;
            filePathTextbox.TextChanged += filePathTextbox_TextChanged;
            // 
            // enterPathLabel
            // 
            enterPathLabel.AutoSize = true;
            enterPathLabel.Location = new Point(9, 151);
            enterPathLabel.Name = "enterPathLabel";
            enterPathLabel.Size = new Size(77, 20);
            enterPathLabel.TabIndex = 2;
            enterPathLabel.Text = "Enter path";
            // 
            // threadsLabel
            // 
            threadsLabel.AutoSize = true;
            threadsLabel.Location = new Point(9, 195);
            threadsLabel.Name = "threadsLabel";
            threadsLabel.Size = new Size(61, 20);
            threadsLabel.TabIndex = 3;
            threadsLabel.Text = "Threads";
            // 
            // chooseThreadsNumber
            // 
            chooseThreadsNumber.FormattingEnabled = true;
            chooseThreadsNumber.Location = new Point(86, 191);
            chooseThreadsNumber.Margin = new Padding(3, 4, 3, 4);
            chooseThreadsNumber.Name = "chooseThreadsNumber";
            chooseThreadsNumber.Size = new Size(87, 28);
            chooseThreadsNumber.TabIndex = 4;
            // 
            // cppCheckbox
            // 
            cppCheckbox.AutoSize = true;
            cppCheckbox.Location = new Point(9, 229);
            cppCheckbox.Margin = new Padding(3, 4, 3, 4);
            cppCheckbox.Name = "cppCheckbox";
            cppCheckbox.Size = new Size(109, 24);
            cppCheckbox.TabIndex = 5;
            cppCheckbox.Text = "C++ Library";
            cppCheckbox.UseVisualStyleBackColor = true;
            cppCheckbox.CheckedChanged += cppCheckbox_CheckedChanged;
            // 
            // asmCheckbox
            // 
            asmCheckbox.AutoSize = true;
            asmCheckbox.Location = new Point(9, 263);
            asmCheckbox.Margin = new Padding(3, 4, 3, 4);
            asmCheckbox.Name = "asmCheckbox";
            asmCheckbox.Size = new Size(137, 24);
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
            tabControl.Margin = new Padding(3, 4, 3, 4);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 7;
            tabControl.Size = new Size(922, 608);
            tabControl.TabIndex = 0;
            // 
            // compresionTab
            // 
            compresionTab.Controls.Add(label1);
            compresionTab.Controls.Add(DecompresionStart);
            compresionTab.Controls.Add(compressButton);
            compresionTab.Controls.Add(asmCheckbox);
            compresionTab.Controls.Add(enterPathLabel);
            compresionTab.Controls.Add(cppCheckbox);
            compresionTab.Controls.Add(filePathTextbox);
            compresionTab.Controls.Add(chooseThreadsNumber);
            compresionTab.Controls.Add(openExplorer);
            compresionTab.Controls.Add(threadsLabel);
            compresionTab.Location = new Point(4, 29);
            compresionTab.Margin = new Padding(3, 4, 3, 4);
            compresionTab.Name = "compresionTab";
            compresionTab.Padding = new Padding(3, 4, 3, 4);
            compresionTab.Size = new Size(914, 575);
            compresionTab.TabIndex = 1;
            compresionTab.Text = "Compresion";
            compresionTab.UseVisualStyleBackColor = true;
            // 
            // DecompresionStart
            // 
            DecompresionStart.Location = new Point(9, 334);
            DecompresionStart.Name = "DecompresionStart";
            DecompresionStart.Size = new Size(164, 29);
            DecompresionStart.TabIndex = 8;
            DecompresionStart.Text = "Start Decompresion";
            DecompresionStart.UseVisualStyleBackColor = true;
            DecompresionStart.Click += DecompresionStart_Click;
            // 
            // compressButton
            // 
            compressButton.Location = new Point(9, 296);
            compressButton.Margin = new Padding(3, 4, 3, 4);
            compressButton.Name = "compressButton";
            compressButton.Size = new Size(164, 31);
            compressButton.TabIndex = 7;
            compressButton.Text = "Start Compresion";
            compressButton.UseVisualStyleBackColor = true;
            compressButton.Click += compressButton_Click;
            // 
            // StatisticsTab
            // 
            StatisticsTab.Controls.Add(statisticsPanel);
            StatisticsTab.Location = new Point(4, 29);
            StatisticsTab.Margin = new Padding(3, 4, 3, 4);
            StatisticsTab.Name = "StatisticsTab";
            StatisticsTab.Padding = new Padding(3, 4, 3, 4);
            StatisticsTab.Size = new Size(914, 575);
            StatisticsTab.TabIndex = 0;
            StatisticsTab.Text = "Statistics";
            StatisticsTab.UseVisualStyleBackColor = true;
            // 
            // statisticsPanel
            // 
            statisticsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            statisticsPanel.Location = new Point(0, 0);
            statisticsPanel.Margin = new Padding(3, 4, 3, 4);
            statisticsPanel.Name = "statisticsPanel";
            statisticsPanel.Size = new Size(913, 567);
            statisticsPanel.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(637, 253);
            label1.Name = "label1";
            label1.Size = new Size(50, 20);
            label1.TabIndex = 9;
            label1.Text = "label1";
            label1.Click += label1_Click;
            // 
            // appView
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(tabControl);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 4, 3, 4);
            Name = "appView";
            Text = "Huffman Compresion";
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
        private Button DecompresionStart;
        private Label label1;
    }
}
