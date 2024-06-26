﻿using InEenNotendop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InEenNotendop.Data;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for SongsWindow.xaml
    /// </summary>
    public partial class HighscoreList : Window
    {
        public SettingsWindow SettingsWindow;
        SqlDataAccess _sqlDataAccess = new();
        private int _lightmodeImport;
        private int _difficulty = 0;
        public bool SongIsFinished;

        public HighscoreList(SettingsWindow settingsWindow)
        {
            InitializeComponent();

            this.SettingsWindow = settingsWindow;
            this.SettingsWindow.ChangeSettingsOwner(this);

            SettingsWindow.CheckDarkOrLight(this);
        }

        // Creates pop-up window with detailed song information


        //AANPASSEN NAAR SCORE SCREEN
        private void OnNumberClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement clickedElement)
            {
                var song = clickedElement.DataContext as Song;
                if (song != null)
                {
                    int nummerId = song.Id;
                    String title = song.Title;
                    String artist = song.Artist;
                    int fullTime = song.FullTime;
                    int bpm = song.Bpm;
                    String filePath = song.Filepath;
                    string convertedTime = song.ConvertedTime;
                    DifficultyConverter difficultyConverter = new DifficultyConverter();
                    int currentScore = song.Score;
                    string difficultyText = difficultyConverter.Convert(song.Difficulty);

                    SelectingWindow detailsWindow = new SelectingWindow(nummerId, difficultyText, title, artist, fullTime, bpm, filePath, convertedTime, this, currentScore);
                    detailsWindow.Owner = this;
                    detailsWindow.ShowDialog();
                }
            }
        }

        // Goes back to main menu
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow.MainMenu();
            Close();

        }
    }
}