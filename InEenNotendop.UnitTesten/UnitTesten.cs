using InEenNotendop.Data;
using System.Windows;
using InEenNotendop.Business;
using NAudio.Midi;


namespace InEenNotendop.UI.Tests
{
    //testen van de moeilijkheidsgraad
    [TestClass]
    public class MoeilijkheidTests
    {
        private MoeilijkheidConverter moeilijkheidConverter;

        [TestInitialize]
        public void Setup()
        {
            // Arrange: Initialize the MoeilijkheidConverter instance
            moeilijkheidConverter = new MoeilijkheidConverter();
        }

        [TestMethod]
        public void Convert_ValidAndInvalidInputs_CorrectOutputs()
        {
            // Arrange: Define expected results
            var expectedResults = new (int input, string expectedOutput)[]
            {
                (1, "Easy"),
                (2, "Medium"),
                (3, "Hard"),
                (4, "Unknown"),
                (0, "Unknown"),
                (-1, "Unknown"),
                (999, "Unknown")
            };

            foreach (var (input, expectedOutput) in expectedResults)
            {
                // Act
                string result = moeilijkheidConverter.Convert(input);

                // Assert
                Assert.AreEqual(expectedOutput, result, $"Failed for input {input}");
            }
        }
    }

    [TestClass]
    public class SettingsWindowTesten
    {
        [TestMethod]
        public void SetLightMode_PressedButton_ChangedValue()
        {
            Thread newThread = new Thread(() =>
            {
                // Arrange
                MainWindow mainWindow = new MainWindow();
                int lightmode;

                // Act
                mainWindow.settingsWindow.SetLightMode(mainWindow);
                lightmode = mainWindow.settingsWindow.GetLightMode();

                // Assert
                Assert.IsTrue(lightmode == 1);
            });

            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Start();
        }

        [TestMethod]
        public void SetDarkMode_PressedButton_ChangedValue()
        {
            Thread newThread = new Thread(() =>
            {
                // Arrange
                MainWindow mainWindow = new MainWindow();
                int lightmode;

                // Act
                mainWindow.settingsWindow.SetDarkMode(mainWindow);
                lightmode = mainWindow.settingsWindow.GetLightMode();

                // Assert
                Assert.IsTrue(lightmode == 0);
            });

            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Start();
        }

        [TestMethod]
        public void MainMenu_PressedMainMenuButton_ChangedOwner()
        {
            Thread newThread = new Thread(() =>
            {
                // Arrange
                MainWindow mainWindow = new MainWindow();
                SongsWindow songsWindow = new SongsWindow(mainWindow.settingsWindow);

                Window expectedNewOwnerWindow = mainWindow;

                // Act
                mainWindow.settingsWindow.MainMenu();

                // Assert
                Assert.AreEqual(expectedNewOwnerWindow, mainWindow.settingsWindow.Owner);
            });

            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Start();
        }
    }
    //testen van afspeelscherm functionaliteit
    [TestClass]
    public class AfspeelSchermTesten
    {
        [TestMethod]
        public void PlayMidi_CheckError()
        {
            //Arrange

            //Act
            var thread = new Thread(() =>
            {
                try
                {
                    
                }
                catch { }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            //Assert

        }
    }

    [TestClass]
    public class MidiInputProcesserTest
    {
        private List<Note> notes;
        private MidiFile midiFile;

        [TestInitialize]
        public void Setup()
        {
            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();
            midiFile = new MidiFile(
                @"UnitTestMidi.mid");
        }

        [TestMethod]
        public void MidiToList_MidiFileImported_ListContainsData()
        {
            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();

            notes = midiInputProcessor.MidiToList(midiFile);
            Assert.IsTrue(notes[0] != null);
        }

        [TestMethod]
        public void MidiToList_MidiFileImported_MidiFileConvertedToListWithCorrectStartTimes()
        {
            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();

            TimeSpan timeNote1Expected = new TimeSpan();
            TimeSpan timeNote2Expected = new TimeSpan();
            TimeSpan timeNote3Expected = new TimeSpan();
            TimeSpan timeNote4Expected = new TimeSpan();

            string format = @"hh\:mm\:ss\.fffffff";

            TimeSpan.TryParseExact("00:00:00", format, null, out timeNote1Expected);
            TimeSpan.TryParseExact("00:00:00.2727269", format, null, out timeNote2Expected);
            TimeSpan.TryParseExact("00:00:00.6818175", format, null, out timeNote3Expected);
            TimeSpan.TryParseExact("00:00:01.2272714", format, null, out timeNote4Expected);


            notes = midiInputProcessor.MidiToList(midiFile);
            Assert.AreEqual(timeNote1Expected, notes[0].NoteStartTime);
            Assert.AreEqual(timeNote2Expected, notes[1].NoteStartTime);
            Assert.AreEqual(timeNote3Expected, notes[2].NoteStartTime);
            Assert.AreEqual(timeNote4Expected, notes[3].NoteStartTime);

        }
    }

    [TestClass]
    public class ScoreCalculatorTests
    {
        private MidiFile midiFile;

        [TestInitialize]
        public void Setup()
        {
            midiFile = new MidiFile(
                @"UnitTestMidi.mid");

        }

        [TestMethod]
        public void CalculateScore_CompareMidiFileToCopyOfMidiFile_ScoreShouldBe1000()
        {
            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();
            midiInputProcessor.MidiToList(midiFile);

            midiInputProcessor.ListOfNotesPlayed = new List<Note>(midiInputProcessor.ListOfNotesSong);
            MidiInputScoreCalculator midiInputScoreCalculator = new MidiInputScoreCalculator(midiInputProcessor);
            Assert.AreEqual(1000, midiInputScoreCalculator.CalculateScoreAfterSongCompleted());
            midiInputProcessor.ListOfNotesPlayed.Clear();
        }

        [TestMethod]
        public void CalculateScore_ManualNotes_ScoreShouldBe850()
        {
            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();
            midiInputProcessor.MidiToList(midiFile);

            TimeSpan timeNote1Expected = new TimeSpan();
            TimeSpan timeNote2Expected = new TimeSpan();
            TimeSpan timeNote3Expected = new TimeSpan();
            TimeSpan timeNote4Expected = new TimeSpan();

            string format = @"hh\:mm\:ss\.fffffff";

            TimeSpan.TryParseExact("00:00:00", format, null, out timeNote1Expected);
            TimeSpan.TryParseExact("00:00:00.3327269", format, null, out timeNote2Expected);
            TimeSpan.TryParseExact("00:00:00.7518175", format, null, out timeNote3Expected);
            TimeSpan.TryParseExact("00:00:01.2872714", format, null, out timeNote4Expected);


            var notesSong = midiInputProcessor.ListOfNotesSong;
            var notes = new List<Note>()
            {
                new Note(notesSong[0].NoteNumber, timeNote1Expected),
                new Note(notesSong[1].NoteNumber, timeNote2Expected),
                new Note(notesSong[2].NoteNumber, timeNote3Expected),
                new Note(notesSong[3].NoteNumber, timeNote4Expected)
            };

            midiInputProcessor.ListOfNotesPlayed = notes;
            MidiInputScoreCalculator midiInputScoreCalculator = new MidiInputScoreCalculator(midiInputProcessor);
            var score = midiInputScoreCalculator.CalculateScoreAfterSongCompleted();
            Assert.AreEqual(850, score);

        }

        [TestMethod]
        public void CalculateScore_CompareMidiFileShifted10Milliseconds_ScoreShouldBe1000()
        {
            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();
            midiInputProcessor.MidiToList(midiFile);

            List<Note> alteredList = new List<Note>(midiInputProcessor.ListOfNotesSong);
            TimeSpan increment = TimeSpan.FromMilliseconds(10);
            for (int i = 0; i < alteredList.Count; i++)
            {
                alteredList[i].NoteStartTime = alteredList[i].NoteStartTime.Add(increment);
            }
            midiInputProcessor.ListOfNotesPlayed = alteredList;
            MidiInputScoreCalculator midiInputScoreCalculator = new MidiInputScoreCalculator(midiInputProcessor);
            Assert.AreEqual(1000, midiInputScoreCalculator.CalculateScoreAfterSongCompleted());
            midiInputProcessor.ListOfNotesPlayed.Clear();
        }

        [TestMethod]
        public void CalculateScore_CompareMidiFileShifted30Milliseconds_ScoreShouldBe950()
        {

            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();
            midiInputProcessor.MidiToList(midiFile);

            foreach(var note in midiInputProcessor.ListOfNotesSong)
            {
                TimeSpan increment = TimeSpan.FromMilliseconds(30);
                midiInputProcessor.ListOfNotesPlayed.Add(new Note(note.NoteNumber, note.NoteStartTime + increment));
            }

            MidiInputScoreCalculator midiInputScoreCalculator = new MidiInputScoreCalculator(midiInputProcessor);
            Assert.AreEqual(952, midiInputScoreCalculator.CalculateScoreAfterSongCompleted());
            midiInputProcessor.ListOfNotesPlayed.Clear();
        }

        [TestMethod]
        public void CalculateScore_CompareMidiFileShifted60Milliseconds_ScoreShouldBe800()
        {

            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();
            midiInputProcessor.MidiToList(midiFile);

            foreach (var note in midiInputProcessor.ListOfNotesSong)
            {
                TimeSpan increment = TimeSpan.FromMilliseconds(60);
                midiInputProcessor.ListOfNotesPlayed.Add(new Note(note.NoteNumber, note.NoteStartTime + increment));
            }

            MidiInputScoreCalculator midiInputScoreCalculator = new MidiInputScoreCalculator(midiInputProcessor);
            Assert.AreEqual(800, midiInputScoreCalculator.CalculateScoreAfterSongCompleted());
            midiInputProcessor.ListOfNotesPlayed.Clear();

        }

        [TestMethod]
        public void CalculateScore_CompareMidiFileShifted90Milliseconds_ScoreShouldBe600()
        {

            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();
            midiInputProcessor.MidiToList(midiFile);

            foreach (var note in midiInputProcessor.ListOfNotesSong)
            {
                TimeSpan increment = TimeSpan.FromMilliseconds(90);
                midiInputProcessor.ListOfNotesPlayed.Add(new Note(note.NoteNumber, note.NoteStartTime + increment));
            }

            MidiInputScoreCalculator midiInputScoreCalculator = new MidiInputScoreCalculator(midiInputProcessor);
            Assert.AreEqual(600, midiInputScoreCalculator.CalculateScoreAfterSongCompleted());
            midiInputProcessor.ListOfNotesPlayed.Clear();

        }

        [TestMethod]
        public void CalculateScore_CompareMidiFileShifted120Milliseconds_ScoreShouldBe300()
        {

            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();
            midiInputProcessor.MidiToList(midiFile);

            foreach (var note in midiInputProcessor.ListOfNotesSong)
            {
                TimeSpan increment = TimeSpan.FromMilliseconds(120);
                midiInputProcessor.ListOfNotesPlayed.Add(new Note(note.NoteNumber, note.NoteStartTime + increment));
            }

            MidiInputScoreCalculator midiInputScoreCalculator = new MidiInputScoreCalculator(midiInputProcessor);
            Assert.AreEqual(300, midiInputScoreCalculator.CalculateScoreAfterSongCompleted());
            midiInputProcessor.ListOfNotesPlayed.Clear();

        }

        [TestMethod]
        public void CalculateScore_CompareMidiFileShifted150Milliseconds_ScoreShouldBe0()
        {

            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();
            midiInputProcessor.MidiToList(midiFile);

            foreach (var note in midiInputProcessor.ListOfNotesSong)
            {
                TimeSpan increment = TimeSpan.FromMilliseconds(150);
                midiInputProcessor.ListOfNotesPlayed.Add(new Note(note.NoteNumber, note.NoteStartTime + increment));
            }

            MidiInputScoreCalculator midiInputScoreCalculator = new MidiInputScoreCalculator(midiInputProcessor);
            Assert.AreEqual(0, midiInputScoreCalculator.CalculateScoreAfterSongCompleted());
            midiInputProcessor.ListOfNotesPlayed.Clear();

        }

        [TestMethod]
        public void CalculateScore_CalculateOneNoteCorrect_ShouldBe250()
        {
            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();
            midiInputProcessor.MidiToList(midiFile);

            List<Note> notes = new List<Note>();
            notes.Add(midiInputProcessor.ListOfNotesSong[1]);
            midiInputProcessor.ListOfNotesPlayed = notes;
            MidiInputScoreCalculator midiInputScoreCalculator = new MidiInputScoreCalculator(midiInputProcessor);

            var score = midiInputScoreCalculator.CalculateScoreAfterSongCompleted();
            Assert.AreEqual(score, 250);
        }

    }



}