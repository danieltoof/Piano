using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InEenNotendop.Business
{
    // Intro song zorgt ervoor dat de midi playback warmdraaid. Denk aan een buizen gitaarversterker die even aan moet staan om het mooiste geluid te krijgen.
    public class IntroSong : Song
    {
        public IntroSong() : base(new MidiFile(
                @"IntroMidi.mid"))
        {
        }
    }
}
