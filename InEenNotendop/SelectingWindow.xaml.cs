﻿using InEenNotendop.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for SelectingWindow.xaml
    /// </summary>
    public partial class SelectingWindow : Window
    {
        public Window Owner { get; set; }
        private SongsWindow songsWindow;
        DataProgram dataProgram = new DataProgram();
        string FilePath;
        private int nummerID;
        public SelectingWindow(int nummerId, string moeilijkheidText, string Title, string Artiest, int Lengte, int Bpm, string FilePath, object sender)
        {
            InitializeComponent();
            songsWindow = (SongsWindow)sender;
            Owner = songsWindow;
            this.FilePath = FilePath;
            DataContext = new NummerDetailsViewModel(nummerId, moeilijkheidText, Title, Artiest, Lengte, Bpm);
            FillDataGrid(nummerId);
            this.nummerID = nummerId;
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }


        public class NummerDetailsViewModel // Class and constructor for detailed song screen
        {
            public string NummerIdText { get; }
            public string MoeilijkheidText { get; }
            public string Title { get; }
            public string Artiest { get; }
            public int Lengte { get; }
            public int Bpm { get; }

            public NummerDetailsViewModel(int nummerId, string moeilijkheidText, string title, string artiest, int lengte, int bpm)
            {
                NummerIdText = $"Clicked on Nummer with ID: {nummerId}";
                MoeilijkheidText = $"Difficulty: {moeilijkheidText}";
                Title = title;
                Artiest = artiest;
                Lengte = lengte;
                Bpm = bpm;

            }
        }
        private void PLAY_Button_Click(object sender, RoutedEventArgs e) // 
        {
            MidiPlayWindow midiPlayWindow = new MidiPlayWindow(FilePath, this, false, nummerID);
            Owner.Close();
            midiPlayWindow.Show();
            Close();
        }
        private void AUTOPLAY_Button_Click(object sender, RoutedEventArgs e)
        {
            MidiPlayWindow midiPlayWindow = new MidiPlayWindow(FilePath, this, true, nummerID);
            //SongsWindow songsWindow = SongsWindow();
            Owner.Close();
            midiPlayWindow.Show();
            Close();

        }

        private void OnDownloadClicked(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as NummerDetailsViewModel;

            dataProgram.DownloadSong(viewModel.Artiest,viewModel.Title);  
        }


        private void FillDataGrid(int nummerId)
        {
            HighScoresGrid.ItemsSource = dataProgram.GetDataForGrid(nummerId);
        }


    }

}
