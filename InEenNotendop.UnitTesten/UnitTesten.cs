﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using InEenNotendop.UI;
using InEenNotendop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

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
            int lengte = 180;
            int bpm = 120;
            string expectedNummerIdText = "Clicked on Nummer with ID: 1";
            string expectedMoeilijkheidText = "Difficulty: easy";

            Exception backgroundThreadException = null;

            // Act
            var thread = new Thread(() =>
            {
                try
                {
                    var window = new SelectingWindow(nummerId, moeilijkheidText, title, artist, lengte, bpm);

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

}