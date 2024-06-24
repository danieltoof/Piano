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