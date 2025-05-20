using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace InspirationLabProjectStanSeyit
{
    public partial class StudyMaterial : Window
    {
        public StudyMaterial()
        {
            InitializeComponent();
            UpdateImageSet();
        }

        

        // ==== MY NOTES ====

        private void UploadMyNote_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog { Multiselect = true };
            if (dlg.ShowDialog() == true)
            {
                foreach (string file in dlg.FileNames)
                {
                    MyNotesList.Items.Add(file);
                }
            }
        }

        private void OpenMyNote_Click(object sender, RoutedEventArgs e)
        {
            if (MyNotesList.SelectedItem is string path && File.Exists(path))
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("Please select a valid note to open.");
            }
        }

        private void DeleteMyNote_Click(object sender, RoutedEventArgs e)
        {
            if (MyNotesList.SelectedItem != null)
            {
                if (MessageBox.Show("Delete this note from the list?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    MyNotesList.Items.Remove(MyNotesList.SelectedItem);
                }
            }
            else
            {
                MessageBox.Show("Please select a note to delete.");
            }
        }

        // ==== SHARED NOTES ====

        private void UploadSharedNote_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog { Multiselect = true };
            if (dlg.ShowDialog() == true)
            {
                foreach (string file in dlg.FileNames)
                {
                    SharedNotesList.Items.Add(file);
                }
            }
        }

        private void OpenSharedNote_Click(object sender, RoutedEventArgs e)
        {
            if (SharedNotesList.SelectedItem is string path && File.Exists(path))
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("Please select a valid shared note to open.");
            }
        }

        private void DeleteSharedNote_Click(object sender, RoutedEventArgs e)
        {
            if (SharedNotesList.SelectedItem != null)
            {
                if (MessageBox.Show("Delete this shared note from the list?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SharedNotesList.Items.Remove(SharedNotesList.SelectedItem);
                }
            }
            else
            {
                MessageBox.Show("Please select a shared note to delete.");
            }
        }

        private void ShareNote_Click(object sender, RoutedEventArgs e)
        {
            if (SharedNotesList.SelectedItem is string filePath && File.Exists(filePath))
            {
                string sharedFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SharedNotes");
                if (!Directory.Exists(sharedFolder)) Directory.CreateDirectory(sharedFolder);

                string dest = Path.Combine(sharedFolder, Path.GetFileName(filePath));
                File.Copy(filePath, dest, true);

                MessageBox.Show($"Note shared to:\n{dest}");
            }
            else
            {
                MessageBox.Show("Select a valid shared note to share.");
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateImageSet();
        }

        private readonly List<string> imagePaths = new List<string>
        {
            "Images/homepagecarouselimage.jpg",
            "Images/featurescarouselimage.jpg",
            "Images/profilecarouselimage.jpg",
            "Images/plannercarouselimage.jpg",
            "Images/groupscarouselimage.jpg",
            "Images/gamescarouselimage.jpg",
            "Images/notescarouselimage.jpg",
            "Images/managementcarouselimage.jpg",
            "Images/contactcarouselimage.jpg"
        };

        private List<string> imageTitles = new List<string>
        {
            "Features",
            "Profile",
            "Planner",
            "Groups",
            "Focus Games",
            "Notes",
            "Management",
            "Contact",
            "Settings"
        };

        private int startIndex = 0;

        public void UpdateImageSet()
        {
            if (imagePaths.Count < 3) return;

            int i1 = startIndex % imagePaths.Count;
            int i2 = (startIndex + 1) % imagePaths.Count;
            int i3 = (startIndex + 2) % imagePaths.Count;

            Image1.Source = new BitmapImage(new Uri(imagePaths[i1], UriKind.Relative));
            Image2.Source = new BitmapImage(new Uri(imagePaths[i2], UriKind.Relative));
            Image3.Source = new BitmapImage(new Uri(imagePaths[i3], UriKind.Relative));

            Label1.Text = imageTitles[i1];
            Label2.Text = imageTitles[i2];
            Label3.Text = imageTitles[i3];
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            startIndex = (startIndex - 1 + imagePaths.Count) % imagePaths.Count;
            UpdateImageSet();
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            startIndex = (startIndex + 1) % imagePaths.Count;
            UpdateImageSet();
        }

        private void Image1_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(startIndex % imagePaths.Count);
        }

        private void Image2_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage((startIndex + 1) % imagePaths.Count);
        }

        private void Image3_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage((startIndex + 2) % imagePaths.Count);
        }

        private void NavigateToPage(int index)
        {
            Window newWindow = null;

            switch (index)
            {
                case 0: // Features
                    newWindow = new Features();
                    break;
                case 1: // Profile
                    newWindow = new Profile();
                    break;
                case 2: // Planner
                    newWindow = new Planner();
                    break;
                case 3: // Groups
                    newWindow = new StudyGroups();
                    break;
                case 4: // Focus Games
                    newWindow = new GamePage();
                    break;
                case 5: // Notes
                    newWindow = new StudyMaterial();
                    break;
                case 6: // Management
                    newWindow = new Management();
                    break;
                case 7: // Contact
                    newWindow = new Contact();
                    break;
                case 8: // Settings
                    newWindow = new Settings();
                    break;
                default:
                    newWindow = new Features();
                    break;
            }

            if (newWindow != null)
            {
                newWindow.Show();
                this.Close();
            }

        }
    }
}

