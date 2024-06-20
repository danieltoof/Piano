using NAudio.Midi;

namespace InEenNotendop.Business;

// Code for handling the Midi device input
public class MidiPlayer 
{
    private MidiOut midiOut;
    private readonly object owner;

    public MidiPlayer(string desiredDevice, object owner)
    {
        Dispose();
        // Find and initialize the Microsoft GS Wavetable Synth
        int deviceIndex = FindMidiDevice("Microsoft GS Wavetable Synth");
        if (deviceIndex != -1)
        {
            //midiOut = new MidiOut(deviceIndex);
        }
        else
        {
            throw new Exception($"{desiredDevice} not found");
        }

        this.owner = owner;
    }

    public void PlayNote(int note)
    {
        midiOut.Send(MidiMessage.StartNote(note, 120, 1).RawData);
    }

    public void StopNote(int note)
    {
        midiOut.Send(MidiMessage.StopNote(note, 127, 1).RawData);
    }

    public void Dispose()
    {
        midiOut?.Dispose();
        midiOut = null;
    }

    private int FindMidiDevice(string deviceName)
    {
        for (int deviceId = 0; deviceId < MidiOut.NumberOfDevices; deviceId++)
        {
            if (MidiOut.DeviceInfo(deviceId).ProductName.Contains(deviceName))
            {
                return deviceId;
            }
        }
        return -1;
    }
}