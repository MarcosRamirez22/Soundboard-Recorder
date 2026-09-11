using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SoundByte_Builder.Audio
{
    public static class AudioConverter
    {
        private const int MaxSampleRate = 96000;
        private const int MaxChannels = 2;

        // Temporary compensation for volume loss
        // when multichannel audio is downmixed.
        private const float DownmixVolume = 3.0f;

        public static bool NeedsConversion(
            WaveFormat format)
        {
            return format.SampleRate > MaxSampleRate ||
                   format.Channels > MaxChannels;
        }

        public static void ConvertForSoundboard(
            string inputPath,
            string outputPath)
        {
            using var reader =
                new AudioFileReader(inputPath);

            int sourceSampleRate =
                reader.WaveFormat.SampleRate;

            int sourceChannels =
                reader.WaveFormat.Channels;

            int targetSampleRate =
                Math.Min(
                    sourceSampleRate,
                    MaxSampleRate
                );

            int targetChannels =
                Math.Min(
                    sourceChannels,
                    MaxChannels
                );

            ISampleProvider sampleProvider =
                reader;

            // Only apply the volume compensation
            // when we're actually reducing channels.
            if (sourceChannels > MaxChannels)
            {
                sampleProvider =
                    new VolumeSampleProvider(
                        sampleProvider
                    )
                    {
                        Volume = DownmixVolume
                    };
            }

            WaveFormat outputFormat =
                WaveFormat.CreateIeeeFloatWaveFormat(
                    targetSampleRate,
                    targetChannels
                );

            using var resampler =
                new MediaFoundationResampler(
                    sampleProvider.ToWaveProvider(),
                    outputFormat
                );

            resampler.ResamplerQuality = 60;

            WaveFileWriter.CreateWaveFile(
                outputPath,
                resampler
            );
        }
    }
}