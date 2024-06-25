using NAudio.Midi;

namespace InEenNotendop.Business;

// Code for handling the Midi device input
public class MidiPlayer 
{
    private MidiOut _midiOut;
    private object _owner;

    public MidiPlayer(string desiredDevice, object owner)
    {
        Dispose();
        // Find and initialize the Microsoft GS Wavetable Synth
        int deviceIndex = MidiUtilities.FindMidiDevice("Microsoft GS Wavetable Synth", InOrOut.OUT);
        if (deviceIndex != -1)
        {
            try
            {
                _midiOut = new MidiOut(deviceIndex);
            }
            catch (Exception ex)
            {
            }

        }
        else
        {
            throw new Exception($"{desiredDevice} not found");
        }

        _owner = owner;
    }

    public void PlayNote(int note)
    {
            _midiOut?.Send(MidiMessage.StartNote(note, 120, 1).RawData);
    }

    public void StopNote(int note)
    {
        _midiOut?.Send(MidiMessage.StopNote(note, 127, 1).RawData);        
    }

    public void Dispose()
    {
        _midiOut?.Dispose();
        _midiOut = null;
    }
}