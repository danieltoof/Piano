﻿namespace InEenNotendop.Business;

public static class ScoreCalculator
{
    private const float MaxScore = 1000;
    static float _maxScorePerNote = 0;

    public static int CalculateScore(NoteCollection song, NoteCollection songPlayed)
    {
        float score = 0;


        if (song == null) {
            song = new NoteCollection();
        }

        if (songPlayed != null && song.Notes.Count > 0)
        {
            _maxScorePerNote = MaxScore / song.Notes.Count;
        } else
        {
            _maxScorePerNote = 0;
        }


        // We're gonna calculate the score of all instances for every note that occurs in the song 
        //

        try
        {
            foreach (var SongNote in song.Notes)
            {
                foreach (var PlayedNote in songPlayed.Notes)
                {
                    if (!SongNote.ScoreIsCalculated && SongNote.NoteNumber == PlayedNote.NoteNumber)
                    {
                        float noteScore = _maxScorePerNote *
                                            GetNoteScoreFactor(SongNote.NoteStartTime, PlayedNote.NoteStartTime);
                        score += noteScore;
                        SongNote.ScoreIsCalculated = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
        }


        
        return (int)score;
    }

    public static float GetNoteScoreFactor(TimeSpan note1TimeSpan, TimeSpan note2TimeSpan)
    {
        // Calculate how far apart the 2 notes are
        int deltaTimeMilliseconds = note1TimeSpan.Milliseconds - note2TimeSpan.Milliseconds;
        // If negative value convert to positive value
        if (deltaTimeMilliseconds < 0) 
        {
            deltaTimeMilliseconds *= -1;
        }

        // These are the margins
        switch (deltaTimeMilliseconds)
        {
            case <= 20:
                return 1f;
            case <= 50:
                return 0.95f;
            case <= 80:
                return 0.8f;
            case <= 100:
                return 0.6f;
            case <= 130:
                return 0.3f;
            default :
                return 0;
        }
    }
}