using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;

namespace Testing.UIChangeInducedByMIDI;


    /* The MidiInProc function is the callback function for handling incoming MIDI messages.
     MidiInProc is a placeholder for the application-supplied function name; MidiInMsgHandler.
     The address of this function can be specified in the callback-address parameter of the midiInOpen function.
  ​   Syntax:
          void CALLBACK MidiInProc(
            HMIDIIN hMidiIn,
            UINT wMsg,                   MIDI message type.
            DWORD_PTR dwInstance,
            DWORD_PTR dwParam1,          The meaning of the dwParam1 and dwParam2 parameters 
            DWORD_PTR dwParam2           is specific to the message type. Typically dwParam1 
                                         contains the MIDI message that was received.
          ); 
     The MIDI message is packed into a double word value as follows:
     ---------DOUBLE WORD-------
     -----WORD----|-----WORD----
     -BYTE-|-BYTE-|-BYTE-|-BYTE-
            Data2  Data1  Status

     So a note on message, which normally reads as 90 3C 7F (9=note on, 0=channel, 3C=note number, 7F=velocity)
     is packed as 00 7F 3C 90 into a DWORD. (This is decimal 8338576.)
*/
    public delegate void MidiInProc(IntPtr handle, UInt32 msgType, UInt32 instance, UInt32 param1, UInt32 param2);

    public abstract class MidiDevice    // 'abstract' means MidiDevice will not be instantiated (only derived classes will)
    {
        [DllImport("winmm.dll")]
        private static extern UInt32 midiInGetNumDevs();

        [DllImport("winmm.dll")]
        private static extern UInt32 midiOutGetNumDevs();

        // in C# Charset.Ansi is used for marshaling (overriding the Charset.Auto CLR default)
        // correspondingly, we use the 'A' version of midiInGetDevCapsA (as opposed to midiInGetDevCapsW)
        // (the CharSet = CharSet.Ansi expression below is redundant)
        [DllImport("winmm.dll", EntryPoint = "midiInGetDevCaps", CharSet = CharSet.Ansi)]
        private static extern MMResult midiInGetDevCapsA(UInt32 devId, ref MidiInCaps devCaps, UInt32 devCapsSize);

        [DllImport("winmm.dll", EntryPoint = "midiOutGetDevCaps")]
        private static extern MMResult midiOutGetDevCapsA(UInt32 devId, ref MidiOutCaps devCaps, UInt32 devCapsSize);

        // when opening a MIDI port the handle variable must be passed by reference, 
        // because it will be assigned a value by the MIDI device
        [DllImport("winmm.dll")]
        protected static extern MMResult midiInOpen(ref IntPtr handle, UInt32 devId, MidiInProc midiInCallback, UInt32 instance, MidiCallbackFlags flags);

        [DllImport("winmm.dll")]
        protected static extern MMResult midiInClose(IntPtr handle);

        // a callback for MIDI 'OUT' will not be implemented
        // in this case NULL should be passed in for the callback argument
        // here, this will be done with IntPtr.Zero
        [DllImport("winmm.dll")]
        protected static extern MMResult midiOutOpen(ref IntPtr handle, UInt32 devId, IntPtr midiOutCallback, UInt32 instance, MidiCallbackFlags flags);

        [DllImport("winmm.dll")]
        protected static extern MMResult midiOutClose(IntPtr handle);

        // MMRESULT midiOutShortMsg(
        //   HMIDIOUT hmo,
        //   DWORD    dwMsg);     this is a 32-bit 'unsigned long' in unmanaged C which corresponds to .NET's System.UInt32
        [DllImport("winmm.dll")]
        protected static extern MMResult midiOutShortMsg(IntPtr handle, UInt32 msg);

        public enum MMResult
        {
            NoError = 0,
            UnspecError = 1,
            BadDeciveID = 2,
            NotEnabled = 3,
            DeviceAlreadyAllocated = 4,
            InvalidHandle = 5,
            NoDriver = 6,
            NoMem = 7,
            NotSupported = 8,
            BadErrNum = 9,
            InvalidFlag = 10,
            InvalidParam = 11,
            HandleBusy = 12,
            InvalidAlias = 13,
            BadDB = 14,
            KeyNotFound = 15,
            ReadError = 16,
            WriteError = 17,
            DeleteError = 18,
            RegistryValueNotFound = 19,
            NoDriverCallback = 20,
            LastError = 20,
            MoreData = 21
        }

        public enum MidiCallbackFlags
        {
            NoCallBack = 0,
            Window = 0x10000,
            Thread = 0x20000,
            Function = 0x30000,
            CallBackEvent = 0x50000,
            MidiIOStatus = 0x20
        }

        public enum MidiMsgTypes
        {
            MM_MIM_OPEN = 961,
            MM_MIM_CLOSE = 962,
            MM_MIM_DATA = 963,
            MM_MIM_LONGDATA = 964,
            MM_MIM_ERROR = 965,
            MM_MIM_LONGERROR = 966,
            MM_MIM_MOREDATA = 972
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MidiInCaps
        {
            public UInt16 manufacturerId;
            public UInt16 productId;
            public UInt32 driverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public String deviceName;
            public UInt32 support;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MidiOutCaps
        {
            public UInt16 manufacturerId;
            public UInt16 productId;
            public UInt32 driverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public String deviceName;
            public UInt16 wTechnology;
            public UInt16 wSounds;
            public UInt16 wNotes;
            public UInt16 wChannelMask;
            public UInt32 support;
        }

        private static void GetMidiDevices(ArrayList midiInDevices, ArrayList midiOutDevices)
        {
            midiInDevices.Clear();

            UInt32 numDevs = midiInGetNumDevs();
            if (numDevs > 0)
            {
                MidiInCaps inCaps = new MidiInCaps();
                for (UInt32 dev = 0; dev < numDevs; ++dev)
                {
                    MMResult res = midiInGetDevCapsA(dev, ref inCaps, (UInt32)Marshal.SizeOf(inCaps));
                    if (res == MMResult.NoError)
                    {
                        MidiInDevice inDev = new MidiInDevice
                        {
                            deviceId = dev,
                            deviceName = inCaps.deviceName,
                            isRolandFP30Compatible = (inCaps.deviceName.StartsWith(Constants.ROLAND_DEVICE))
                        };
                        midiInDevices.Add(inDev);
                    }
                }
            }

            midiOutDevices.Clear();

            numDevs = midiOutGetNumDevs();
            if (numDevs > 0)
            {
                MidiOutCaps outCaps = new MidiOutCaps();
                for (UInt32 dev = 0; dev < numDevs; ++dev)
                {
                    MMResult res = midiOutGetDevCapsA(dev, ref outCaps, (UInt32)Marshal.SizeOf(outCaps));
                    if (res == MMResult.NoError)
                    {
                        MidiOutDevice outDev = new MidiOutDevice
                        {
                            deviceId = dev,
                            deviceName = outCaps.deviceName,
                            isRolandFP30Compatible = (outCaps.deviceName.StartsWith(Constants.ROLAND_DEVICE))
                        };
                        midiOutDevices.Add(outDev);
                    }
                }
            }
        }

        static public void ConfigMidiDevices(ArrayList midiInDevices, ArrayList midiOutDevices)
        {
            GetMidiDevices(midiInDevices, midiOutDevices);

        }

        protected IntPtr handle;   // a handle to reference the device port in communications
        protected UInt32 deviceId;
        public String deviceName;
        public bool isRolandFP30Compatible;
        public bool isOpen; // = false; device is closed initially

        public abstract void Open();    // 'abstract' means derived classes must implement this method
        public abstract void Close();
    }

    public class MidiInDevice : MidiDevice
    {
        public MidiInProc midiInCallback;

        public override void Open()
        {
            midiInCallback = new MidiInProc(MidiInMsgHandler);

            MMResult result = MMResult.NoError;

            if (!isOpen)
            {
                result = midiInOpen(ref handle, deviceId, midiInCallback, deviceId, MidiCallbackFlags.Function | MidiCallbackFlags.MidiIOStatus);

                if (result == MMResult.NoError)
                    isOpen = true;
            }
        }

        public override void Close()
        {
            MMResult result = MMResult.NoError;

            if (isOpen)
            {
                result = midiInClose(handle);

                if (result == MMResult.NoError)
                    isOpen = false;
            }
        }

        public void MidiInMsgHandler(IntPtr handle, UInt32 msgType, UInt32 instance, UInt32 param1, UInt32 param2)
        {
            // the messages received here will inlude the following (among others):
            // MIDI device open : msgType = 961
            //    -"-     close : msgType = 962
            // note on / off : msgType = 963     

            switch ((MidiMsgTypes)msgType)
            {
                case MidiMsgTypes.MM_MIM_ERROR:
                    Console.WriteLine("A MIDI error was received.");
                    break;
                case MidiMsgTypes.MM_MIM_OPEN:
                    Console.WriteLine("The MIDI device is opened.");
                    break;
                case MidiMsgTypes.MM_MIM_CLOSE:
                    Console.WriteLine("The MIDI device is closed.");
                    break;
                case MidiMsgTypes.MM_MIM_DATA:
                    Console.WriteLine("MIDI data were received.");
                    break;
                default:
                    Console.WriteLine("An unidentified message was received.");
                    break;
            }
        }
    }

    public class MidiOutDevice : MidiDevice
    {
        public override void Open()
        {
            MMResult result = MMResult.NoError;

            if (!isOpen)
            {
                result = midiOutOpen(ref handle, deviceId, IntPtr.Zero, 0, MidiCallbackFlags.NoCallBack);

                if (result == MMResult.NoError)
                    isOpen = true;
            }
        }

        public override void Close()
        {
            MMResult result = MMResult.NoError;

            if (isOpen)
            {
                result = midiOutClose(handle);

                if (result == MMResult.NoError)
                    isOpen = false;
            }
        }

        public void SendCCMsg(UInt32 msg)
        {
            if (isOpen)
                midiOutShortMsg(handle, msg);
        }
    }
