// See https://aka.ms/new-console-template for more information

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;

static String GetLength(string MidiFileName)
{
    // check of bestandsnaam opgegeven is
    if (!MidiFileName.EndsWith(".mid"))
    {
        //voeg bestandsnaam toe aan string
        MidiFileName = MidiFileName + ".mid";
    }

    // inladen midi
    var midi = MidiFile.Read(MidiFileName);

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
    //Console.WriteLine(midiLengte);

    return midiLengte;
}

static int GetStartTempo(string MidiFileName)
{
    // inladen midi
    var midi = MidiFile.Read(MidiFileName);

    // haal tempo van midi op
    using (var tempoMapManager = new TempoMapManager(
        midi.TimeDivision,
        midi.GetTrackChunks().Select(c => c.Events)))
    {
        // maken tempomap
        TempoMap tempoMap = tempoMapManager.TempoMap;

        // lezen start tempo
        int tempoStart = (int)Math.Round(tempoMap.GetTempoAtTime((MidiTimeSpan)1).BeatsPerMinute);

        return tempoStart;
    }
}
static int GetHighestTempo(string MidiFileName)
{
    // inladen midi
    var midi = MidiFile.Read(MidiFileName);

    // haal tempo van midi op
    using (var tempoMapManager = new TempoMapManager(
        midi.TimeDivision,
        midi.GetTrackChunks().Select(c => c.Events)))
    {
        // maken tempomap
        TempoMap tempoMap = tempoMapManager.TempoMap;

        // checken hoogste tempo in midi
        var tempoList = tempoMap.GetTempoChanges().Select(valueChange => valueChange.Value).ToList();
        var orderedBPMList = tempoList.Select(tempo => tempo.BeatsPerMinute).Order();
        // check voor als er geen tempo changes zijn - midi default dan naar 120 BPM
        if (orderedBPMList.Count() == 0)
        {
            //Console.WriteLine("120 BPM");
            int returnTempo = 120;
            return returnTempo;
        }
        else
        {
            //Console.WriteLine(orderedBPMList.Last() + " BPM");
            return (int)Math.Round(orderedBPMList.Last());
        }
    }

}

//string a = Console.ReadLine();
//GetLength(a);

//var midi = MidiFile.Read(@"..\..\..\midis\Darude_-_Sandstorm__Izzet_Selanik__Arne_Mulder_20061031124814.mid");
//var midi = MidiFile.Read(@"..\..\..\midis\AUD_HTX0525.mid");

while (true)
{
    var rand = new Random();
    var files = Directory.GetFiles(@"..\..\..\midis\");
    string randomsong = files[rand.Next(files.Length)];


    string randomshort = randomsong.Remove(randomsong.Length - 4, 4).Remove(0, 15);
    Console.WriteLine(randomshort);
    //Console.WriteLine(randomsong);

    var midi = MidiFile.Read(randomsong);
    Console.WriteLine(GetLength(randomsong));

    if (GetStartTempo(randomsong) == GetHighestTempo(randomsong))
    {
        Console.WriteLine(GetStartTempo(randomsong)+" BPM");
    }
    else
    {
        Console.WriteLine("Start tempo: " + GetStartTempo(randomsong) + " BPM");
        Console.WriteLine("Highest tempo: " + GetHighestTempo(randomsong) + " BPM");
    }




    using (var output = OutputDevice.GetByName("Microsoft GS Wavetable Synth"))
    using (var playback = midi.GetPlayback(output))
    {
                
        playback.Speed = 1.0;
        //playback.Play();
        midi.Play(output); //uitgecomment zodat ie m niet eerst hoeft af te spelen
        //Console.WriteLine("playing midi");
        System.Threading.Thread.Sleep(2000);
        Console.WriteLine();
    }
}
