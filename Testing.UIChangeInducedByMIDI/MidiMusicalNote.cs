namespace Testing.UIChangeInducedByMIDI;

public class MidiMusicalNote
{
    public MidiMusicalNote() { }

    public MidiMusicalNote(UInt32 channelNum, UInt32 noteNum, UInt32 velocity)
    {
        this.channelNum = channelNum;
        this.noteNum = noteNum;
        this.velocity = velocity;
    }

    UInt32 channelNum = 0;     // 0-15
    UInt32 noteNum = 60;       // middle C
    UInt32 velocity = 100;  // 0-127
    // although each of the values of the above variables would fit into a byte
    // we are using 32 bit integers for them, because the bits will be shifted
    // up-and-down within a 32 bit space during processing
    bool isPlaying;

    public void BeginPlay(MidiOutDevice dev)
    {
        UInt32 msg = 0;

        if (!isPlaying)
        {
            msg = (velocity << 16);  // velocity is packed into the low order byte of the high order word of the double-word
            msg += (noteNum << 8);     // the note number goes into the high order byte of the low order word of the double-word
            msg += (0x90 + channelNum); // note-on command on channel n = 0x9n = 0x90 + n 
        }
        if (msg > 0)
        {
            dev.SendCCMsg(msg);
            isPlaying = true;
        }
    }

    public void EndPlay(MidiOutDevice dev)
    {
        UInt32 msg = 0;

        if (isPlaying)
        {
            msg = (noteNum << 8);       // the note number goes into the high order byte of the low order word of the double-word
            msg += (0x90 + channelNum); // note-on (or note-off, when velocity is zero) command on channel n = 0x9n = 0x90 + n 
        }
        if (msg > 0)
        {
            dev.SendCCMsg(msg);
            isPlaying = false;
        }
    }
}