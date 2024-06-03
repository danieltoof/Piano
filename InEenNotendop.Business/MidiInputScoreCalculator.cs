using System.Collections;


namespace InEenNotendop.Business;

public class MidiInputScoreCalculator
{
    public MidiInputProcessor MidiInputProcessor { get; set; }

    private const decimal maxScore = 1000;

    // Lists omzetten naar dictionary, voor snellere verwerking en bijhouden of note al verwerkt is of niet.
    //public Dictionary<Note, bool> NotesPlayed = new();
    //public Dictionary<Note, bool> NotesShouldHavePlayed = new();
    public List<int> NotesInSong = new();
    public List<Note> HashNotesSong = [];
    public List<Note> HashNotesPlayed = [];
    private int amountOfNotesInSong;
    private decimal maxScorePerNote;
    public decimal Score { get; set; }

    public MidiInputScoreCalculator(MidiInputProcessor midiInputProcessor)
    {
        MidiInputProcessor = midiInputProcessor;
        amountOfNotesInSong = MidiInputProcessor.ListOfNotesSong.Count;
        if (amountOfNotesInSong == 0)
        {
            amountOfNotesInSong = 1;
        }

        maxScorePerNote = maxScore
                                  / amountOfNotesInSong;

        // Elke unieke noot uit het liedje wordt opgeslagen.
        foreach (var note in MidiInputProcessor.ListOfNotesSong) 
        {
            if (!NotesInSong.Contains(note.NoteNumber))
            {
                NotesInSong.Add(note.NoteNumber);
            }
        }

    }

    public int CalculateScoreAfterSongCompleted() // SCORE BEREKENEN ALS NUMMER HELEMAAL VOLTOOID IS
    {

        // Lists vanuit MidiInputProcessor overzettan naar lokale variabelen
        // Dat maakt dit document beter te lezen
        MidiInputProcessor.ListOfNotesSong.ForEach(note => HashNotesSong?.Add(note));
        MidiInputProcessor.ListOfNotesPlayed.ForEach(note => HashNotesPlayed?.Add(note));


        #region DebugRegion

        //// DEBUG -----------------------------
        //Debug.WriteLine("Note numbers in HashNotesSong");
        //foreach (var VARIABLE in HashNotesSong)
        //{
        //    Debug.WriteLine(VARIABLE.NoteNumber);
        //}

        //Debug.WriteLine("Note numbers in HashNotesPlayed");
        //foreach (var VARIABLE in HashNotesPlayed)
        //{
        //    Debug.WriteLine(VARIABLE.NoteNumber);
        //}
        //// DEBUG -----------------------------

        #endregion


        // Per noot die in het nummer voorkomt gaan we de score berekenen van alle instances
        foreach (var noteNumber in NotesInSong)
        {
            // We vergelijken per noot hoe ver de gespeelde noten er van af zitten
            // Dus, we creeëren eerst 2 gefiltreerde lijsten van de noten uit het liedje en de gespeelde noten
            var hashNotesInSongFilteredByNote =
                from note in HashNotesSong where note.NoteNumber == noteNumber select note ;
            var hashNotesPlayedFilteredByNote =
                from note in HashNotesPlayed where note.NoteNumber == noteNumber select note;


            // Per noot gaan we nu kijken hoe ver hij van een potentiële noot die geraakt zou moeten worden zit
            // Hij itereert elke keer door elke instance van de specifieke noot, wat wellicht inefficiënt lijkt
            // Er is een efficiëntere manier om de score te berekenen alleen vanwege de gelimiteerde hoeveelheid 
            // tijd hebben we besloten dat dit een 'Nice to have is'. Een optie zou zijn om gelijk bij het spelen
            // van een noot gelijk te checken of de noot in de buurt zit. Dit wordt voor het moment te complex 
            // maar zou in de toekomst wellicht nog toegevoegd kunnen worden.
            foreach (var SongNote in hashNotesInSongFilteredByNote)
            {
                foreach (var PlayedNote in hashNotesPlayedFilteredByNote)
                {
                    if (!SongNote.ScoreIsCalculated)
                    {
                        decimal noteScore = maxScorePerNote *
                                            getNoteScoreFactor(SongNote.NoteStartTime, PlayedNote.NoteStartTime);
                        Score += noteScore;
                        SongNote.ScoreIsCalculated = true;
                    }
                }
            }
            Score = Math.Round(Score);
        }
        return (int)Score;
    }

    public decimal getNoteScoreFactor(TimeSpan note1TimeSpan, TimeSpan note2TimeSpan)
    {
        // Berekenen hoe ver de 2 noten van elkaar af zitten
        int deltaTimeMilliseconds = note1TimeSpan.Milliseconds - note2TimeSpan.Milliseconds;
        if (deltaTimeMilliseconds < 0) // Als negatieve waarde is omzetten naar positieve waarde
        {
            deltaTimeMilliseconds *= -1;
        }

        // Dit zijn de marges
        switch (deltaTimeMilliseconds)
        {
            case <= 20:
                return 1.00m;
            case <= 50:
                return 0.95m;
            case <= 80:
                return 0.80m;
            case <= 100:
                return 0.60m;
            case <= 130:
                return 0.30m;
            default :
                return 0;
        }
    }

}