namespace SoundByte_Builder.Models
{
    public class RunningApplication
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string WindowTitle { get; set; } = "";
        public string? ExecutablePath { get; set; }
    }
}