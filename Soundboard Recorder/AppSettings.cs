namespace Soundboard_Recorder
{
    public class AppSettings
    {
        public string? AudioDeviceId { get; set; }

        public int RecordingMode { get; set; } = 0;

        public int RecordingHotkey { get; set; } = (int)Keys.F8;

        public string? SaveFolder { get; set; }
    }
}