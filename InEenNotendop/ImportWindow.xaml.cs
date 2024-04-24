using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InEenNotendop.UI
{
    /// <summary>
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        public ImportWindow()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            int diffecultyCheckbox = 1;
            string myText = ImportName.Text;
            var checkedValue = "Easy";
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
            string songName;
            string songArtist;
            songName = ImportName.Text;
            songArtist = ImportArtist.Text;
        }

        private void selectFile_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".mid";
            dlg.Filter = "MID Files (*.mid)|*.mid";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {}

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {}

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

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                switch (columnName)
                {
                    case nameof(ImportArtist):
                        if (string.IsNullOrWhiteSpace(ImportArtist.Text))
                            error = "First artist cannot be empty.";
                        break;

                    case nameof(ImportName.Text):
                        if (string.IsNullOrWhiteSpace(ImportName.Text))
                            error = "Last name cannot be empty.";
                        break;
                }

                return error;
            }
        }
        public string Error => string.Empty;
    }
}
