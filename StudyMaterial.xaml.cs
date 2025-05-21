using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace InspirationLabProjectStanSeyit
{
    public partial class StudyMaterial : Window
    {
        public StudyMaterial()
        {
            InitializeComponent();
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Previous image clicked.");
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Next image clicked.");
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
    }
}

