namespace Soundboard_Recorder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnStart = new Button();
            btnStop = new Button();
            cmbAudioDevice = new ComboBox();
            AudioOutput = new Label();
            cmbRecordingMode = new ComboBox();
            label1 = new Label();
            lblHotkey = new Label();
            btnChangeHotkey = new Button();
            txtSaveFolder = new TextBox();
            lblSaveFolder = new Label();
            btnBrowseFolder = new Button();
            lblStatus = new Label();
            btnShowRecordings = new Button();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(47, 240);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(100, 23);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start Recording";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(153, 240);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(97, 23);
            btnStop.TabIndex = 1;
            btnStop.Text = "Stop Recording";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // cmbAudioDevice
            // 
            cmbAudioDevice.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAudioDevice.FormattingEnabled = true;
            cmbAudioDevice.Location = new Point(120, 27);
            cmbAudioDevice.Name = "cmbAudioDevice";
            cmbAudioDevice.Size = new Size(330, 23);
            cmbAudioDevice.TabIndex = 2;
            cmbAudioDevice.SelectedIndexChanged += cmbAudioDevice_SelectedIndexChanged;
            // 
            // AudioOutput
            // 
            AudioOutput.AutoSize = true;
            AudioOutput.Location = new Point(31, 30);
            AudioOutput.Name = "AudioOutput";
            AudioOutput.Size = new Size(83, 15);
            AudioOutput.TabIndex = 3;
            AudioOutput.Text = "Audio Output:";
            // 
            // cmbRecordingMode
            // 
            cmbRecordingMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRecordingMode.FormattingEnabled = true;
            cmbRecordingMode.Location = new Point(135, 67);
            cmbRecordingMode.Name = "cmbRecordingMode";
            cmbRecordingMode.Size = new Size(315, 23);
            cmbRecordingMode.TabIndex = 4;
            cmbRecordingMode.SelectedIndexChanged += cmbRecordingMode_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(31, 70);
            label1.Name = "label1";
            label1.Size = new Size(98, 15);
            label1.TabIndex = 5;
            label1.Text = "Recording Mode:";
            // 
            // lblHotkey
            // 
            lblHotkey.AutoSize = true;
            lblHotkey.Location = new Point(31, 109);
            lblHotkey.Name = "lblHotkey";
            lblHotkey.Size = new Size(105, 15);
            lblHotkey.TabIndex = 6;
            lblHotkey.Text = "Recording Hotkey:";
            // 
            // btnChangeHotkey
            // 
            btnChangeHotkey.Location = new Point(350, 105);
            btnChangeHotkey.Name = "btnChangeHotkey";
            btnChangeHotkey.Size = new Size(100, 23);
            btnChangeHotkey.TabIndex = 8;
            btnChangeHotkey.Text = "Change Hotkey";
            btnChangeHotkey.UseVisualStyleBackColor = true;
            btnChangeHotkey.Click += btnChangeHotkey_Click;
            // 
            // txtSaveFolder
            // 
            txtSaveFolder.Location = new Point(107, 137);
            txtSaveFolder.Name = "txtSaveFolder";
            txtSaveFolder.Size = new Size(343, 23);
            txtSaveFolder.TabIndex = 9;
            // 
            // lblSaveFolder
            // 
            lblSaveFolder.AutoSize = true;
            lblSaveFolder.Location = new Point(31, 140);
            lblSaveFolder.Name = "lblSaveFolder";
            lblSaveFolder.Size = new Size(70, 15);
            lblSaveFolder.TabIndex = 10;
            lblSaveFolder.Text = "Save Folder:";
            // 
            // btnBrowseFolder
            // 
            btnBrowseFolder.Location = new Point(107, 166);
            btnBrowseFolder.Name = "btnBrowseFolder";
            btnBrowseFolder.Size = new Size(75, 23);
            btnBrowseFolder.TabIndex = 11;
            btnBrowseFolder.Text = "Browse...";
            btnBrowseFolder.UseVisualStyleBackColor = true;
            btnBrowseFolder.Click += btnBrowseFolder_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(47, 286);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 15);
            lblStatus.TabIndex = 12;
            // 
            // btnShowRecordings
            // 
            btnShowRecordings.Location = new Point(256, 240);
            btnShowRecordings.Name = "btnShowRecordings";
            btnShowRecordings.Size = new Size(107, 23);
            btnShowRecordings.TabIndex = 13;
            btnShowRecordings.Text = "Show Recordings";
            btnShowRecordings.UseVisualStyleBackColor = true;
            btnShowRecordings.Click += btnShowRecordings_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(477, 332);
            Controls.Add(btnShowRecordings);
            Controls.Add(lblStatus);
            Controls.Add(btnBrowseFolder);
            Controls.Add(lblSaveFolder);
            Controls.Add(txtSaveFolder);
            Controls.Add(btnChangeHotkey);
            Controls.Add(lblHotkey);
            Controls.Add(label1);
            Controls.Add(cmbRecordingMode);
            Controls.Add(AudioOutput);
            Controls.Add(cmbAudioDevice);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Soundboard Recorder";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private Button btnStop;
        private ComboBox cmbAudioDevice;
        private Label AudioOutput;
        private ComboBox cmbRecordingMode;
        private Label label1;
        private Label lblHotkey;
        private Button btnChangeHotkey;
        private TextBox txtSaveFolder;
        private Label lblSaveFolder;
        private Button btnBrowseFolder;
        private Label lblStatus;
        private Button btnShowRecordings;
    }
}
