using Microsoft.VisualStudio.TestTools.UnitTesting;
using InEenNotendop.UI;
using InEenNotendop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Security.Permissions;
using System.Windows;

namespace InEenNotendop.UI.Tests
{
    //testen van de moeilijkheidsgraad
    [TestClass()]
    public class MoeilijkheidTests
    {

        private MoeilijkheidConverter converter;

        [TestInitialize]
        public void SetUp()
        {
            converter = new MoeilijkheidConverter();
        }

        [DataTestMethod]
        [DataRow(1, "easy")]
        [DataRow(2, "medium")]
        [DataRow(3, "hard")]
        [DataRow(4, "Unknown")]
        public void Convert_GeeftDeJuisteMoeilijkheid(int moeilijkheid, string expectedResult)
        {
            // Act
            var result = converter.Convert(moeilijkheid, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ConvertBack_ShouldThrowNotImplementedException()
        {
            // Act & Assert
            Assert.ThrowsException<System.NotImplementedException>(() => converter.ConvertBack(null, null, null, CultureInfo.InvariantCulture));
        }
    }

    // testen van het selectingWindow
    [TestClass]
    public class SelectieWindowTesten
    {
        [TestMethod]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            int nummerId = 1;
            string moeilijkheidText = "easy";
            string title = "Title";
            string artist = "Artist";
            string lengte = "180";
            int bpm = 120;
            string expectedNummerIdText = "Clicked on Nummer with ID: 1";
            string expectedMoeilijkheidText = "Difficulty: easy";

            Exception backgroundThreadException = null;

            // Act
            var thread = new Thread(() =>
            {
                try
                {
                    var window = new SelectingWindow(nummerId, moeilijkheidText, title, artist, lengte,  bpm, "path",this);

                    // Assert
                    var viewModel = window.DataContext as SelectingWindow.NummerDetailsViewModel;
                    Assert.IsNotNull(viewModel);
                    Assert.AreEqual(expectedNummerIdText, viewModel.NummerIdText);
                    Assert.AreEqual(expectedMoeilijkheidText, viewModel.MoeilijkheidText);
                    Assert.AreEqual(title, viewModel.Title);
                    Assert.AreEqual(artist, viewModel.Artiest);
                    Assert.AreEqual(lengte, viewModel.Lengte);
                    Assert.AreEqual(bpm, viewModel.Bpm);
                }
                catch (Exception ex)
                {
                    backgroundThreadException = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            // Assert for any exceptions occurred in the background thread
            if (backgroundThreadException != null)
            {
                Assert.Fail($"Background thread threw an exception: {backgroundThreadException}");
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

}