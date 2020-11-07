using Calista.FireplaySupport;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace MixMaster
{
    public class MixElement : PlayItem, ISampleProvider
    {
        public MixElement(PlayItem item) : base()
        {
            //stream = new AudioFileReader(item.)
        }

        private WaveStream stream { get; set; } 

        public WaveFormat WaveFormat => throw new NotImplementedException();

        public int Read(float[] buffer, int offset, int count)
        {
            
        }
    }
}
