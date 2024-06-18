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
        private Stopwatch stopwatch; // Acurater dan DateTime.Now


        public MidiProcessor(object Owner)
        {
            this.Owner = Owner;
            InitializeMidi("Microsoft GS Wavetable Synth");
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }


        private void MidiInMessageReceived(object? sender, MidiInMessageEventArgs e)
        {
            if (e.MidiEvent is NoteOnEvent noteOnEvent)
            {
                midiNoteToButton[noteOnEvent.NoteNumber].Button.Dispatcher.Invoke(() =>
                {
                    // kleur toets veranderen
                    midiNoteToButton[noteOnEvent.NoteNumber].Button.Background = noteHitBrush;
                    // noot beginnnen met spelen
                    midiPlayer.PlayNote(noteOnEvent.NoteNumber);

                    // Tijd berekenen sinds start spelen
                    TimeSpan startTimeNotePlayed = stopwatch.Elapsed;
                    // Noot toevoegen aan list in midiInputProcessor's list voor score berekening
                    midiInputProcessor.ListOfNotesPlayed.Add(new Note(noteOnEvent, startTimeNotePlayed));
                });

            }
            else if (e.MidiEvent is NoteEvent noteEvent) // een noteevent wat geen noteonevent is is in dit geval altijd een event die een noot eindigt.
            {
                if (midiNoteToButton.ContainsKey(noteEvent.NoteNumber))
                {
                    //noot stoppen met spelen
                    midiPlayer.StopNote(noteEvent.NoteNumber);
                    // toets terugveranderen naar originele kleur
                    midiNoteToButton[noteEvent.NoteNumber].Button.Dispatcher.Invoke(() =>
                    {
                        midiNoteToButton[noteEvent.NoteNumber].Button.Background = midiNoteToButton[noteEvent.NoteNumber].ButtonColor;
                    });
                }
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
                midiIn.MessageReceived += MidiIn_MessageReceived;
                midiIn.Start();
            }
            else
            {
                MessageBox.Show("Invalid MIDI device index or no devices found.");
            }
        }
    }
}
