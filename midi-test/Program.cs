// See https://aka.ms/new-console-template for more information

using CleanCode.Data;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using System.Numerics;

var midi = MidiFile.Read(@"..\..\..\midis\Darude_-_Sandstorm__Izzet_Selanik__Arne_Mulder_20061031124814.mid");
//var midi = MidiFile.Read(@"..\..\..\midis\AUD_HTX0525.mid");

// lees en print lengte van midi
TimeSpan midiFileDuration = midi.GetDuration<MetricTimeSpan>();
Console.WriteLine(midiFileDuration);

Console.WriteLine(midiFileDuration.TotalSeconds);
Console.WriteLine(Convert.ToInt32(midiFileDuration.TotalSeconds));

int minutes = (Convert.ToInt32(midiFileDuration.TotalSeconds)/60);
int seconds = (Convert.ToInt32(midiFileDuration.TotalSeconds)%60);

Console.WriteLine(minutes+ ":" + seconds); //print lengte todo: zorg voor nullen onder 10 en niet hoger dan 59


using (var output = OutputDevice.GetByName("Microsoft GS Wavetable Synth"))
using (var playback = midi.GetPlayback(output))
{
    playback.Speed = 1.0;
    //playback.Play();
    midi.Play(output);
    Console.WriteLine("playing midi");
    System.Threading.Thread.Sleep(2000);
}