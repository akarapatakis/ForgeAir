using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using NAudio.Wave;
namespace ForgeAir.Core.Services.TrafficAnnouncement
{
    public static class TextToSpeechService
    {
        public static Stream ConvertToSpeech(string text)
        {
            var stream = new MemoryStream();
            using (var synth = new SpeechSynthesizer())
            {
                synth.SetOutputToWaveStream(stream);
                synth.Speak(text);
            }
            stream.Position = 0;
            return stream;
        }
    }
}
