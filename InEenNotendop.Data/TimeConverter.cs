namespace InEenNotendop.Data;

public class TimeConverter
{
    public string ToMinutesSeconds(int fullTime)
    {
        int minutes = (Convert.ToInt32(fullTime) / 60);
        string minutesString = Convert.ToString(minutes);
        int seconds = (Convert.ToInt32(fullTime) % 60);

        string secondsString = null;

        // 0 voor de secondes plakken als ze onder 10 zijn
        if (seconds < 10)
        {
            secondsString = "0" + seconds;
        }
        else
        {
            secondsString = seconds.ToString();
        }

        // minuten en secondes aan elkaar plakken
        return minutesString + ":" + secondsString;
        //return "aaa";
    }
}