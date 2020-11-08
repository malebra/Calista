using Calista.FireplaySupport;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml.Schema;

namespace MixMaster
{
    public class MixElement : PlayItem, ISampleProvider
    {
        private int readSamples = 0;
        private Stream stream = null;
        WaveFormat waveFormat = null;

        private MixElement()
        {

        }

        public MixElement(PlayItem item, WaveFormat format) : base(item)
        {
            waveFormat = format;

        }

        public WaveFormat WaveFormat => WaveFormat;

        public WaveStream GetWaveStream()
        {
            WaveStream temp = null;
            using (Stream afd = new AudioFileReader(System.IO.Path.GetFullPath(_PathName)))
            {
                
            }

            if (!WaveFormat.Equals(temp.WaveFormat) || temp.WaveFormat.BitsPerSample != 16)
            {
                temp = new WaveFormatConversionStream(new WaveFormat(48000, 16, 2), temp) as WaveStream;

            }

            return temp;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (stream == null)
            {
                stream = GetWaveStream();
            }
            return ((WaveStream)stream).Read(buffer, offset, count);
        }
    }
}
