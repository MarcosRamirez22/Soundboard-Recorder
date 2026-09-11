namespace SoundByte_Builder
{
    partial class MainForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            btnStart = new Button();
            btnStop = new Button();
            cmbRecordingMode = new ComboBox();
            label1 = new Label();
            lblHotkey = new Label();
            btnChangeHotkey = new Button();
            txtSaveFolder = new TextBox();
            lblSaveFolder = new Label();
            btnBrowseFolder = new Button();
            lblStatus = new Label();
            btnShowRecordings = new Button();
            tableLayoutPanel2 = new TableLayoutPanel();
            cmbMicrophone = new ComboBox();
            chkIncludeMicrophone = new CheckBox();
            chkAudioOutput = new CheckBox();
            btnAudioOutputs = new Button();
            ctxAudioOutputs = new ContextMenuStrip(components);
            chkApplications = new CheckBox();
            btnApplications = new Button();
            ctxApplications = new ContextMenuStrip(components);
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStart.AutoSize = true;
            btnStart.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnStart.Location = new Point(3, 5);
            btnStart.MaximumSize = new Size(100, 23);
            btnStart.MinimumSize = new Size(50, 23);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(98, 23);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start Recording";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Anchor = AnchorStyles.Bottom;
            btnStop.AutoSize = true;
            btnStop.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnStop.Location = new Point(159, 5);
            btnStop.MaximumSize = new Size(100, 23);
            btnStop.MinimumSize = new Size(50, 23);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(98, 23);
            btnStop.TabIndex = 1;
            btnStop.Text = "Stop Recording";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // cmbRecordingMode
            // 
            cmbRecordingMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRecordingMode.FormattingEnabled = true;
            cmbRecordingMode.Location = new Point(135, 117);
            cmbRecordingMode.Name = "cmbRecordingMode";
            cmbRecordingMode.Size = new Size(103, 23);
            cmbRecordingMode.TabIndex = 4;
            cmbRecordingMode.SelectedIndexChanged += cmbRecordingMode_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(31, 120);
            label1.Name = "label1";
            label1.Size = new Size(98, 15);
            label1.TabIndex = 5;
            label1.Text = "Recording Mode:";
            // 
            // lblHotkey
            // 
            lblHotkey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblHotkey.AutoSize = true;
            lblHotkey.Location = new Point(31, 162);
            lblHotkey.Name = "lblHotkey";
            lblHotkey.Size = new Size(105, 15);
            lblHotkey.TabIndex = 6;
            lblHotkey.Text = "Recording Hotkey:";
            // 
            // btnChangeHotkey
            // 
            btnChangeHotkey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnChangeHotkey.Location = new Point(205, 158);
            btnChangeHotkey.MaximumSize = new Size(200, 23);
            btnChangeHotkey.MinimumSize = new Size(100, 23);
            btnChangeHotkey.Name = "btnChangeHotkey";
            btnChangeHotkey.Size = new Size(100, 23);
            btnChangeHotkey.TabIndex = 8;
            btnChangeHotkey.Text = "Change Hotkey";
            btnChangeHotkey.UseVisualStyleBackColor = true;
            btnChangeHotkey.Click += btnChangeHotkey_Click;
            // 
            // txtSaveFolder
            // 
            txtSaveFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSaveFolder.Location = new Point(107, 190);
            txtSaveFolder.MaximumSize = new Size(600, 23);
            txtSaveFolder.MinimumSize = new Size(50, 23);
            txtSaveFolder.Name = "txtSaveFolder";
            txtSaveFolder.Size = new Size(343, 23);
            txtSaveFolder.TabIndex = 9;
            // 
            // lblSaveFolder
            // 
            lblSaveFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblSaveFolder.AutoSize = true;
            lblSaveFolder.Location = new Point(31, 193);
            lblSaveFolder.Name = "lblSaveFolder";
            lblSaveFolder.Size = new Size(70, 15);
            lblSaveFolder.TabIndex = 10;
            lblSaveFolder.Text = "Save Folder:";
            // 
            // btnBrowseFolder
            // 
            btnBrowseFolder.Location = new Point(107, 219);
            btnBrowseFolder.MaximumSize = new Size(200, 23);
            btnBrowseFolder.MinimumSize = new Size(20, 23);
            btnBrowseFolder.Name = "btnBrowseFolder";
            btnBrowseFolder.Size = new Size(90, 23);
            btnBrowseFolder.TabIndex = 11;
            btnBrowseFolder.Text = "Browse...";
            btnBrowseFolder.UseVisualStyleBackColor = true;
            btnBrowseFolder.Click += btnBrowseFolder_Click;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(47, 323);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 15);
            lblStatus.TabIndex = 12;
            // 
            // btnShowRecordings
            // 
            btnShowRecordings.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnShowRecordings.AutoSize = true;
            btnShowRecordings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnShowRecordings.Location = new Point(308, 5);
            btnShowRecordings.MaximumSize = new Size(110, 23);
            btnShowRecordings.MinimumSize = new Size(50, 23);
            btnShowRecordings.Name = "btnShowRecordings";
            btnShowRecordings.Size = new Size(108, 23);
            btnShowRecordings.TabIndex = 13;
            btnShowRecordings.Text = "Show Recordings";
            btnShowRecordings.UseVisualStyleBackColor = true;
            btnShowRecordings.Click += btnShowRecordings_Click;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.Controls.Add(btnStart, 0, 0);
            tableLayoutPanel2.Controls.Add(btnStop, 1, 0);
            tableLayoutPanel2.Controls.Add(btnShowRecordings, 2, 0);
            tableLayoutPanel2.Location = new Point(31, 277);
            tableLayoutPanel2.MaximumSize = new Size(419, 31);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(419, 31);
            tableLayoutPanel2.TabIndex = 15;
            // 
            // cmbMicrophone
            // 
            cmbMicrophone.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbMicrophone.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMicrophone.FormattingEnabled = true;
            cmbMicrophone.Location = new Point(135, 19);
            cmbMicrophone.MaximumSize = new Size(572, 0);
            cmbMicrophone.MinimumSize = new Size(20, 0);
            cmbMicrophone.Name = "cmbMicrophone";
            cmbMicrophone.Size = new Size(312, 23);
            cmbMicrophone.TabIndex = 17;
            // 
            // chkIncludeMicrophone
            // 
            chkIncludeMicrophone.AutoSize = true;
            chkIncludeMicrophone.Location = new Point(31, 23);
            chkIncludeMicrophone.Name = "chkIncludeMicrophone";
            chkIncludeMicrophone.Size = new Size(94, 19);
            chkIncludeMicrophone.TabIndex = 18;
            chkIncludeMicrophone.Text = "Microphone:";
            chkIncludeMicrophone.UseVisualStyleBackColor = true;
            chkIncludeMicrophone.CheckedChanged += cmbMicrophone_SelectedIndexChanged;
            chkIncludeMicrophone.CheckStateChanged += chkIncludeMicrophone_CheckedChanged;
            // 
            // chkAudioOutput
            // 
            chkAudioOutput.AutoSize = true;
            chkAudioOutput.Location = new Point(31, 53);
            chkAudioOutput.Name = "chkAudioOutput";
            chkAudioOutput.Size = new Size(102, 19);
            chkAudioOutput.TabIndex = 19;
            chkAudioOutput.Text = "Audio Output:";
            chkAudioOutput.UseVisualStyleBackColor = true;
            chkAudioOutput.CheckedChanged += chkAudioOutput_CheckedChanged;
            // 
            // btnAudioOutputs
            // 
            btnAudioOutputs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnAudioOutputs.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnAudioOutputs.Location = new Point(135, 50);
            btnAudioOutputs.MaximumSize = new Size(572, 23);
            btnAudioOutputs.MinimumSize = new Size(20, 23);
            btnAudioOutputs.Name = "btnAudioOutputs";
            btnAudioOutputs.Size = new Size(315, 23);
            btnAudioOutputs.TabIndex = 20;
            btnAudioOutputs.Text = "Select Audio Outputs";
            btnAudioOutputs.UseVisualStyleBackColor = true;
            btnAudioOutputs.Click += btnAudioOutputs_Click;
            // 
            // ctxAudioOutputs
            // 
            ctxAudioOutputs.MaximumSize = new Size(0, 400);
            ctxAudioOutputs.Name = "ctxAudioOutputs";
            ctxAudioOutputs.Size = new Size(61, 4);
            ctxAudioOutputs.Closing += ctxAudioOutputs_Closing;
            // 
            // chkApplications
            // 
            chkApplications.AutoSize = true;
            chkApplications.Location = new Point(30, 82);
            chkApplications.Name = "chkApplications";
            chkApplications.Size = new Size(95, 19);
            chkApplications.TabIndex = 21;
            chkApplications.Text = "Applications:";
            chkApplications.UseVisualStyleBackColor = true;
            chkApplications.Click += chkApplications_CheckedChanged;
            // 
            // btnApplications
            // 
            btnApplications.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnApplications.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnApplications.Location = new Point(135, 79);
            btnApplications.MaximumSize = new Size(572, 23);
            btnApplications.MinimumSize = new Size(20, 23);
            btnApplications.Name = "btnApplications";
            btnApplications.Size = new Size(312, 23);
            btnApplications.TabIndex = 22;
            btnApplications.Text = "Select Applications";
            btnApplications.UseVisualStyleBackColor = true;
            btnApplications.Click += btnApplications_Click;
            // 
            // ctxApplications
            // 
            ctxApplications.MaximumSize = new Size(0, 400);
            ctxApplications.Name = "ctxApplications";
            ctxApplications.Size = new Size(61, 4);
            ctxApplications.Closing += ctxApplications_Closing;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(471, 361);
            Controls.Add(btnApplications);
            Controls.Add(chkApplications);
            Controls.Add(btnAudioOutputs);
            Controls.Add(chkAudioOutput);
            Controls.Add(chkIncludeMicrophone);
            Controls.Add(cmbMicrophone);
            Controls.Add(tableLayoutPanel2);
            Controls.Add(lblStatus);
            Controls.Add(btnBrowseFolder);
            Controls.Add(lblSaveFolder);
            Controls.Add(txtSaveFolder);
            Controls.Add(btnChangeHotkey);
            Controls.Add(lblHotkey);
            Controls.Add(label1);
            Controls.Add(cmbRecordingMode);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(341, 365);
            Name = "MainForm";
            Text = "SoundByte Builder";
            Load += MainForm_Load;
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private Button btnStop;
        private ComboBox cmbAudioDevice;
        private ComboBox cmbRecordingMode;
        private Label label1;
        private Label lblHotkey;
        private Button btnChangeHotkey;
        private TextBox txtSaveFolder;
        private Label lblSaveFolder;
        private Button btnBrowseFolder;
        private Label lblStatus;
        private Button btnShowRecordings;
        private TableLayoutPanel tableLayoutPanel2;
        private ComboBox cmbMicrophone;
        private CheckBox chkIncludeMicrophone;
        private CheckBox chkAudioOutput;
        private Button btnAudioOutputs;
        private ContextMenuStrip ctxAudioOutputs;
        private CheckBox chkApplications;
        private Button btnApplications;
        private ContextMenuStrip ctxApplications;
    }
}
