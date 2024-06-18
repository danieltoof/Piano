namespace InEenNotendop.Data;

public class TimeConverter
{
    //Method to convert fulltime int to string
    public string ToMinutesSeconds(int fullTime)
    {
        int minutes = (Convert.ToInt32(fullTime) / 60);
        string minutesString = Convert.ToString(minutes);
        int seconds = (Convert.ToInt32(fullTime) % 60);

        string secondsString = null;

        // adds a 0 before the seconds if under 10 (otherwise it'd show 1:9 when it should be 1:09)
        if (seconds < 10)
        {
            secondsString = "0" + seconds;
        }
        else
        {
            secondsString = seconds.ToString();
        }

        // Put minutes and seconds together
        return minutesString + ":" + secondsString;
    }
}