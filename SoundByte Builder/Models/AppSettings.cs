namespace SoundByte_Builder.Models
{
    public class AppSettings
    {
        public List<string> AudioDeviceIds { get; set; } = new();

        public List<SavedApplication> Applications { get; set; } = new();

        public int RecordingMode { get; set; } = 0;

        public int RecordingHotkey { get; set; } = (int)Keys.F8;

        public string? SaveFolder { get; set; }

        public string? MicrophoneDeviceId { get; set; }

        public bool IncludeMicrophone { get; set; }

        public bool IncludeAudioOutput { get; set; } = true;

        public bool IncludeApplications { get; set; }
    }
}