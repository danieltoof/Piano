namespace InEenNotendop.Business
{
    // Custom class for songs
    public class Song 
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public int FullTime { get; set; }
        public int Bpm { get; set; }
        public int Difficulty { get; set; }
        public int Id { get; set; }
        public string Filepath { get; set; }
        public int Score { get; set; }
        public string ConvertedTime { get; set; }
        public string ConvertedDifficulty { get; set; }
        public string Name { get; set; }

        public Song(string title, string artist, int fulltime, int bpm, int difficulty, int id, string filepath, int score, string convertedTime, string convertedDifficulty, string name)
        {
            Title = title;
            Artist = artist;
            FullTime = fulltime;
            Bpm = bpm;
            Difficulty = difficulty;
            Id = id;
            Filepath = filepath;
            Score = score;
            ConvertedTime = convertedTime;
            ConvertedDifficulty = convertedDifficulty;
            Name = name;
        }
    }
}