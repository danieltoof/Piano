using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        private string FilePath;
        private string FileName;
        public ImportWindow()
        {
            InitializeComponent();
        }

        private void selectFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".midi"; // Default file extension
            dialog.Filter = "midi documenten (.mid)|*.mid"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Get the data from selected file
                FilePath = dialog.FileName;
                FileName = dialog.SafeFileName;
            }
        }

        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            int error = 1;
            int diffecultyCheckbox = 1;
            string myText = ImportName.Text;
            var checkedValue = "Easy";
            string songName;
            string songArtist;

            RadioButton rb = FindVisualChildren<RadioButton>(ImportDiffeculty).FirstOrDefault(x => x.IsChecked == true);
            if (rb != null)
            {
                checkedValue = rb.Content.ToString();
                switch (checkedValue)
                {
                    case string x when x.StartsWith("Easy"):
                        diffecultyCheckbox |= 1;
                        break;
                    case string x when x.StartsWith("Medium"):
                        diffecultyCheckbox |= 2;
                        break;
                    case string x when x.StartsWith("Hard"):
                        diffecultyCheckbox |= 3;
                        break;
                }
            }
            // check if song is not null
            if (string.IsNullOrEmpty(ImportName.Text))
            {
                error = 1;
                MessageBox.Show("Vul een naam in");
            } else { songName = ImportName.Text; error = 0; }

            // check if artist is not null
            if (string.IsNullOrEmpty(ImportArtist.Text))
            {
                error = 1;
                MessageBox.Show("Vul een artiest in");
            } else { songArtist = ImportArtist.Text; error = 0; }

            // move selected file
            if (string.IsNullOrEmpty(FilePath) && string.IsNullOrEmpty(FileName)) { 
                error = 1; 
                MessageBox.Show("Selecteer een bestand"); 
            } else { File.Move(FilePath, @"\Resources\Songs\Song_" + FileName); error = 0; }

            // check if there are errors
            if (error == 0)
            {
                MessageBox.Show("succes");
                await Task.Delay(1000);
                Close();
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                int NbChild = VisualTreeHelper.GetChildrenCount(depObj);

                for (int i = 0; i < NbChild; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childNiv2 in FindVisualChildren<T>(child))
                    {
                        yield return childNiv2;
                    }
                }
            }
        }
    }
}
