namespace SimpleNN
{
    partial class Form1
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
            lblTraningFile = new Label();
            lblTestFile = new Label();
            btnRun = new Button();
            btnClose = new Button();
            lstLog = new ListBox();
            grphErr = new ScottPlot.WinForms.FormsPlot();
            lblEpox = new Label();
            lblLayers = new Label();
            lblNeurons = new Label();
            lblLearningRate = new Label();
            numbxEpox = new NumericUpDown();
            numbxLayers = new NumericUpDown();
            numbxNeurons = new NumericUpDown();
            numbxLearningRate = new NumericUpDown();
            btnTest = new Button();
            btnSingle = new Button();
            btnBrowseTrain = new Button();
            btnBrowseTest = new Button();
            btnBrowseSingle = new Button();
            lblSingleFile = new Label();
            lblTrainCaption = new Label();
            lblTestCaption = new Label();
            lblSingleCaption = new Label();
            ((System.ComponentModel.ISupportInitialize)numbxEpox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numbxLayers).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numbxNeurons).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numbxLearningRate).BeginInit();
            SuspendLayout();
            // 
            // lblTraningFile
            // 
            lblTraningFile.AutoEllipsis = true;
            lblTraningFile.Location = new Point(190, 12);
            lblTraningFile.Name = "lblTraningFile";
            lblTraningFile.Size = new Size(562, 28);
            lblTraningFile.TabIndex = 0;
            lblTraningFile.Text = "File di Training";
            //
            // lblTestFile
            //
            lblTestFile.AutoEllipsis = true;
            lblTestFile.Location = new Point(190, 49);
            lblTestFile.Name = "lblTestFile";
            lblTestFile.Size = new Size(562, 28);
            lblTestFile.TabIndex = 1;
            lblTestFile.Text = "File di Test";
            //
            // lblTrainCaption
            //
            lblTrainCaption.AutoSize = true;
            lblTrainCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTrainCaption.Location = new Point(12, 12);
            lblTrainCaption.Name = "lblTrainCaption";
            lblTrainCaption.Size = new Size(120, 25);
            lblTrainCaption.TabIndex = 20;
            lblTrainCaption.Text = "Training:";
            //
            // lblTestCaption
            //
            lblTestCaption.AutoSize = true;
            lblTestCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTestCaption.Location = new Point(12, 49);
            lblTestCaption.Name = "lblTestCaption";
            lblTestCaption.Size = new Size(120, 25);
            lblTestCaption.TabIndex = 21;
            lblTestCaption.Text = "Test:";
            //
            // lblSingleCaption
            //
            lblSingleCaption.AutoSize = true;
            lblSingleCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSingleCaption.Location = new Point(12, 86);
            lblSingleCaption.Name = "lblSingleCaption";
            lblSingleCaption.Size = new Size(120, 25);
            lblSingleCaption.TabIndex = 22;
            lblSingleCaption.Text = "Single:";
            // 
            // btnRun
            // 
            btnRun.Location = new Point(12, 1045);
            btnRun.Name = "btnRun";
            btnRun.Size = new Size(112, 34);
            btnRun.TabIndex = 2;
            btnRun.Text = "Run";
            btnRun.UseVisualStyleBackColor = true;
            btnRun.Click += btnRun_Click;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(1476, 1045);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(112, 34);
            btnClose.TabIndex = 3;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // lstLog
            // 
            lstLog.FormattingEnabled = true;
            lstLog.ItemHeight = 25;
            lstLog.Location = new Point(1130, 10);
            lstLog.Name = "lstLog";
            lstLog.Size = new Size(458, 1029);
            lstLog.TabIndex = 4;
            // 
            // grphErr
            // 
            grphErr.DisplayScale = 1.5F;
            grphErr.Location = new Point(-5, 195);
            grphErr.Name = "grphErr";
            grphErr.Size = new Size(1119, 844);
            grphErr.TabIndex = 5;
            //
            // lblEpox
            //
            lblEpox.AutoSize = true;
            lblEpox.Location = new Point(12, 125);
            lblEpox.Name = "lblEpox";
            lblEpox.Size = new Size(51, 25);
            lblEpox.TabIndex = 6;
            lblEpox.Text = "Epox";
            //
            // lblLayers
            //
            lblLayers.AutoSize = true;
            lblLayers.Location = new Point(213, 125);
            lblLayers.Name = "lblLayers";
            lblLayers.Size = new Size(61, 25);
            lblLayers.TabIndex = 7;
            lblLayers.Text = "Layers";
            //
            // lblNeurons
            //
            lblNeurons.AutoSize = true;
            lblNeurons.Location = new Point(415, 125);
            lblNeurons.Name = "lblNeurons";
            lblNeurons.Size = new Size(79, 25);
            lblNeurons.TabIndex = 8;
            lblNeurons.Text = "Neurons";
            //
            // lblLearningRate
            //
            lblLearningRate.AutoSize = true;
            lblLearningRate.Location = new Point(617, 125);
            lblLearningRate.Name = "lblLearningRate";
            lblLearningRate.Size = new Size(119, 25);
            lblLearningRate.TabIndex = 9;
            lblLearningRate.Text = "Learning Rate";
            //
            // numbxEpox
            //
            numbxEpox.Location = new Point(12, 155);
            numbxEpox.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numbxEpox.Name = "numbxEpox";
            numbxEpox.Size = new Size(180, 31);
            numbxEpox.TabIndex = 10;
            numbxEpox.Value = new decimal(new int[] { 50, 0, 0, 0 });
            numbxEpox.ValueChanged += numbxEpox_ValueChanged;
            //
            // numbxLayers
            //
            numbxLayers.Location = new Point(213, 155);
            numbxLayers.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numbxLayers.Name = "numbxLayers";
            numbxLayers.Size = new Size(180, 31);
            numbxLayers.TabIndex = 11;
            numbxLayers.Value = new decimal(new int[] { 4, 0, 0, 0 });
            numbxLayers.ValueChanged += numbxEpox_ValueChanged;
            //
            // numbxNeurons
            //
            numbxNeurons.Location = new Point(415, 153);
            numbxNeurons.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numbxNeurons.Name = "numbxNeurons";
            numbxNeurons.Size = new Size(180, 31);
            numbxNeurons.TabIndex = 12;
            numbxNeurons.Value = new decimal(new int[] { 5, 0, 0, 0 });
            numbxNeurons.ValueChanged += numbxEpox_ValueChanged;
            //
            // numbxLearningRate
            //
            numbxLearningRate.DecimalPlaces = 4;
            numbxLearningRate.Increment = new decimal(new int[] { 1, 0, 0, 196608 });
            numbxLearningRate.Location = new Point(617, 153);
            numbxLearningRate.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            numbxLearningRate.Name = "numbxLearningRate";
            numbxLearningRate.Size = new Size(180, 31);
            numbxLearningRate.TabIndex = 13;
            numbxLearningRate.Value = new decimal(new int[] { 1, 0, 0, 131072 });
            //
            // btnTest
            // 
            btnTest.Location = new Point(162, 1045);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(112, 34);
            btnTest.TabIndex = 14;
            btnTest.Text = "Test";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += btnTest_Click;
            // 
            // btnSingle
            // 
            btnSingle.Location = new Point(317, 1045);
            btnSingle.Name = "btnSingle";
            btnSingle.Size = new Size(112, 34);
            btnSingle.TabIndex = 15;
            btnSingle.Text = "Single";
            btnSingle.UseVisualStyleBackColor = true;
            btnSingle.Click += btnSingle_Click;
            //
            // btnBrowseTrain
            //
            btnBrowseTrain.Location = new Point(760, 9);
            btnBrowseTrain.Name = "btnBrowseTrain";
            btnBrowseTrain.Size = new Size(150, 34);
            btnBrowseTrain.TabIndex = 16;
            btnBrowseTrain.Text = "Sfoglia...";
            btnBrowseTrain.UseVisualStyleBackColor = true;
            btnBrowseTrain.Click += btnBrowseTrain_Click;
            //
            // btnBrowseTest
            //
            btnBrowseTest.Location = new Point(760, 46);
            btnBrowseTest.Name = "btnBrowseTest";
            btnBrowseTest.Size = new Size(150, 34);
            btnBrowseTest.TabIndex = 17;
            btnBrowseTest.Text = "Sfoglia...";
            btnBrowseTest.UseVisualStyleBackColor = true;
            btnBrowseTest.Click += btnBrowseTest_Click;
            //
            // lblSingleFile
            //
            lblSingleFile.AutoEllipsis = true;
            lblSingleFile.Location = new Point(190, 86);
            lblSingleFile.Name = "lblSingleFile";
            lblSingleFile.Size = new Size(562, 28);
            lblSingleFile.TabIndex = 18;
            lblSingleFile.Text = "File Single";
            //
            // btnBrowseSingle
            //
            btnBrowseSingle.Location = new Point(760, 83);
            btnBrowseSingle.Name = "btnBrowseSingle";
            btnBrowseSingle.Size = new Size(150, 34);
            btnBrowseSingle.TabIndex = 19;
            btnBrowseSingle.Text = "Sfoglia...";
            btnBrowseSingle.UseVisualStyleBackColor = true;
            btnBrowseSingle.Click += btnBrowseSingle_Click;
            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1600, 1091);
            Controls.Add(lblTrainCaption);
            Controls.Add(lblTestCaption);
            Controls.Add(lblSingleCaption);
            Controls.Add(btnBrowseSingle);
            Controls.Add(lblSingleFile);
            Controls.Add(btnBrowseTest);
            Controls.Add(btnBrowseTrain);
            Controls.Add(btnSingle);
            Controls.Add(btnTest);
            Controls.Add(numbxLearningRate);
            Controls.Add(numbxNeurons);
            Controls.Add(numbxLayers);
            Controls.Add(numbxEpox);
            Controls.Add(lblLearningRate);
            Controls.Add(lblNeurons);
            Controls.Add(lblLayers);
            Controls.Add(lblEpox);
            Controls.Add(grphErr);
            Controls.Add(lstLog);
            Controls.Add(btnClose);
            Controls.Add(btnRun);
            Controls.Add(lblTestFile);
            Controls.Add(lblTraningFile);
            Margin = new Padding(4, 5, 4, 5);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)numbxEpox).EndInit();
            ((System.ComponentModel.ISupportInitialize)numbxLayers).EndInit();
            ((System.ComponentModel.ISupportInitialize)numbxNeurons).EndInit();
            ((System.ComponentModel.ISupportInitialize)numbxLearningRate).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTraningFile;
        private Label lblTestFile;
        private Button btnRun;
        private Button btnClose;
        private ListBox lstLog;
        private ScottPlot.WinForms.FormsPlot grphErr;
        private Label lblEpox;
        private Label lblLayers;
        private Label lblNeurons;
        private Label lblLearningRate;
        private NumericUpDown numbxEpox;
        private NumericUpDown numbxLayers;
        private NumericUpDown numbxNeurons;
        private NumericUpDown numbxLearningRate;
        private Button btnTest;
        private Button btnSingle;
        private Button btnBrowseTrain;
        private Button btnBrowseTest;
        private Button btnBrowseSingle;
        private Label lblSingleFile;
        private Label lblTrainCaption;
        private Label lblTestCaption;
        private Label lblSingleCaption;
    }
}
