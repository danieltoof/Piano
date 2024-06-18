using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InEenNotendop.Business
{
    public class MidiProcessor
    {
        private object Owner;
        private MidiPlayer midiPlayer;
        private MidiIn midiIn;
        public Stopwatch Stopwatch { get; set; } // Acurater dan DateTime.Now

        private Song songPlayed;


        public MidiProcessor(object Owner)
        {
            this.Owner = Owner;
            InitializeMidi("Microsoft GS Wavetable Synth");
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
            songPlayed = new Song();
        }

        
        private void MidiInMessageReceived(object? sender, MidiInMessageEventArgs e)
        {
            if (e.MidiEvent is NoteOnEvent noteOnEvent)
            {
                midiPlayer.PlayNote(noteOnEvent.NoteNumber);
                songPlayed.AddNote(new Note(noteOnEvent, Stopwatch.Elapsed));
                SendChangeSignalToUI(noteOnEvent.NoteNumber, true);
            }
            else if (e.MidiEvent is NoteEvent noteEvent) // een noteevent wat geen noteonevent is is in dit geval altijd een event die een noot eindigt.
            {
                midiPlayer.StopNote(noteEvent.NoteNumber);
                SendChangeSignalToUI(noteEvent.NoteNumber, false);
            }
        }

        public UIChangeMessage SendChangeSignalToUI(int NoteNumber, bool isNoteOnMessage)
        {
            if (isNoteOnMessage)
            {
                return new UIChangeMessage(NoteNumber, true);
            } else
            {
                return new UIChangeMessage(NoteNumber, false);
            }
        }

        private void InitializeMidi(string desiredOutDevice)
        {
            midiPlayer = new MidiPlayer(desiredOutDevice, this);

            var numDevices = MidiIn.NumberOfDevices;
            var desiredDeviceIndex = 0; // DEZE KAN VERANDEREN SOMS SPONTAAN
            if (desiredDeviceIndex < numDevices)
            {
                midiIn = new MidiIn(desiredDeviceIndex);
                midiIn.MessageReceived += MidiInMessageReceived;
                midiIn.Start();
            }
            else
            {
                MessageBox.Show("Invalid MIDI device index or no devices found.");
            }
        }
    }
}
