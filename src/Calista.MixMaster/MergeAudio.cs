using System;
using System.Collections.Generic;
using System.Text;
using NAudio;
using NAudio.Wave;
using System.Linq;
using System.IO;
using NAudio.Midi;
using NAudio.Wave.SampleProviders;
using Calista.FireplaySupport;
using System.Xml.Serialization;

namespace Calista.MixMaster
{
    public static class MergeAudio
    {
        public static void Merge(string[] paths)
        {

            IEnumerable<ISampleProvider> files = paths.Select(p =>
            {
                if (!File.Exists(p)) throw new FileNotFoundException(message: $"The file {p} was not found.");

                WaveStream temp = null;

                if (p.EndsWith(".mp3"))
                {
                    temp = new Mp3FileReader(p);
                }
                else if (p.EndsWith(".wav"))
                {
                    temp = new WaveFileReader(p);
                }
                else
                {
                    throw new FormatException(message: $"The file {p} is of an unsupported format.");
                    return null;
                }

                if (temp.WaveFormat.SampleRate != 48000 || temp.WaveFormat.Channels != 2 || temp.WaveFormat.BitsPerSample != 16)
                {
                    return new WaveFormatConversionProvider(new WaveFormat(48000, 16, 2), temp).ToSampleProvider();
                }

                return temp.ToSampleProvider();
            });

            var fufu = new MixingSampleProvider(new WaveFormat(48000, 16, 2));
            
            
        }


        public static void Merge(PlayList list)
        {

            IEnumerable<ISampleProvider> files = list.Select(item =>
            {
                if (!File.Exists(item.PathName)) throw new FileNotFoundException(message: $"The file {item} was not found.");

                WaveStream temp = null;

                if (item.PathName.EndsWith(".mp3"))
                {
                    temp = new Mp3FileReader(item.PathName);

                }
                else if (item.PathName.EndsWith(".wav"))
                {
                    temp = new WaveFileReader(item.PathName);
                }
                else
                {
                    throw new FormatException(message: $"The file {item} is of an unsupported format.");
                }

                if (temp.WaveFormat.SampleRate != 48000 || temp.WaveFormat.Channels != 2 || temp.WaveFormat.BitsPerSample != 16)
                {
                    return new WaveFormatConversionProvider(new WaveFormat(48000, 16, 2), temp).ToSampleProvider();
                }

                var fin = new FadeInOutSampleProvider(temp.ToSampleProvider());

                fin.Take(item.Duration);
                fin.Skip(item.Start);
                fin.BeginFadeIn(double.Parse(item.SoftIn));
                fin.BeginFadeOut(double.Parse(item.SoftOut));

                return fin;
            });

            

            var fufu = new MixingSampleProvider(new WaveFormat(48000, 16, 2));




        }


        
    }
}
