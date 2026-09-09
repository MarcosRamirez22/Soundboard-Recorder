# Soundboard Recorder
A lightweight Windows audio recorder that captures system output using a customizable global hotkey.

Soundboard Recorder is designed for quickly saving audio clips while gaming, talking in Discord, watching videos, or using any other application without needing to switch windows.

# Features:
Record audio from a selected Windows output device
Global hotkey works while other applications are focused
Customizable recording hotkey
Two recording modes: Hold to Record and Press to Toggle
Choose where recordings are saved
Open the recordings folder directly from the application
Automatic timestamped WAV filenames
Persistent settings between launches
Recording status shown directly in the application
Runs without interrupting the active application when a recording is completed

# How It Works

Soundboard Recorder uses Windows WASAPI loopback capture to record audio that is being played through a selected output device.

For example:

Discord / Game / Browser
          ↓
Windows Audio Output
          ↓
Soundboard Recorder
          ↓
       WAV File

The application records the combined audio being sent through the selected playback device.

# Recording Modes
Hold to Record

Hold the configured hotkey to begin recording.

Release the hotkey to stop and save the recording.

Hotkey Down → Start Recording
Hotkey Up   → Stop and Save

Press to Toggle

Press the configured hotkey once to begin recording.

Press it again to stop and save.

First Press  → Start Recording
Second Press → Stop and Save
Settings

The application remembers your preferences between launches, including:

Selected audio output device
Recording mode
Recording hotkey
Recording save folder

Settings are stored locally in the user's Windows AppData directory.

Recording Files

Recordings are currently saved in WAV format.

Files are automatically named using the date and time:

Recording_2026-09-08_18-30-45.wav

The recording location can be changed from within the application.

# Download

Download the latest Windows build from the [Releases](../../releases) page.

# Requirements
Windows
A working Windows audio output device

The application is built with:

C#
.NET
Windows Forms
NAudio
Building From Source
Clone the repository:
git clone https://github.com/MarcosRamirez22/Soundboard-Recorder.git
Open the solution in Visual Studio.
Restore NuGet packages if Visual Studio does not do so automatically.
Build and run the project.

The project uses the NAudio package for Windows audio capture.

# Current Limitations

The current version records a single Windows audio output device at a time.

It does not yet support:

Recording multiple output devices simultaneously
Recording individual applications separately
Microphone recording
MP3 output
Replay-buffer recording
System tray operation

These may be added in future versions.

# Planned Features

Possible future improvements include:

Multiple audio source selection
Per-application audio capture
Optional microphone recording
MP3 support
System tray/background operation
Replay buffer for saving the previous 1–60 seconds
Soundboard integration
Recording start/stop sound feedback

This project is licensed under the MIT License.

See the LICENSE file for details.
