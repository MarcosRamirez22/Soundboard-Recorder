# Soundbyte Builder

Soundbyte Builder is a Windows audio recording tool designed to make it easy to capture audio from multiple sources and combine them into a single recording.

The project originally started as a simple soundboard recording utility, but it has expanded into a more flexible multi-source audio recorder.

## Features

- Record from one or more audio output devices
- Record from a microphone
- Record audio from specific running applications
- Record multiple applications at the same time
- Mix multiple recording sources into a single WAV file
- Remember selected applications between launches
- Show saved applications as offline when they are not currently running
- Remember selected audio devices and microphone settings
- Support global recording hotkeys
- Support both:
  - Hold to Record
  - Press to Toggle
- Custom recording save folder
- Quickly open the recordings folder
- Automatically convert incompatible multichannel recordings
- Multichannel volume compensation for sources with more than two channels

## Recording Sources

Soundbyte Builder currently supports three main recording source types.

### Audio Outputs

Record everything being played through selected Windows audio output devices.

Multiple output devices can be selected at the same time.

### Applications

Record audio directly from selected running applications.

Applications are shown using friendly names when possible.

Previously selected applications are remembered between launches. If a saved application is not currently running, it will appear as:

`Application Name (Offline)`

Offline applications are skipped when recording until they are running again.

### Microphone

A microphone can be recorded by itself or combined with audio outputs and applications.

## Source Combinations

Soundbyte Builder supports combinations such as:

- Audio output only
- Multiple audio outputs
- Microphone only
- Application only
- Multiple applications
- Audio output + microphone
- Application + microphone
- Audio output + application
- Multiple outputs + multiple applications + microphone

## Important Note About Duplicate Audio

If an application is selected directly and that same application's audio is also being captured through a selected audio output, the application may be recorded twice.

For example:

- Audio Output: Headphones
- Application: Discord

If Discord is playing through the selected headphones, Discord may exist in both recording sources and sound louder in the final mix.

## Audio Format

When multiple recording sources are mixed, Soundbyte Builder currently uses:

- 48 kHz sample rate
- Stereo
- 32-bit floating point audio

Single-source recordings may retain their original sample rate when possible.

Sources with more than two channels are converted to stereo before final output.

## System Requirements

- Windows 10 version 2004 or newer
- 64-bit Windows recommended

Application-specific audio recording relies on Windows process loopback capture and requires Windows 10 build 19041 or newer.

## Download

Download the latest Windows release from the GitHub Releases page.

`Soundbyte Builder.exe`

## Building From Source

### Requirements

- Visual Studio
- .NET desktop development workload
- Windows Forms support
- NAudio

### Steps

1. Clone the repository:

```bash
git clone https://github.com/YOUR_USERNAME/Soundbyte-Builder.git
```

2. Open the solution in Visual Studio.

3. Restore NuGet packages if needed.

4. Build the solution.

5. Run the project.

## Project Structure

```text
Soundbyte Builder
├── Audio
│   ├── AudioConverter.cs
│   ├── AudioMixer.cs
│   └── RecordingManager.cs
├── Input
│   └── GlobalKeyboardHook.cs
├── Models
│   ├── AppSettings.cs
│   ├── RunningApplication.cs
│   └── SavedApplication.cs
├── Programs
│   └── ApplicationDiscovery.cs
├── UI
│   ├── MainForm.cs
│   ├── MainForm.Designer.cs
│   └── MainForm.resx
└── Program.cs
```

## Planned Features

Possible future improvements include:

- Per-source volume controls
- Master volume control
- Audio limiting to help prevent clipping
- Audio trimming and editing
- More advanced microphone controls
- Improved multi-source mixing controls
- Additional application recording options
- Custom application icon and visual improvements

## Current Status

Soundbyte Builder is still in active development.

Some features and behavior may change between releases.

## License

This project is licensed under the MIT License.

See the LICENSE file for details.
