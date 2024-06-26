﻿using NAudio.Midi;

namespace InEenNotendop.Business
{

    public class Note
    {
        public int NoteNumber { get; set; }
        public TimeSpan NoteStartTime { get; set; }
        public TimeSpan NoteDuration { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsBlockGenerated { get; set; } = false;
        public bool ScoreIsCalculated = false;

        public Note(NoteOnEvent noteOnEvent, TimeSpan startTime) // for midi input
        {
            NoteNumber = noteOnEvent.NoteNumber;
            NoteStartTime = startTime;
            IsPlaying = true;

        }

        public Note(int noteNumber, TimeSpan noteStartTime) // for making note without midi event
        {
            NoteNumber = noteNumber;
            NoteStartTime = noteStartTime;
        }

        public Note(int noteNumber, TimeSpan noteStartTime, TimeSpan noteDuration)
        {
            NoteNumber = noteNumber;
            NoteStartTime = noteStartTime;
            NoteDuration = noteDuration;
        }

        public void EndNote(TimeSpan endTime)
        {
            NoteDuration = endTime - NoteStartTime;
            IsPlaying = false;
        }
    }
}
