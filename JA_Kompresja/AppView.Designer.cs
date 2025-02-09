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
            threadsLabel = new Label();
            openExplorer = new Button();
            chooseThreadsNumber = new ComboBox();
            filePathTextbox = new TextBox();
            cppCheckbox = new CheckBox();
            enterPathLabel = new Label();
            asmCheckbox = new CheckBox();
            compressButton = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // threadsLabel
            // 
            threadsLabel.AutoSize = true;
            threadsLabel.Location = new Point(36, 173);
            threadsLabel.Name = "threadsLabel";
            threadsLabel.Size = new Size(61, 20);
            threadsLabel.TabIndex = 3;
            threadsLabel.Text = "Threads";
            // 
            // openExplorer
            // 
            openExplorer.Location = new Point(713, 125);
            openExplorer.Margin = new Padding(3, 4, 3, 4);
            openExplorer.Name = "openExplorer";
            openExplorer.Size = new Size(128, 31);
            openExplorer.TabIndex = 0;
            openExplorer.Text = "Choose Files";
            openExplorer.UseVisualStyleBackColor = true;
            openExplorer.Click += openExplorerButton_Click;
            // 
            // chooseThreadsNumber
            // 
            chooseThreadsNumber.FormattingEnabled = true;
            chooseThreadsNumber.Location = new Point(113, 169);
            chooseThreadsNumber.Margin = new Padding(3, 4, 3, 4);
            chooseThreadsNumber.Name = "chooseThreadsNumber";
            chooseThreadsNumber.Size = new Size(87, 28);
            chooseThreadsNumber.TabIndex = 4;
            // 
            // filePathTextbox
            // 
            filePathTextbox.Location = new Point(113, 125);
            filePathTextbox.Margin = new Padding(3, 4, 3, 4);
            filePathTextbox.Name = "filePathTextbox";
            filePathTextbox.Size = new Size(593, 27);
            filePathTextbox.TabIndex = 1;
            filePathTextbox.TextChanged += filePathTextbox_TextChanged;
            // 
            // cppCheckbox
            // 
            cppCheckbox.AutoSize = true;
            cppCheckbox.Location = new Point(36, 207);
            cppCheckbox.Margin = new Padding(3, 4, 3, 4);
            cppCheckbox.Name = "cppCheckbox";
            cppCheckbox.Size = new Size(109, 24);
            cppCheckbox.TabIndex = 5;
            cppCheckbox.Text = "C++ Library";
            cppCheckbox.UseVisualStyleBackColor = true;
            cppCheckbox.CheckedChanged += cppCheckbox_CheckedChanged;
            // 
            // enterPathLabel
            // 
            enterPathLabel.AutoSize = true;
            enterPathLabel.Location = new Point(36, 129);
            enterPathLabel.Name = "enterPathLabel";
            enterPathLabel.Size = new Size(77, 20);
            enterPathLabel.TabIndex = 2;
            enterPathLabel.Text = "Enter path";
            // 
            // asmCheckbox
            // 
            asmCheckbox.AutoSize = true;
            asmCheckbox.Location = new Point(36, 241);
            asmCheckbox.Margin = new Padding(3, 4, 3, 4);
            asmCheckbox.Name = "asmCheckbox";
            asmCheckbox.Size = new Size(137, 24);
            asmCheckbox.TabIndex = 6;
            asmCheckbox.Text = "Asembly Library";
            asmCheckbox.UseVisualStyleBackColor = true;
            asmCheckbox.CheckedChanged += asmCheckbox_CheckedChanged;
            // 
            // compressButton
            // 
            compressButton.Location = new Point(36, 274);
            compressButton.Margin = new Padding(3, 4, 3, 4);
            compressButton.Name = "compressButton";
            compressButton.Size = new Size(164, 31);
            compressButton.TabIndex = 7;
            compressButton.Text = "Start Compresion";
            compressButton.UseVisualStyleBackColor = true;
            compressButton.Click += compressButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(656, 177);
            label1.Name = "label1";
            label1.Size = new Size(13, 20);
            label1.TabIndex = 9;
            label1.Text = " ";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(571, 177);
            label2.Name = "label2";
            label2.Size = new Size(45, 20);
            label2.TabIndex = 10;
            label2.Text = "Time:";
            // 
            // appView
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(882, 453);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(filePathTextbox);
            Controls.Add(compressButton);
            Controls.Add(threadsLabel);
            Controls.Add(asmCheckbox);
            Controls.Add(openExplorer);
            Controls.Add(enterPathLabel);
            Controls.Add(chooseThreadsNumber);
            Controls.Add(cppCheckbox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 4, 3, 4);
            Name = "appView";
            Text = "Huffman Compresion";
            Load += appView_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label threadsLabel;
        private Button openExplorer;
        private ComboBox chooseThreadsNumber;
        private TextBox filePathTextbox;
        private CheckBox cppCheckbox;
        private Label enterPathLabel;
        private CheckBox asmCheckbox;
        private Button compressButton;
        private Label label1;
        private Label label2;
    }
}
