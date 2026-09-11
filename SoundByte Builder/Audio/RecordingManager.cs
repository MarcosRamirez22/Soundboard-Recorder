using NAudio.CoreAudioApi;
using NAudio.Wave;
using SoundByte_Builder.Models;

namespace SoundByte_Builder.Audio
{
    public class RecordingManager : IDisposable
    {
        private class OutputRecordingSource
        {
            public WasapiLoopbackCapture? Capture { get; set; }
            public WaveFileWriter? Writer { get; set; }
            public string TemporaryPath { get; set; } = "";
            public bool Stopped { get; set; }
            public Exception? Exception { get; set; }
        }

        private class ApplicationRecordingSource
        {
            public RunningApplication Application { get; set; } = new();

            public WasapiRecorder? Recorder { get; set; }

            public WaveFileWriter? Writer { get; set; }

            public string TemporaryPath { get; set; } = "";

            public bool Stopped { get; set; }

            public Exception? Exception { get; set; }
        }

        private readonly List<OutputRecordingSource> outputSources = new();
        private readonly List<ApplicationRecordingSource> applicationSources = new();

        private WasapiCapture? microphoneCapture;
        private WaveFileWriter? microphoneWriter;

        private string? microphoneTemporaryPath;
        private string? finalFilePath;

        private bool includeOutput;
        private bool includeMicrophone;
        private bool includeApplications;

        private bool microphoneStopped;
        private Exception? microphoneException;

        private bool finishStarted;

        private readonly object stopLock = new();

        public bool IsRecording { get; private set; }

        public event Action<string>? StatusChanged;
        public event Action<string, int, int>? RecordingSaved;
        public event Action<Exception>? RecordingFailed;

        public async Task StartRecording(
            IReadOnlyList<MMDevice> outputDevices,
            bool recordOutput,
            MMDevice? microphoneDevice,
            bool recordMicrophone,
            IReadOnlyList<RunningApplication> applications,
            bool recordApplications,
            string recordingsFolder)
        {
            if (IsRecording)
            {
                return;
            }

            if (!recordOutput &&
                !recordMicrophone &&
                !recordApplications)
            {
                throw new InvalidOperationException(
                    "At least one recording source must be selected."
                );
            }

            if (recordOutput &&
                outputDevices.Count == 0)
            {
                throw new InvalidOperationException(
                    "At least one audio output device must be selected."
                );
            }

            if (recordMicrophone &&
                microphoneDevice == null)
            {
                throw new InvalidOperationException(
                    "A microphone must be selected."
                );
            }

            if (recordApplications &&
                applications.Count == 0)
            {
                throw new InvalidOperationException(
                    "None of the selected applications are currently running."
                );
            }

            Directory.CreateDirectory(
                recordingsFolder
            );

            string baseFileName =
                $"Recording_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";

            finalFilePath =
                Path.Combine(
                    recordingsFolder,
                    $"{baseFileName}.wav"
                );

            microphoneTemporaryPath =
                Path.Combine(
                    recordingsFolder,
                    $"{baseFileName}_mic_temp.wav"
                );

            includeOutput = recordOutput;
            includeMicrophone = recordMicrophone;
            includeApplications = recordApplications;

            microphoneStopped =
                !includeMicrophone;

            microphoneException = null;
            finishStarted = false;

            outputSources.Clear();
            applicationSources.Clear();

            try
            {
                if (includeOutput)
                {
                    for (int i = 0;
                         i < outputDevices.Count;
                         i++)
                    {
                        MMDevice device =
                            outputDevices[i];

                        string temporaryPath =
                            Path.Combine(
                                recordingsFolder,
                                $"{baseFileName}_output_{i + 1}_temp.wav"
                            );

                        var capture =
                            new WasapiLoopbackCapture(
                                device
                            );

                        var writer =
                            new WaveFileWriter(
                                temporaryPath,
                                capture.WaveFormat
                            );

                        var source =
                            new OutputRecordingSource
                            {
                                Capture = capture,
                                Writer = writer,
                                TemporaryPath = temporaryPath,
                                Stopped = false
                            };

                        capture.DataAvailable +=
                            OutputCapture_DataAvailable;

                        capture.RecordingStopped +=
                            OutputCapture_RecordingStopped;

                        outputSources.Add(
                            source
                        );
                    }
                }

                if (includeMicrophone)
                {
                    microphoneCapture =
                        new WasapiCapture(
                            microphoneDevice!
                        );

                    microphoneWriter =
                        new WaveFileWriter(
                            microphoneTemporaryPath,
                            microphoneCapture.WaveFormat
                        );

                    microphoneCapture.DataAvailable +=
                        MicrophoneCapture_DataAvailable;

                    microphoneCapture.RecordingStopped +=
                        MicrophoneCapture_RecordingStopped;
                }

                if (includeApplications)
                {
                    for (int i = 0;
                         i < applications.Count;
                         i++)
                    {
                        RunningApplication application =
                            applications[i];

                        string temporaryPath =
                            Path.Combine(
                                recordingsFolder,
                                $"{baseFileName}_application_{i + 1}_temp.wav"
                            );

                        WasapiRecorder recorder =
                            await Task.Run(async () =>
                                await new WasapiRecorderBuilder()
                                    .WithProcessLoopback(
                                        (uint)application.ProcessId,
                                        ProcessLoopbackMode.IncludeTargetProcessTree
                                    )
                                    .BuildAsync()
                            );

                        var writer =
                            new WaveFileWriter(
                                temporaryPath,
                                recorder.WaveFormat
                            );

                        var source =
                            new ApplicationRecordingSource
                            {
                                Application = application,
                                Recorder = recorder,
                                Writer = writer,
                                TemporaryPath = temporaryPath,
                                Stopped = false
                            };

                        recorder.DataAvailable +=
                            (buffer, flags, devicePosition, qpcPosition) =>
                            {
                                source.Writer?.Write(
                                    buffer
                                );
                            };

                        recorder.RecordingStopped +=
                            (sender, e) =>
                            {
                                ApplicationCapture_RecordingStopped(
                                    source,
                                    e
                                );
                            };

                        applicationSources.Add(
                            source
                        );
                    }
                }

                foreach (var source in outputSources)
                {
                    source.Capture!.StartRecording();
                }

                if (includeMicrophone)
                {
                    microphoneCapture!.StartRecording();
                }

                foreach (var source in applicationSources)
                {
                    source.Recorder!.StartRecording();
                }

                IsRecording = true;

                UpdateRecordingStatus();
            }
            catch
            {
                CleanupAll();
                DeleteTemporaryFiles();

                throw;
            }
        }

        public void StopRecording()
        {
            if (!IsRecording)
            {
                return;
            }

            IsRecording = false;

            StatusChanged?.Invoke(
                "Saving recording..."
            );

            foreach (var source in outputSources)
            {
                source.Capture?.StopRecording();
            }

            if (includeMicrophone)
            {
                microphoneCapture?.StopRecording();
            }

            foreach (var source in applicationSources)
            {
                source.Recorder?.StopRecording();
            }
        }

        private void UpdateRecordingStatus()
        {
            int sourceCount =
                outputSources.Count +
                applicationSources.Count +
                (includeMicrophone ? 1 : 0);

            if (applicationSources.Count > 0 &&
                outputSources.Count == 0 &&
                !includeMicrophone)
            {
                if (applicationSources.Count == 1)
                {
                    StatusChanged?.Invoke(
                        $"Recording {applicationSources[0].Application.DisplayName}..."
                    );
                }
                else
                {
                    StatusChanged?.Invoke(
                        $"Recording {applicationSources.Count} Applications..."
                    );
                }

                return;
            }

            if (sourceCount > 1)
            {
                StatusChanged?.Invoke(
                    $"Recording {sourceCount} Sources..."
                );

                return;
            }

            if (outputSources.Count == 1)
            {
                var format =
                    outputSources[0]
                        .Capture!
                        .WaveFormat;

                StatusChanged?.Invoke(
                    $"Recording Audio... " +
                    $"{format.SampleRate} Hz, " +
                    $"{format.Channels} channel(s)"
                );

                return;
            }

            if (includeMicrophone)
            {
                StatusChanged?.Invoke(
                    $"Recording Microphone... " +
                    $"{microphoneCapture!.WaveFormat.SampleRate} Hz, " +
                    $"{microphoneCapture.WaveFormat.Channels} channel(s)"
                );
            }
        }

        private void OutputCapture_DataAvailable(
            object? sender,
            WaveInEventArgs e)
        {
            OutputRecordingSource? source =
                outputSources.FirstOrDefault(
                    item =>
                        ReferenceEquals(
                            item.Capture,
                            sender
                        )
                );

            source?.Writer?.Write(
                e.Buffer,
                0,
                e.BytesRecorded
            );
        }

        private void MicrophoneCapture_DataAvailable(
            object? sender,
            WaveInEventArgs e)
        {
            microphoneWriter?.Write(
                e.Buffer,
                0,
                e.BytesRecorded
            );
        }

        private void OutputCapture_RecordingStopped(
            object? sender,
            StoppedEventArgs e)
        {
            OutputRecordingSource? source =
                outputSources.FirstOrDefault(
                    item =>
                        ReferenceEquals(
                            item.Capture,
                            sender
                        )
                );

            if (source == null)
            {
                return;
            }

            source.Writer?.Dispose();
            source.Writer = null;

            if (source.Capture != null)
            {
                source.Capture.DataAvailable -=
                    OutputCapture_DataAvailable;

                source.Capture.RecordingStopped -=
                    OutputCapture_RecordingStopped;

                source.Capture.Dispose();
                source.Capture = null;
            }

            lock (stopLock)
            {
                source.Exception = e.Exception;
                source.Stopped = true;
            }

            TryFinishRecording();
        }

        private void MicrophoneCapture_RecordingStopped(
            object? sender,
            StoppedEventArgs e)
        {
            microphoneWriter?.Dispose();
            microphoneWriter = null;

            if (microphoneCapture != null)
            {
                microphoneCapture.DataAvailable -=
                    MicrophoneCapture_DataAvailable;

                microphoneCapture.RecordingStopped -=
                    MicrophoneCapture_RecordingStopped;

                microphoneCapture.Dispose();
                microphoneCapture = null;
            }

            lock (stopLock)
            {
                microphoneException = e.Exception;
                microphoneStopped = true;
            }

            TryFinishRecording();
        }

        private void ApplicationCapture_RecordingStopped(
            ApplicationRecordingSource source,
            StoppedEventArgs e)
        {
            source.Writer?.Dispose();
            source.Writer = null;

            WasapiRecorder? recorder =
                source.Recorder;

            source.Recorder = null;

            lock (stopLock)
            {
                source.Exception = e.Exception;
                source.Stopped = true;
            }

            if (recorder != null)
            {
                _ = Task.Run(() =>
                {
                    recorder.Dispose();
                });
            }

            TryFinishRecording();
        }

        private void TryFinishRecording()
        {
            lock (stopLock)
            {
                bool outputsStopped =
                    !includeOutput ||
                    outputSources.All(
                        source =>
                            source.Stopped
                    );

                bool applicationsStopped =
                    !includeApplications ||
                    applicationSources.All(
                        source =>
                            source.Stopped
                    );

                if (!outputsStopped ||
                    !applicationsStopped ||
                    !microphoneStopped ||
                    finishStarted)
                {
                    return;
                }

                finishStarted = true;
            }

            try
            {
                Exception? outputException =
                    outputSources
                        .Select(
                            source =>
                                source.Exception
                        )
                        .FirstOrDefault(
                            exception =>
                                exception != null
                        );

                if (outputException != null)
                {
                    throw outputException;
                }

                Exception? applicationException =
                    applicationSources
                        .Select(
                            source =>
                                source.Exception
                        )
                        .FirstOrDefault(
                            exception =>
                                exception != null
                        );

                if (applicationException != null)
                {
                    throw applicationException;
                }

                if (microphoneException != null)
                {
                    throw microphoneException;
                }

                if (finalFilePath == null)
                {
                    throw new Exception(
                        "The final recording path was not created."
                    );
                }

                List<string> sourcePaths =
                    GetRecordedSourcePaths();

                if (sourcePaths.Count == 0)
                {
                    throw new Exception(
                        "No audio was recorded."
                    );
                }

                if (sourcePaths.Count == 1)
                {
                    ProcessSingleRecording(
                        sourcePaths[0],
                        finalFilePath,
                        "audio"
                    );
                }
                else
                {
                    ProcessMixedRecording(
                        sourcePaths
                    );
                }
            }
            catch (Exception ex)
            {
                PreserveFailedRecording();

                RecordingFailed?.Invoke(
                    ex
                );
            }
            finally
            {
                ResetPaths();
            }
        }

        private List<string> GetRecordedSourcePaths()
        {
            var paths =
                new List<string>();

            paths.AddRange(
                outputSources
                    .Select(
                        source =>
                            source.TemporaryPath
                    )
            );

            paths.AddRange(
                applicationSources
                    .Select(
                        source =>
                            source.TemporaryPath
                    )
            );

            if (includeMicrophone &&
                microphoneTemporaryPath != null)
            {
                paths.Add(
                    microphoneTemporaryPath
                );
            }

            return paths;
        }

        private void ProcessMixedRecording(
            IReadOnlyList<string> sourcePaths)
        {
            if (finalFilePath == null)
            {
                throw new Exception(
                    "Could not determine the final recording path."
                );
            }

            foreach (string path in sourcePaths)
            {
                if (!File.Exists(path))
                {
                    throw new Exception(
                        "Could not find one of the recordings."
                    );
                }
            }

            StatusChanged?.Invoke(
                "Mixing recording..."
            );

            AudioMixer.Mix(
                sourcePaths,
                finalFilePath
            );

            foreach (string path in sourcePaths)
            {
                File.Delete(
                    path
                );
            }

            RecordingSaved?.Invoke(
                finalFilePath,
                48000,
                2
            );
        }

        private void ProcessSingleRecording(
            string? rawPath,
            string outputPath,
            string sourceName)
        {
            if (rawPath == null ||
                !File.Exists(rawPath))
            {
                throw new Exception(
                    $"Could not find the {sourceName} recording."
                );
            }

            int sampleRate;
            int channels;
            bool needsConversion;

            using (var reader =
                   new WaveFileReader(
                       rawPath
                   ))
            {
                sampleRate =
                    reader.WaveFormat.SampleRate;

                channels =
                    reader.WaveFormat.Channels;

                needsConversion =
                    AudioConverter.NeedsConversion(
                        reader.WaveFormat
                    );
            }

            if (needsConversion)
            {
                StatusChanged?.Invoke(
                    "Optimizing recording..."
                );

                AudioConverter.ConvertForSoundboard(
                    rawPath,
                    outputPath
                );

                File.Delete(
                    rawPath
                );

                sampleRate =
                    Math.Min(
                        sampleRate,
                        96000
                    );

                channels =
                    Math.Min(
                        channels,
                        2
                    );
            }
            else
            {
                File.Move(
                    rawPath,
                    outputPath,
                    true
                );
            }

            RecordingSaved?.Invoke(
                outputPath,
                sampleRate,
                channels
            );
        }

        private void PreserveFailedRecording()
        {
            try
            {
                string directory =
                    Path.GetDirectoryName(
                        finalFilePath ?? ""
                    ) ?? "";

                string baseName =
                    Path.GetFileNameWithoutExtension(
                        finalFilePath
                    );

                for (int i = 0;
                     i < outputSources.Count;
                     i++)
                {
                    PreserveTemporaryFile(
                        outputSources[i].TemporaryPath,
                        Path.Combine(
                            directory,
                            $"{baseName}_output_{i + 1}_unconverted.wav"
                        )
                    );
                }

                for (int i = 0;
                     i < applicationSources.Count;
                     i++)
                {
                    PreserveTemporaryFile(
                        applicationSources[i].TemporaryPath,
                        Path.Combine(
                            directory,
                            $"{baseName}_application_{i + 1}_unconverted.wav"
                        )
                    );
                }

                if (microphoneTemporaryPath != null)
                {
                    PreserveTemporaryFile(
                        microphoneTemporaryPath,
                        Path.Combine(
                            directory,
                            $"{baseName}_mic_unconverted.wav"
                        )
                    );
                }
            }
            catch
            {
            }
        }

        private static void PreserveTemporaryFile(
            string sourcePath,
            string destinationPath)
        {
            if (!File.Exists(
                    sourcePath))
            {
                return;
            }

            File.Move(
                sourcePath,
                destinationPath,
                true
            );
        }

        private void CleanupAll()
        {
            foreach (var source in outputSources)
            {
                if (source.Capture != null)
                {
                    source.Capture.DataAvailable -=
                        OutputCapture_DataAvailable;

                    source.Capture.RecordingStopped -=
                        OutputCapture_RecordingStopped;

                    source.Capture.Dispose();
                    source.Capture = null;
                }

                source.Writer?.Dispose();
                source.Writer = null;
            }

            foreach (var source in applicationSources)
            {
                source.Recorder?.Dispose();
                source.Recorder = null;

                source.Writer?.Dispose();
                source.Writer = null;
            }

            if (microphoneCapture != null)
            {
                microphoneCapture.DataAvailable -=
                    MicrophoneCapture_DataAvailable;

                microphoneCapture.RecordingStopped -=
                    MicrophoneCapture_RecordingStopped;

                microphoneCapture.Dispose();
                microphoneCapture = null;
            }

            microphoneWriter?.Dispose();
            microphoneWriter = null;

            IsRecording = false;
        }

        private void DeleteTemporaryFiles()
        {
            try
            {
                foreach (var source in outputSources)
                {
                    if (File.Exists(
                            source.TemporaryPath))
                    {
                        File.Delete(
                            source.TemporaryPath
                        );
                    }
                }

                foreach (var source in applicationSources)
                {
                    if (File.Exists(
                            source.TemporaryPath))
                    {
                        File.Delete(
                            source.TemporaryPath
                        );
                    }
                }

                if (microphoneTemporaryPath != null &&
                    File.Exists(
                        microphoneTemporaryPath
                    ))
                {
                    File.Delete(
                        microphoneTemporaryPath
                    );
                }
            }
            catch
            {
            }

            ResetPaths();
        }

        private void ResetPaths()
        {
            outputSources.Clear();
            applicationSources.Clear();

            microphoneTemporaryPath = null;
            finalFilePath = null;

            includeOutput = false;
            includeMicrophone = false;
            includeApplications = false;

            microphoneStopped = false;
            microphoneException = null;

            finishStarted = false;
        }

        public void Dispose()
        {
            CleanupAll();
            DeleteTemporaryFiles();
        }
    }
}