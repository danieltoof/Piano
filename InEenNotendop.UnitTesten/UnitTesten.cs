using System.Windows;
using InEenNotendop.Business;
using InEenNotendop.Data;
using InEenNotendop.UI;
using NAudio.Midi;

namespace InEenNotendop.UnitTesten
{
    //testen van de moeilijkheidsgraad
    [TestClass]
    public class MoeilijkheidTests
    {
        private DifficultyConverter _difficultyConverter;

        [TestInitialize]
        public void Setup()
        {
            // Arrange: Initialize the MoeilijkheidConverter instance
            _difficultyConverter = new DifficultyConverter();
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
                string result = _difficultyConverter.Convert(input);

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
                bool lightmode;

                // Act
                mainWindow.SettingsWindow.SetLightMode(mainWindow);
                lightmode = mainWindow.SettingsWindow.GetLightMode();

                // Assert
                Assert.IsTrue(lightmode);
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
                bool lightmode;

                // Act
                mainWindow.SettingsWindow.SetDarkMode(mainWindow);
                lightmode = mainWindow.SettingsWindow.GetLightMode();

                // Assert
                Assert.IsFalse(lightmode);
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
    public class MidiToListConverterTests
    {
        private NoteCollection _unitTestSong;
        private TimeSpan _timeNote1Expected, _timeNote2Expected,
           _timeNote3Expected, _timeNote4Expected = new();


        [TestInitialize]
        public void Setup()
        {
            // Creëren van NoteCollection die we gebruiken als vergelijkingsmateriaal
            _unitTestSong = new(new MidiFile(
                @"UnitTestMidi.mid"));

            string format = @"hh\:mm\:ss\.fffffff";
            TimeSpan.TryParseExact("00:00:00", format, null, out _timeNote1Expected);
            TimeSpan.TryParseExact("00:00:00.2727269", format, null, out _timeNote2Expected);
            TimeSpan.TryParseExact("00:00:00.6818175", format, null, out _timeNote3Expected);
            TimeSpan.TryParseExact("00:00:01.2272714", format, null, out _timeNote4Expected);
        }

        [TestMethod]
        public void MidiToListConverter_MidiFileImported_ListContainsData()
        {
            Assert.IsTrue(_unitTestSong.Notes[0] != null);
        }

        [TestMethod]
        public void MidiToListConverter_MidiFileImported_MidiFileConvertedToListWithCorrectStartTimes()
        {
            Assert.AreEqual(_timeNote1Expected, _unitTestSong.Notes[0].NoteStartTime);
            Assert.AreEqual(_timeNote2Expected, _unitTestSong.Notes[1].NoteStartTime);
            Assert.AreEqual(_timeNote3Expected, _unitTestSong.Notes[2].NoteStartTime);
            Assert.AreEqual(_timeNote4Expected, _unitTestSong.Notes[3].NoteStartTime);

        }

    }

    [TestClass]
    public class ScoreCalculatorTests
    {
        private NoteCollection _unitTestSong;
        //private NoteCollection _compareSong;

        [TestInitialize]
        public void Setup()
        {
            _unitTestSong = new(new MidiFile(
                @"UnitTestMidi.mid"));
            //_compareSong = new();

        }

        [TestMethod]
        public void ScoreCalculatorCalculateScore_CalculateScore0msOffset_ScoreShouldB1000()
        {
            Assert.AreEqual(1000, ScoreCalculator.CalculateScore(_unitTestSong, NoteTimeManipulator.GenerateDelayedNoteCollection(_unitTestSong, 0)));
        }
        [TestMethod]
        public void ScoreCalculatorCalculateScore_CalculateScore30msOffset_ScoreShouldBe950()
        {
            Assert.AreEqual(950, ScoreCalculator.CalculateScore(_unitTestSong, NoteTimeManipulator.GenerateDelayedNoteCollection(_unitTestSong, 30)));
        }
        [TestMethod]
        public void ScoreCalculatorCalculateScore_CalculateScore60msOffset_ScoreShouldBe800()
        {
            Assert.AreEqual(800, ScoreCalculator.CalculateScore(_unitTestSong, NoteTimeManipulator.GenerateDelayedNoteCollection(_unitTestSong, 60)));
        }
        [TestMethod]
        public void ScoreCalculatorCalculateScore_CalculateScore90msOffset_ScoreShouldBe600()
        {
            Assert.AreEqual(600, ScoreCalculator.CalculateScore(_unitTestSong, NoteTimeManipulator.GenerateDelayedNoteCollection(_unitTestSong, 90)));
        }
        [TestMethod]
        public void ScoreCalculatorCalculateScore_CalculateScore120msOffset_ScoreShouldBe300()
        {
            Assert.AreEqual(300, ScoreCalculator.CalculateScore(_unitTestSong, NoteTimeManipulator.GenerateDelayedNoteCollection(_unitTestSong, 120)));
        }
        [TestMethod]
        public void ScoreCalculatorCalculateScore_CalculateScore90msOffset_ScoreShouldBe0()
        {
            Assert.AreEqual(0, ScoreCalculator.CalculateScore(_unitTestSong, NoteTimeManipulator.GenerateDelayedNoteCollection(_unitTestSong, 150)));
        }



    }
}