using InEenNotendop.Data;
using System.Windows;
using InEenNotendop.Business;
using NAudio.Midi;
using Moq;
using System.Windows.Controls;


namespace InEenNotendop.UI.Tests
{
    //testen van de moeilijkheidsgraad
    [TestClass]
    public class MoeilijkheidTests
    {
        private MoeilijkheidConverter _moeilijkheidConverter;

        [TestInitialize]
        public void Setup()
        {
            // Arrange: Initialize the MoeilijkheidConverter instance
            _moeilijkheidConverter = new MoeilijkheidConverter();
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
                string result = _moeilijkheidConverter.Convert(input);

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
                mainWindow.SettingsWindow.SetLightMode(mainWindow);
                lightmode = mainWindow.SettingsWindow.GetLightMode();

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
                mainWindow.SettingsWindow.SetDarkMode(mainWindow);
                lightmode = mainWindow.SettingsWindow.GetLightMode();

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
                SongsWindow songsWindow = new SongsWindow(mainWindow.SettingsWindow);

                Window expectedNewOwnerWindow = mainWindow;

                // Act
                mainWindow.SettingsWindow.MainMenu();

                // Assert
                Assert.AreEqual(expectedNewOwnerWindow, mainWindow.SettingsWindow.Owner);
            });

            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Start();
        }
    }

    [TestClass]
    public class MidiInputProcesserTest
    {
        private List<Note> _notes;
        private MidiFile _midiFile;

        [TestInitialize]
        public void Setup()
        {
            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();
            _midiFile = new MidiFile(
                @"UnitTestMidi.mid");
        }

        [TestMethod]
        public void MidiToList_MidiFileImported_ListContainsData()
        {
            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();

            _notes = midiInputProcessor.MidiToList(_midiFile);
            Assert.IsTrue(_notes[0] != null);
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


            _notes = midiInputProcessor.MidiToList(_midiFile);
            Assert.AreEqual(timeNote1Expected, _notes[0].NoteStartTime);
            Assert.AreEqual(timeNote2Expected, _notes[1].NoteStartTime);
            Assert.AreEqual(timeNote3Expected, _notes[2].NoteStartTime);
            Assert.AreEqual(timeNote4Expected, _notes[3].NoteStartTime);

        }
    }

    [TestClass]
    public class ScoreCalculatorTests
    {
        private MidiFile _midiFile;

        [TestInitialize]
        public void Setup()
        {
            _midiFile = new MidiFile(
                @"UnitTestMidi.mid");

        }

        [TestMethod]
        public void CalculateScore_CompareMidiFileToCopyOfMidiFile_ScoreShouldBe1000()
        {
            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();
            midiInputProcessor.MidiToList(_midiFile);

            midiInputProcessor.ListOfNotesPlayed = new List<Note>(midiInputProcessor.ListOfNotesSong);
            MidiInputScoreCalculator midiInputScoreCalculator = new MidiInputScoreCalculator(midiInputProcessor);
            Assert.AreEqual(1000, midiInputScoreCalculator.CalculateScoreAfterSongCompleted());
            midiInputProcessor.ListOfNotesPlayed.Clear();
        }

        [TestMethod]
        public void CalculateScore_ManualNotes_ScoreShouldBe850()
        {
            MidiInputProcessor midiInputProcessor = new MidiInputProcessor();
            midiInputProcessor.MidiToList(_midiFile);

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
            midiInputProcessor.MidiToList(_midiFile);

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
            midiInputProcessor.MidiToList(_midiFile);

            foreach (var note in midiInputProcessor.ListOfNotesSong)
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
            midiInputProcessor.MidiToList(_midiFile);

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
            midiInputProcessor.MidiToList(_midiFile);

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
            midiInputProcessor.MidiToList(_midiFile);

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
            midiInputProcessor.MidiToList(_midiFile);

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
            midiInputProcessor.MidiToList(_midiFile);

            List<Note> notes = new List<Note>();
            notes.Add(midiInputProcessor.ListOfNotesSong[1]);
            midiInputProcessor.ListOfNotesPlayed = notes;
            MidiInputScoreCalculator midiInputScoreCalculator = new MidiInputScoreCalculator(midiInputProcessor);

            var score = midiInputScoreCalculator.CalculateScoreAfterSongCompleted();
            Assert.AreEqual(score, 250);
        }
    }

    [TestClass]
    public class DatabaseTests
    {
        MoqDataAccess dataAccess = new();

        [TestMethod]
        public void FindDirectory_FindCorrectFile_UnitTestFile()
        {
            // Arrange
            string wantedDocument = "UnitTestFile.txt";

            string threeFoldersUp = Path.GetFullPath(Path.Combine("..", "..", ".."));
            string expectedDirectory = Path.Combine(threeFoldersUp, wantedDocument);

            // Act
            string foundDirectory = dataAccess.FindDirectory(wantedDocument);

            // Assert
            Assert.AreEqual(expectedDirectory, foundDirectory);
        }

        //[TestMethod]
        //public void UploadSongToDatabase_CorrectlyAddedToList_SongObjectsMatch()
        //{
        //    var nummersList = new List<Nummer>();
        //    nummersList.Add(new Nummer("Song 1", "Real Artist", 125, 100, 3, 1, "N/A", 100, "2:05", "Hard"));
        //    nummersList.Add(new Nummer("Song 2", "Fake Artist", 125, 100, 2, 2, "N/A", 100, "2:05", "Medium"));
        //    nummersList.Add(new Nummer("Song 3", "An Artist?", 125, 100, 1, 3, "N/A", 100, "2:05", "Easy"));

        //    Mock<IDatabaseInterface> mock = new Mock<IDatabaseInterface>();

        //    //Mocking UploadSongToDatabase
        //    mock.Setup(mock => mock.UploadSongToDataBase());

        //}
        // Ik snap het echt niet, ik heb uren geprobeerd en het is gewoon niet mogelijk in de tijd die we hebben nu
    }
}