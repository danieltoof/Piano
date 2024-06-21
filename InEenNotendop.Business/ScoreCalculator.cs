namespace InEenNotendop.Business;

public static class ScoreCalculator
{
    private const float maxScore = 1000;
    static float maxScorePerNote = 0;

    public static int CalculateScore(NoteCollection song, NoteCollection songPlayed)
    {
        float score = 0;


        if (song == null) {
            song = new NoteCollection();
        }
        if (song.Notes.Count == 0)
        {
            song.Notes.Add(new Note(1, TimeSpan.FromDays(1)));
        }

        if (songPlayed != null)
        {
            maxScorePerNote = maxScore / song.Notes.Count;
        } else
        {
            maxScorePerNote = 0;
        }


        // Per noot die in het nummer voorkomt gaan we de score berekenen van alle instance
        // 

        try
        {
            foreach (var SongNote in song.Notes)
            {
                song.Notes.Add(new Note(1, TimeSpan.FromDays(1)));

                foreach (var PlayedNote in songPlayed.Notes)
                {
                    if (!SongNote.ScoreIsCalculated && SongNote.NoteNumber == PlayedNote.NoteNumber)
                    {
                        float noteScore = maxScorePerNote *
                                            getNoteScoreFactor(SongNote.NoteStartTime, PlayedNote.NoteStartTime);
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

    public static float getNoteScoreFactor(TimeSpan note1TimeSpan, TimeSpan note2TimeSpan)
    {
        // Berekenen hoe ver de 2 noten van elkaar af zitten
        int deltaTimeMilliseconds = note1TimeSpan.Milliseconds - note2TimeSpan.Milliseconds;
        // Als negatieve waarde is omzetten naar positieve waarde
        if (deltaTimeMilliseconds < 0) 
        {
            deltaTimeMilliseconds *= -1;
        }

        // Dit zijn de marges
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