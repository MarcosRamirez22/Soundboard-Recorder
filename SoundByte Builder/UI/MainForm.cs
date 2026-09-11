using NAudio.CoreAudioApi;
using SoundByte_Builder.Audio;
using SoundByte_Builder.Input;
using SoundByte_Builder.Models;
using System.Diagnostics;
using System.Text.Json;
using SoundByte_Builder.Programs;

namespace SoundByte_Builder
{
    public partial class MainForm : Form
    {
        private readonly MMDeviceEnumerator deviceEnumerator = new();
        private readonly List<MMDevice> audioDevices = new();
        private readonly List<MMDevice> microphoneDevices = new();
        private readonly List<RunningApplication>runningApplications = new();

        private readonly RecordingManager recordingManager = new();

        private GlobalKeyboardHook? keyboardHook;

        private Keys recordingHotkey = Keys.F8;

        private bool hotkeyHeld = false;
        private bool waitingForHotkey = false;
        private bool ignoreHotkeyRelease = false;

        private bool isLoadingSettings = true;

        private bool audioOutputsMenuClosedByButton = false;

        private bool applicationsMenuClosedByButton = false;
        private bool isLoadingApplications = false;

        private bool isStartingRecording = false;

        private AppSettings appSettings = new();

        private readonly string settingsFolder = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData
            ),
            "SoundByte Builder"
        );

        private string SettingsFilePath =>
            Path.Combine(
                settingsFolder,
                "settings.json"
            );

        public MainForm()
        {
            InitializeComponent();

            btnStop.Enabled = false;

            cmbRecordingMode.Items.Add("Hold to Record");
            cmbRecordingMode.Items.Add("Press to Toggle");

            recordingManager.StatusChanged +=
                RecordingManager_StatusChanged;

            recordingManager.RecordingSaved +=
                RecordingManager_RecordingSaved;

            recordingManager.RecordingFailed +=
                RecordingManager_RecordingFailed;

            LoadAudioDevices();
            LoadMicrophones();
            LoadSettings();
            LoadRunningApplications();

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
                    string json =
                        File.ReadAllText(SettingsFilePath);

                    AppSettings? loadedSettings =
                        JsonSerializer.Deserialize<AppSettings>(
                            json
                        );

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
                appSettings.RecordingMode <
                cmbRecordingMode.Items.Count)
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

            if (!string.IsNullOrWhiteSpace(
                    appSettings.SaveFolder))
            {
                txtSaveFolder.Text =
                    appSettings.SaveFolder;
            }
            else
            {
                txtSaveFolder.Text =
                    Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.MyMusic
                        ),
                        "SoundByte Builder"
                    );
            }

            foreach (ToolStripMenuItem item in ctxAudioOutputs.Items)
            {
                item.Checked = false;
            }

            foreach (ToolStripMenuItem item in ctxAudioOutputs.Items)
            {
                if (item.Tag is string deviceId &&
                    appSettings.AudioDeviceIds.Contains(deviceId))
                {
                    item.Checked = true;
                }
            }

            chkAudioOutput.Checked =
                appSettings.IncludeAudioOutput;

            btnAudioOutputs.Enabled =
                chkAudioOutput.Checked &&
                audioDevices.Count > 0;

            UpdateAudioOutputsButtonText();

            bool microphoneFound = false;

            if (!string.IsNullOrWhiteSpace(
                    appSettings.MicrophoneDeviceId))
            {
                for (int i = 0;
                     i < microphoneDevices.Count;
                     i++)
                {
                    if (microphoneDevices[i].ID ==
                        appSettings.MicrophoneDeviceId)
                    {
                        cmbMicrophone.SelectedIndex = i;
                        microphoneFound = true;
                        break;
                    }
                }
            }

            chkApplications.Checked =
            appSettings.IncludeApplications;

            btnApplications.Enabled =
                chkApplications.Checked;

            if (!microphoneFound &&
                microphoneDevices.Count > 0)
            {
                cmbMicrophone.SelectedIndex = 0;
            }

            chkIncludeMicrophone.Checked =
                appSettings.IncludeMicrophone;

            cmbMicrophone.Enabled =
                chkIncludeMicrophone.Checked &&
                microphoneDevices.Count > 0;
        }

        private void SaveSettings()
        {
            if (isLoadingSettings)
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(
                    settingsFolder
                );

                appSettings.RecordingMode =
                    cmbRecordingMode.SelectedIndex;

                appSettings.RecordingHotkey =
                    (int)recordingHotkey;

                appSettings.SaveFolder =
                    txtSaveFolder.Text.Trim();

                appSettings.IncludeMicrophone =
                    chkIncludeMicrophone.Checked;

                appSettings.IncludeAudioOutput =
                    chkAudioOutput.Checked;

                appSettings.IncludeApplications =
                    chkApplications.Checked;

                appSettings.AudioDeviceIds.Clear();

                foreach (ToolStripMenuItem item in ctxAudioOutputs.Items)
                {
                    if (item.Checked &&
                        item.Tag is string deviceId)
                    {
                        appSettings.AudioDeviceIds.Add(
                            deviceId
                        );
                    }
                }

                if (cmbMicrophone.SelectedIndex >= 0 &&
                    cmbMicrophone.SelectedIndex <
                    microphoneDevices.Count)
                {
                    appSettings.MicrophoneDeviceId =
                        microphoneDevices[
                            cmbMicrophone.SelectedIndex
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
            ctxAudioOutputs.Items.Clear();
            audioDevices.Clear();

            var devices =
                deviceEnumerator.EnumerateAudioEndPoints(
                    DataFlow.Render,
                    DeviceState.Active
                );

            foreach (var device in devices)
            {
                audioDevices.Add(device);

                var item =
                    new ToolStripMenuItem(
                        device.FriendlyName
                    )
                    {
                        CheckOnClick = true,
                        Tag = device.ID
                    };

                item.CheckedChanged +=
                    AudioOutputItem_CheckedChanged;

                ctxAudioOutputs.Items.Add(item);
            }

            UpdateAudioOutputsButtonText();
        }

        private void LoadMicrophones()
        {
            cmbMicrophone.Items.Clear();
            microphoneDevices.Clear();

            var devices =
                deviceEnumerator.EnumerateAudioEndPoints(
                    DataFlow.Capture,
                    DeviceState.Active
                );

            foreach (var device in devices)
            {
                microphoneDevices.Add(device);

                cmbMicrophone.Items.Add(
                    device.FriendlyName
                );
            }

            if (microphoneDevices.Count > 0)
            {
                cmbMicrophone.SelectedIndex = 0;
            }
            else
            {
                cmbMicrophone.Items.Add(
                    "No microphones found"
                );

                cmbMicrophone.SelectedIndex = 0;

                cmbMicrophone.Enabled = false;
                chkIncludeMicrophone.Enabled = false;
            }
        }

        private void LoadRunningApplications()
        {
            isLoadingApplications = true;

            try
            {
                ctxApplications.Items.Clear();
                runningApplications.Clear();

                var discoveredApplications =
                    ApplicationDiscovery
                        .GetRunningApplications();

                runningApplications.AddRange(
                    discoveredApplications
                );

                var displayedKeys =
                    new HashSet<string>(
                        StringComparer.OrdinalIgnoreCase
                    );

                foreach (var application in discoveredApplications)
                {
                    string key =
                        GetApplicationKey(
                            application.ProcessName,
                            application.ExecutablePath
                        );

                    if (!displayedKeys.Add(key))
                    {
                        continue;
                    }

                    SavedApplication? savedApplication =
                        appSettings.Applications
                            .FirstOrDefault(
                                saved =>
                                    GetApplicationKey(
                                        saved.ProcessName,
                                        saved.ExecutablePath
                                    ) == key
                            );

                    var applicationInfo =
                        new SavedApplication
                        {
                            ProcessName =
                                application.ProcessName,

                            DisplayName =
                                application.DisplayName,

                            ExecutablePath =
                                application.ExecutablePath
                        };

                    var item =
                        new ToolStripMenuItem(
                            application.DisplayName
                        )
                        {
                            CheckOnClick = true,
                            Checked =
                                savedApplication != null,

                            Tag =
                                applicationInfo
                        };

                    item.CheckedChanged +=
                        ApplicationItem_CheckedChanged;

                    ctxApplications.Items.Add(
                        item
                    );
                }

                foreach (SavedApplication savedApplication
                         in appSettings.Applications)
                {
                    string key =
                        GetApplicationKey(
                            savedApplication.ProcessName,
                            savedApplication.ExecutablePath
                        );

                    if (displayedKeys.Contains(key))
                    {
                        continue;
                    }

                    var item =
                        new ToolStripMenuItem(
                            $"{savedApplication.DisplayName} (Offline)"
                        )
                        {
                            CheckOnClick = true,
                            Checked = true,
                            Tag = savedApplication
                        };

                    item.CheckedChanged +=
                        ApplicationItem_CheckedChanged;

                    ctxApplications.Items.Add(
                        item
                    );
                }

                UpdateApplicationsButtonText();
            }
            finally
            {
                isLoadingApplications = false;
            }
        }

        private static string GetApplicationKey(
            string processName,
            string? executablePath)
        {
            if (!string.IsNullOrWhiteSpace(
                    executablePath))
            {
                return executablePath;
            }

            return processName;
        }

        private void ApplicationItem_CheckedChanged(
            object? sender,
            EventArgs e)
        {
            if (isLoadingApplications ||
                sender is not ToolStripMenuItem item ||
                item.Tag is not SavedApplication application)
            {
                return;
            }

            string key =
                GetApplicationKey(
                    application.ProcessName,
                    application.ExecutablePath
                );

            SavedApplication? existing =
                appSettings.Applications
                    .FirstOrDefault(
                        saved =>
                            GetApplicationKey(
                                saved.ProcessName,
                                saved.ExecutablePath
                            ) == key
                    );

            if (item.Checked)
            {
                if (existing == null)
                {
                    appSettings.Applications.Add(
                        new SavedApplication
                        {
                            ProcessName =
                                application.ProcessName,

                            DisplayName =
                                application.DisplayName,

                            ExecutablePath =
                                application.ExecutablePath
                        }
                    );
                }
            }
            else
            {
                if (existing != null)
                {
                    appSettings.Applications.Remove(
                        existing
                    );
                }
            }

            UpdateApplicationsButtonText();
            SaveSettings();
        }

        private void AudioOutputItem_CheckedChanged(
            object? sender,
            EventArgs e)
        {
            UpdateAudioOutputsButtonText();
            SaveSettings();
        }

        private void btnAudioOutputs_Click(
            object sender,
            EventArgs e)
        {
            if (audioOutputsMenuClosedByButton)
            {
                audioOutputsMenuClosedByButton = false;
                return;
            }

            ctxAudioOutputs.Show(
                btnAudioOutputs,
                new Point(
                    0,
                    btnAudioOutputs.Height
                )
            );
        }

        private void chkAudioOutput_CheckedChanged(
            object sender,
            EventArgs e)
        {
            btnAudioOutputs.Enabled =
                chkAudioOutput.Checked &&
                audioDevices.Count > 0;

            SaveSettings();
        }

        private void btnApplications_Click(
            object sender,
            EventArgs e)
        {
            if (applicationsMenuClosedByButton)
            {
                applicationsMenuClosedByButton = false;
                return;
            }

            LoadRunningApplications();

            ctxApplications.Show(
                btnApplications,
                new Point(
                    0,
                    btnApplications.Height
                )
            );
        }

        private void ctxApplications_Closing(
            object sender,
            ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason ==
                ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
                return;
            }

            if (e.CloseReason ==
                ToolStripDropDownCloseReason.AppClicked)
            {
                Point mousePosition =
                    btnApplications.PointToClient(
                        Cursor.Position
                    );

                if (btnApplications
                    .ClientRectangle
                    .Contains(mousePosition))
                {
                    applicationsMenuClosedByButton = true;
                }
            }
        }

        private void ctxAudioOutputs_Closing(
            object sender,
            ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason ==
                ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
                return;
            }

            if (e.CloseReason ==
                ToolStripDropDownCloseReason.AppClicked)
            {
                Point mousePosition =
                    btnAudioOutputs.PointToClient(
                        Cursor.Position
                    );

                if (btnAudioOutputs.ClientRectangle.Contains(
                        mousePosition))
                {
                    audioOutputsMenuClosedByButton = true;
                }
            }
        }

        private void chkApplications_CheckedChanged(
            object sender,
            EventArgs e)
        {
            btnApplications.Enabled =
                chkApplications.Checked;

            SaveSettings();
        }

        private void cmbMicrophone_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            SaveSettings();
        }

        private void chkIncludeMicrophone_CheckedChanged(
            object sender,
            EventArgs e)
        {
            cmbMicrophone.Enabled =
                chkIncludeMicrophone.Checked &&
                microphoneDevices.Count > 0;

            SaveSettings();
        }

        private void UpdateAudioOutputsButtonText()
        {
            var selectedItems =
                ctxAudioOutputs.Items
                    .OfType<ToolStripMenuItem>()
                    .Where(item => item.Checked)
                    .ToList();

            if (selectedItems.Count == 0)
            {
                btnAudioOutputs.Text =
                    "Select Audio Outputs";
            }
            else if (selectedItems.Count == 1)
            {
                btnAudioOutputs.Text =
                    selectedItems[0].Text;
            }
            else
            {
                btnAudioOutputs.Text =
                    $"{selectedItems[0].Text} + {selectedItems.Count - 1} more";
            }
        }

        private void UpdateApplicationsButtonText()
        {
            var selectedItems =
                ctxApplications.Items
                    .OfType<ToolStripMenuItem>()
                    .Where(item => item.Checked)
                    .ToList();

            if (selectedItems.Count == 0)
            {
                btnApplications.Text =
                    "Select Applications";
            }
            else if (selectedItems.Count == 1)
            {
                btnApplications.Text =
                    selectedItems[0].Text;
            }
            else
            {
                btnApplications.Text =
                    $"{selectedItems[0].Text} + {selectedItems.Count - 1} more";
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

        private async Task StartRecording()
        {
            if (recordingManager.IsRecording ||
                isStartingRecording)
            {
                return;
            }

            if (!chkAudioOutput.Checked &&
                !chkIncludeMicrophone.Checked &&
                !chkApplications.Checked)
            {
                lblStatus.Text =
                    "Please select at least one recording source.";

                return;
            }

            List<RunningApplication> selectedApplications =
                chkApplications.Checked
                    ? ApplicationDiscovery.ResolveSavedApplications(
                        appSettings.Applications
                    )
                    : new List<RunningApplication>();

            bool recordApplications =
                chkApplications.Checked &&
                selectedApplications.Count > 0;

            if (!chkAudioOutput.Checked &&
                !chkIncludeMicrophone.Checked &&
                !recordApplications)
            {
                lblStatus.Text =
                    "None of the selected applications are currently running.";

                return;
            }

            string recordingsFolder =
                txtSaveFolder.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                    recordingsFolder))
            {
                lblStatus.Text =
                    "Please choose a save folder.";

                return;
            }

            List<MMDevice> selectedOutputDevices =
                new();

            if (chkAudioOutput.Checked)
            {
                foreach (ToolStripMenuItem item in ctxAudioOutputs.Items)
                {
                    if (!item.Checked ||
                        item.Tag is not string deviceId)
                    {
                        continue;
                    }

                    MMDevice? device =
                        audioDevices.FirstOrDefault(
                            d => d.ID == deviceId
                        );

                    if (device != null)
                    {
                        selectedOutputDevices.Add(
                            device
                        );
                    }
                }

                if (selectedOutputDevices.Count == 0)
                {
                    lblStatus.Text =
                        "Please select at least one audio output device.";

                    return;
                }
            }

            MMDevice? microphoneDevice = null;

            if (chkIncludeMicrophone.Checked)
            {
                if (cmbMicrophone.SelectedIndex < 0 ||
                    cmbMicrophone.SelectedIndex >=
                    microphoneDevices.Count)
                {
                    lblStatus.Text =
                        "Please select a microphone.";

                    return;
                }

                microphoneDevice =
                    microphoneDevices[
                        cmbMicrophone.SelectedIndex
                    ];
            }

            isStartingRecording = true;
            SetRecordingControlsEnabled(false);

            try
            {
                await recordingManager.StartRecording(
                    selectedOutputDevices,
                    chkAudioOutput.Checked,
                    microphoneDevice,
                    chkIncludeMicrophone.Checked,
                    selectedApplications,
                    recordApplications,
                    recordingsFolder
                );
            }
            catch (Exception ex)
            {
                SetRecordingControlsEnabled(true);

                lblStatus.Text =
                    "Could not start recording.";

                MessageBox.Show(
                    $"Could not start recording:\n\n{ex.Message}",
                    "Recording Error"
                );
            }
            finally
            {
                isStartingRecording = false;
            }
        }

        private void StopRecording()
        {
            recordingManager.StopRecording();
        }

        private void RecordingManager_StatusChanged(
            string status)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() =>
                    RecordingManager_StatusChanged(
                        status
                    )
                );

                return;
            }

            lblStatus.Text = status;
        }

        private void RecordingManager_RecordingSaved(
            string filePath,
            int sampleRate,
            int channels)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() =>
                    RecordingManager_RecordingSaved(
                        filePath,
                        sampleRate,
                        channels
                    )
                );

                return;
            }

            SetRecordingControlsEnabled(
                true
            );

            lblStatus.Text =
                $"Saved: " +
                $"{Path.GetFileName(filePath)} " +
                $"({sampleRate} Hz, " +
                $"{channels} channel(s))";
        }

        private void RecordingManager_RecordingFailed(
            Exception exception)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() =>
                    RecordingManager_RecordingFailed(
                        exception
                    )
                );

                return;
            }

            SetRecordingControlsEnabled(
                true
            );

            lblStatus.Text =
                "Recording failed.";

            MessageBox.Show(
                $"Recording failed:\n\n{exception.Message}",
                "Recording Error"
            );
        }

        private void SetRecordingControlsEnabled(
            bool enabled)
        {
            btnStart.Enabled = enabled;
            btnStop.Enabled = !enabled;

            cmbRecordingMode.Enabled = enabled;
            btnChangeHotkey.Enabled = enabled;

            txtSaveFolder.Enabled = enabled;
            btnBrowseFolder.Enabled = enabled;

            chkAudioOutput.Enabled =
                enabled &&
                audioDevices.Count > 0;

            btnAudioOutputs.Enabled =
                enabled &&
                chkAudioOutput.Checked &&
                audioDevices.Count > 0;

            chkIncludeMicrophone.Enabled =
                enabled &&
                microphoneDevices.Count > 0;

            cmbMicrophone.Enabled =
                enabled &&
                chkIncludeMicrophone.Checked &&
                microphoneDevices.Count > 0;

            chkApplications.Enabled =
                enabled;

            btnApplications.Enabled =
                enabled &&
                chkApplications.Checked;
        }

        private async void GlobalKeyDown(
            Keys key)
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
                BeginInvoke(async () =>
                    await HandleHotkeyDown()
                );
            }
            else
            {
                await HandleHotkeyDown();
            }
        }

        private void GlobalKeyUp(
            Keys key)
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

        private async Task HandleHotkeyDown()
        {
            if (cmbRecordingMode.SelectedIndex == 0)
            {
                await StartRecording();
            }
            else
            {
                if (recordingManager.IsRecording)
                {
                    StopRecording();
                }
                else
                {
                    await StartRecording();
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

        private void SetNewHotkey(
            Keys key)
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
            using FolderBrowserDialog folderDialog =
                new();

            folderDialog.Description =
                "Choose where recordings should be saved.";

            folderDialog.ShowNewFolderButton =
                true;

            if (Directory.Exists(
                    txtSaveFolder.Text))
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

            if (string.IsNullOrWhiteSpace(
                    folderPath))
            {
                lblStatus.Text =
                    "No save folder selected.";

                return;
            }

            if (!Directory.Exists(folderPath))
            {
                try
                {
                    Directory.CreateDirectory(
                        folderPath
                    );
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

        private void cmbRecordingMode_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            SaveSettings();
        }

        private async void btnStart_Click(
            object sender,
            EventArgs e)
        {
            await StartRecording();
        }

        private void btnStop_Click(
            object sender,
            EventArgs e)
        {
            StopRecording();
        }

        private void MainForm_Load(
            object sender,
            EventArgs e)
        {
        }

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            SaveSettings();

            recordingManager.Dispose();

            keyboardHook?.Dispose();
            deviceEnumerator.Dispose();

            base.OnFormClosed(e);
        }
    }
}