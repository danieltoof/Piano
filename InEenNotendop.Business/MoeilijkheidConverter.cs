namespace InEenNotendop.Data
{
    public class MoeilijkheidConverter
    {
        public string Convert(int moeilijkheid)
        {
            // Directly convert the moeilijkheid value to readable text
            switch (moeilijkheid)
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