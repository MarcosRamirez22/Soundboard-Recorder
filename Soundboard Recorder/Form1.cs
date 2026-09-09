using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Text.Json;

namespace Soundboard_Recorder
{
    public partial class Form1 : Form
    {
        private WasapiLoopbackCapture? capture;
        private WaveFileWriter? writer;

        private readonly MMDeviceEnumerator deviceEnumerator = new();
        private readonly List<MMDevice> audioDevices = new();

        private bool isRecording = false;
        private string? currentFilePath;

        private GlobalKeyboardHook? keyboardHook;

        private Keys recordingHotkey = Keys.F8;

        private bool hotkeyHeld = false;
        private bool waitingForHotkey = false;
        private bool ignoreHotkeyRelease = false;

        private bool isLoadingSettings = true;

        private AppSettings appSettings = new();

        private readonly string settingsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Soundboard Recorder"
        );

        private string SettingsFilePath =>
            Path.Combine(settingsFolder, "settings.json");

        public Form1()
        {
            InitializeComponent();

            btnStop.Enabled = false;

            cmbRecordingMode.Items.Add("Hold to Record");
            cmbRecordingMode.Items.Add("Press to Toggle");

            LoadAudioDevices();
            LoadSettings();

            UpdateHotkeyLabel();

            keyboardHook = new GlobalKeyboardHook();

            keyboardHook.KeyDown += GlobalKeyDown;
            keyboardHook.KeyUp += GlobalKeyUp;

            lblStatus.Text = "Ready";

            isLoadingSettings = false;
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);

                    AppSettings? loadedSettings =
                        JsonSerializer.Deserialize<AppSettings>(json);

                    if (loadedSettings != null)
                    {
                        appSettings = loadedSettings;
                    }
                }
            }
            catch
            {
                appSettings = new AppSettings();
            }

            if (appSettings.RecordingMode >= 0 &&
                appSettings.RecordingMode < cmbRecordingMode.Items.Count)
            {
                cmbRecordingMode.SelectedIndex =
                    appSettings.RecordingMode;
            }
            else
            {
                cmbRecordingMode.SelectedIndex = 0;
            }

            recordingHotkey =
                (Keys)appSettings.RecordingHotkey;

            if (!string.IsNullOrWhiteSpace(appSettings.SaveFolder))
            {
                txtSaveFolder.Text =
                    appSettings.SaveFolder;
            }
            else
            {
                txtSaveFolder.Text = Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.MyMusic
                    ),
                    "Soundboard Recorder"
                );
            }

            bool deviceFound = false;

            if (!string.IsNullOrWhiteSpace(appSettings.AudioDeviceId))
            {
                for (int i = 0; i < audioDevices.Count; i++)
                {
                    if (audioDevices[i].ID ==
                        appSettings.AudioDeviceId)
                    {
                        cmbAudioDevice.SelectedIndex = i;
                        deviceFound = true;
                        break;
                    }
                }
            }

            if (!deviceFound && cmbAudioDevice.Items.Count > 0)
            {
                cmbAudioDevice.SelectedIndex = 0;
            }
        }

        private void SaveSettings()
        {
            if (isLoadingSettings)
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(settingsFolder);

                appSettings.RecordingMode =
                    cmbRecordingMode.SelectedIndex;

                appSettings.RecordingHotkey =
                    (int)recordingHotkey;

                appSettings.SaveFolder =
                    txtSaveFolder.Text.Trim();

                if (cmbAudioDevice.SelectedIndex >= 0 &&
                    cmbAudioDevice.SelectedIndex < audioDevices.Count)
                {
                    appSettings.AudioDeviceId =
                        audioDevices[
                            cmbAudioDevice.SelectedIndex
                        ].ID;
                }

                string json =
                    JsonSerializer.Serialize(
                        appSettings,
                        new JsonSerializerOptions
                        {
                            WriteIndented = true
                        }
                    );

                File.WriteAllText(
                    SettingsFilePath,
                    json
                );
            }
            catch (Exception ex)
            {
                lblStatus.Text =
                    $"Could not save settings: {ex.Message}";
            }
        }

        private void LoadAudioDevices()
        {
            cmbAudioDevice.Items.Clear();
            audioDevices.Clear();

            var devices =
                deviceEnumerator.EnumerateAudioEndPoints(
                    DataFlow.Render,
                    DeviceState.Active
                );

            foreach (var device in devices)
            {
                audioDevices.Add(device);
                cmbAudioDevice.Items.Add(device.FriendlyName);
            }
        }

        private void UpdateHotkeyLabel()
        {
            if (waitingForHotkey)
            {
                lblHotkey.Text =
                    "Recording Hotkey: Press any key...";
            }
            else
            {
                lblHotkey.Text =
                    $"Recording Hotkey: {recordingHotkey}";
            }
        }

        private void StartRecording()
        {
            if (isRecording)
            {
                return;
            }

            if (cmbAudioDevice.SelectedIndex < 0)
            {
                lblStatus.Text =
                    "Please select an audio output device.";

                return;
            }

            string recordingsFolder =
                txtSaveFolder.Text.Trim();

            if (string.IsNullOrWhiteSpace(recordingsFolder))
            {
                lblStatus.Text =
                    "Please choose a save folder.";

                return;
            }

            try
            {
                Directory.CreateDirectory(recordingsFolder);
            }
            catch (Exception ex)
            {
                lblStatus.Text =
                    "Could not access the save folder.";

                MessageBox.Show(
                    $"Could not access the selected save folder:\n\n{ex.Message}",
                    "Save Folder Error"
                );

                return;
            }

            string fileName =
                $"Recording_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.wav";

            currentFilePath = Path.Combine(
                recordingsFolder,
                fileName
            );

            MMDevice selectedDevice =
                audioDevices[cmbAudioDevice.SelectedIndex];

            try
            {
                capture =
                    new WasapiLoopbackCapture(selectedDevice);

                writer =
                    new WaveFileWriter(
                        currentFilePath,
                        capture.WaveFormat
                    );

                capture.DataAvailable +=
                    Capture_DataAvailable;

                capture.RecordingStopped +=
                    Capture_RecordingStopped;

                capture.StartRecording();

                isRecording = true;

                lblStatus.Text = "Recording...";

                btnStart.Enabled = false;
                btnStop.Enabled = true;

                cmbAudioDevice.Enabled = false;
                cmbRecordingMode.Enabled = false;
                btnChangeHotkey.Enabled = false;

                txtSaveFolder.Enabled = false;
                btnBrowseFolder.Enabled = false;
            }
            catch (Exception ex)
            {
                CleanupRecording();

                lblStatus.Text =
                    "Could not start recording.";

                MessageBox.Show(
                    $"Could not start recording:\n\n{ex.Message}",
                    "Recording Error"
                );
            }
        }

        private void StopRecording()
        {
            if (!isRecording)
            {
                return;
            }

            lblStatus.Text =
                "Saving recording...";

            capture?.StopRecording();
        }

        private void Capture_DataAvailable(
            object? sender,
            WaveInEventArgs e)
        {
            writer?.Write(
                e.Buffer,
                0,
                e.BytesRecorded
            );
        }

        private void Capture_RecordingStopped(
            object? sender,
            StoppedEventArgs e)
        {
            string? savedFilePath =
                currentFilePath;

            CleanupRecording();

            if (InvokeRequired)
            {
                BeginInvoke(() =>
                    RecordingFinished(
                        savedFilePath,
                        e.Exception
                    )
                );
            }
            else
            {
                RecordingFinished(
                    savedFilePath,
                    e.Exception
                );
            }
        }

        private void RecordingFinished(
            string? filePath,
            Exception? exception)
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;

            cmbAudioDevice.Enabled = true;
            cmbRecordingMode.Enabled = true;
            btnChangeHotkey.Enabled = true;

            txtSaveFolder.Enabled = true;
            btnBrowseFolder.Enabled = true;

            if (exception != null)
            {
                lblStatus.Text =
                    "Recording failed.";

                MessageBox.Show(
                    $"Recording stopped because of an error:\n\n{exception.Message}",
                    "Recording Error"
                );

                return;
            }

            if (filePath != null)
            {
                lblStatus.Text =
                    $"Saved: {Path.GetFileName(filePath)}";
            }
            else
            {
                lblStatus.Text = "Ready";
            }
        }

        private void CleanupRecording()
        {
            writer?.Dispose();
            writer = null;

            if (capture != null)
            {
                capture.DataAvailable -=
                    Capture_DataAvailable;

                capture.RecordingStopped -=
                    Capture_RecordingStopped;

                capture.Dispose();
                capture = null;
            }

            isRecording = false;
            currentFilePath = null;
        }

        private void GlobalKeyDown(Keys key)
        {
            if (waitingForHotkey)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(() =>
                        SetNewHotkey(key)
                    );
                }
                else
                {
                    SetNewHotkey(key);
                }

                return;
            }

            if (key != recordingHotkey)
            {
                return;
            }

            if (hotkeyHeld)
            {
                return;
            }

            hotkeyHeld = true;

            if (InvokeRequired)
            {
                BeginInvoke(() =>
                    HandleHotkeyDown()
                );
            }
            else
            {
                HandleHotkeyDown();
            }
        }

        private void GlobalKeyUp(Keys key)
        {
            if (key != recordingHotkey)
            {
                return;
            }

            if (ignoreHotkeyRelease)
            {
                ignoreHotkeyRelease = false;
                hotkeyHeld = false;

                return;
            }

            hotkeyHeld = false;

            if (InvokeRequired)
            {
                BeginInvoke(() =>
                    HandleHotkeyUp()
                );
            }
            else
            {
                HandleHotkeyUp();
            }
        }

        private void HandleHotkeyDown()
        {
            if (cmbRecordingMode.SelectedIndex == 0)
            {
                // Hold to Record
                StartRecording();
            }
            else
            {
                // Press to Toggle
                if (isRecording)
                {
                    StopRecording();
                }
                else
                {
                    StartRecording();
                }
            }
        }

        private void HandleHotkeyUp()
        {
            if (cmbRecordingMode.SelectedIndex == 0)
            {
                StopRecording();
            }
        }

        private void btnChangeHotkey_Click(
            object sender,
            EventArgs e)
        {
            waitingForHotkey = true;
            hotkeyHeld = false;

            UpdateHotkeyLabel();

            btnChangeHotkey.Enabled = false;
        }

        private void SetNewHotkey(Keys key)
        {
            recordingHotkey = key;

            waitingForHotkey = false;
            ignoreHotkeyRelease = true;

            UpdateHotkeyLabel();

            btnChangeHotkey.Enabled = true;

            SaveSettings();
        }

        private void btnBrowseFolder_Click(
            object sender,
            EventArgs e)
        {
            using FolderBrowserDialog folderDialog = new();

            folderDialog.Description =
                "Choose where recordings should be saved.";

            folderDialog.ShowNewFolderButton = true;

            if (Directory.Exists(txtSaveFolder.Text))
            {
                folderDialog.SelectedPath =
                    txtSaveFolder.Text;
            }

            if (folderDialog.ShowDialog() ==
                DialogResult.OK)
            {
                txtSaveFolder.Text =
                    folderDialog.SelectedPath;

                lblStatus.Text =
                    "Save folder changed.";

                SaveSettings();
            }
        }

        private void btnShowRecordings_Click(
            object sender,
            EventArgs e)
        {
            string folderPath =
                txtSaveFolder.Text.Trim();

            if (string.IsNullOrWhiteSpace(folderPath))
            {
                lblStatus.Text =
                    "No save folder selected.";

                return;
            }

            if (!Directory.Exists(folderPath))
            {
                try
                {
                    Directory.CreateDirectory(folderPath);
                }
                catch
                {
                    lblStatus.Text =
                        "Save folder does not exist.";

                    return;
                }
            }

            try
            {
                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = folderPath,
                        UseShellExecute = true
                    }
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not open the recordings folder:\n\n{ex.Message}",
                    "Folder Error"
                );
            }
        }

        private void cmbAudioDevice_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            SaveSettings();
        }

        private void cmbRecordingMode_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            SaveSettings();
        }

        private void btnStart_Click(
            object sender,
            EventArgs e)
        {
            StartRecording();
        }

        private void btnStop_Click(
            object sender,
            EventArgs e)
        {
            StopRecording();
        }

        private void Form1_Load(
            object sender,
            EventArgs e)
        {
        }

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            SaveSettings();

            keyboardHook?.Dispose();
            deviceEnumerator.Dispose();

            base.OnFormClosed(e);
        }
    }
}