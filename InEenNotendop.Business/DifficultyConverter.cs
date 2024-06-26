namespace InEenNotendop.Data
{
    public class DifficultyConverter
    {
        public string Convert(int difficulty)
        {
            // Directly convert the moeilijkheid value to readable text
            switch (difficulty)
            {
                case 1:
                    return "Easy";
                case 2:
                    return "Medium";
                case 3:
                    return "Hard";
                default:
                    return "Unknown";
            }
        }
    }
}