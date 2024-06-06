namespace PianoHero.Data
{
    // Custom class for songs
    public class Nummer 
    {
        public string Title { get; set; }
        public string Artiest { get; set; }
        public int FullTime { get; set; }
        public int Bpm { get; set; }
        public int Moeilijkheid { get; set; }
        public int Id { get; set; }
        public string Filepath { get; set; }
        public int Score { get; set; }
        public string ConvertedTime { get; set; }
        public string ConvertedMoeilijkheid { get; set; }

        public Nummer(string title, string artiest, int fulltime, int bpm, int moeilijkheid, int id, string filepath, int score, string convertedTime, string convertedMoeilijkheid)
        {
            Title = title;
            Artiest = artiest;
            FullTime = fulltime;
            Bpm = bpm;
            Moeilijkheid = moeilijkheid;
            Id = id;
            Filepath = filepath;
            Score = score;
            ConvertedTime = convertedTime;
            ConvertedMoeilijkheid = convertedMoeilijkheid;
        }
    }
}