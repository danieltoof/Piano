// See https://aka.ms/new-console-template for more information

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;

static void GetLength(string MidiFileName)
{
    // check of bestandsnaam opgegeven is
    if (!MidiFileName.EndsWith(".mid"))
    {
        //voeg bestandsnaam toe aan string
        MidiFileName = MidiFileName + ".mid";
    }

    try
    {
        // inladen midi
        var midi = MidiFile.Read(@"..\..\..\midis\" + MidiFileName);

        // lees lengte van midi
        TimeSpan MidiFileDuration = midi.GetDuration<MetricTimeSpan>();

        // lengte timespan omzetten naar minuten en secondes
        int minutes = (Convert.ToInt32(MidiFileDuration.TotalSeconds) / 60);
        int seconds = (Convert.ToInt32(MidiFileDuration.TotalSeconds) % 60);

        string secondsString;

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
        String midiLengte = minutes.ToString() + ":" + secondsString;

        // print final waarde
        Console.WriteLine(midiLengte);

    } 
    catch (FileNotFoundException)
    {
        Console.WriteLine("ERROR: MIDI file niet gevonden.");
    }
    
}

string a = Console.ReadLine();
GetLength(a);

var midi = MidiFile.Read(@"..\..\..\midis\Darude_-_Sandstorm__Izzet_Selanik__Arne_Mulder_20061031124814.mid");
//var midi = MidiFile.Read(@"..\..\..\midis\AUD_HTX0525.mid");

using (var output = OutputDevice.GetByName("Microsoft GS Wavetable Synth"))
using (var playback = midi.GetPlayback(output))
{
    playback.Speed = 1.0;
    //playback.Play();
    midi.Play(output);
    Console.WriteLine("playing midi");
    System.Threading.Thread.Sleep(2000);
}