using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SoundByte_Builder.Audio
{
    public static class AudioMixer
    {
        private const int MixSampleRate = 48000;
        private const int MixChannels = 2;

        public static void Mix(
            IEnumerable<string> sourcePaths,
            string finalPath)
        {
            var readers =
                new List<AudioFileReader>();

            var resamplers =
                new List<MediaFoundationResampler>();

            try
            {
                var mixer =
                    new MixingSampleProvider(
                        WaveFormat.CreateIeeeFloatWaveFormat(
                            MixSampleRate,
                            MixChannels
                        )
                    )
                    {
                        ReadFully = false
                    };

                foreach (string path in sourcePaths)
                {
                    var reader =
                        new AudioFileReader(
                            path
                        );

                    readers.Add(
                        reader
                    );

                    ISampleProvider source =
                        PrepareSource(
                            reader,
                            resamplers
                        );

                    mixer.AddMixerInput(
                        source
                    );
                }

                WaveFileWriter.CreateWaveFile(
                    finalPath,
                    mixer.ToWaveProvider()
                );
            }
            finally
            {
                foreach (var resampler in resamplers)
                {
                    resampler.Dispose();
                }

                foreach (var reader in readers)
                {
                    reader.Dispose();
                }
            }
        }

        private const float MultichannelGain = 3.0f;

        private static ISampleProvider PrepareSource(
            AudioFileReader reader,
            List<MediaFoundationResampler> resamplers)
        {
            ISampleProvider source = reader;

            if (reader.WaveFormat.Channels > 2)
            {
                source =
                    new VolumeSampleProvider(source)
                    {
                        Volume = MultichannelGain
                    };
            }

            var targetFormat =
                WaveFormat.CreateIeeeFloatWaveFormat(
                    MixSampleRate,
                    MixChannels
                );

            var resampler =
                new MediaFoundationResampler(
                    source.ToWaveProvider(),
                    targetFormat
                )
                {
                    ResamplerQuality = 60
                };

            resamplers.Add(resampler);

            return resampler.ToSampleProvider();
        }
    }
}