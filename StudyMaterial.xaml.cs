using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using InspirationLabProjectStanSeyit.Models;

namespace InspirationLabProjectStanSeyit
{
    public partial class StudyMaterial : Window
    {
        public StudyMaterial()
        {
            InitializeComponent();
            MyNotesList.DisplayMemberPath = "Title";
            LoadMyNotes();
        }

        private void LoadMyNotes()
        {
            MyNotesList.Items.Clear();
            var notes = Data.GetNotesForUser(Session.CurrentUserId);
            foreach (var note in notes)
            {
                MyNotesList.Items.Add(note);
            }
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
            OpenFileDialog dlg = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Text files (*.txt)|*.txt|PDF files (*.pdf)|*.pdf|PowerPoint files (*.ppt;*.pptx)|*.ppt;*.pptx"
            };
            if (dlg.ShowDialog() == true)
            {
                foreach (string file in dlg.FileNames)
                {
                    string ext = System.IO.Path.GetExtension(file).ToLower();
                    if (ext == ".txt" || ext == ".pdf" || ext == ".ppt" || ext == ".pptx")
                    {
                        var note = new Note
                        {
                            UserId = Session.CurrentUserId,
                            Title = Path.GetFileNameWithoutExtension(file),
                            Content = File.ReadAllBytes(file), // Save file content as byte[]
                            FilePath = file
                        };
                        Data.AddNote(note);
                    }
                    else
                    {
                        MessageBox.Show("Only .txt, .pdf, .ppt, and .pptx files are allowed.");
                    }
                }
                LoadMyNotes();
            }
        }

        private void OpenMyNote_Click(object sender, RoutedEventArgs e)
        {
            if (MyNotesList.SelectedItem is Note note && File.Exists(note.FilePath))
            {
                Process.Start(new ProcessStartInfo(note.FilePath) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("Please select a valid note to open.");
            }
        }

        private void DeleteMyNote_Click(object sender, RoutedEventArgs e)
        {
            if (MyNotesList.SelectedItem is Note note)
            {
                if (MessageBox.Show("Delete this note from the database?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Data.DeleteNote(note.Id);
                    LoadMyNotes();
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
            if (MyNotesList.SelectedItem is Note noteToShare)
            {
                // Get friends list
                var friends = Data.GetFriends(Session.CurrentUserId);
                if (friends.Count == 0)
                {
                    MessageBox.Show("You have no friends to share with.");
                    return;
                }

                // Prompt user to select a friend
                var selectFriendWindow = new Window
                {
                    Title = "Select Friend to Share With",
                    Width = 300,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    ResizeMode = ResizeMode.NoResize,
                    Owner = this
                };
                var stack = new StackPanel { Margin = new Thickness(10) };
                var combo = new ComboBox { ItemsSource = friends, SelectedIndex = 0, Margin = new Thickness(0,0,0,10) };
                var okBtn = new Button { Content = "Share", Width = 80, IsDefault = true, HorizontalAlignment = HorizontalAlignment.Right };
                stack.Children.Add(new TextBlock { Text = "Select a friend:", Margin = new Thickness(0,0,0,5) });
                stack.Children.Add(combo);
                stack.Children.Add(okBtn);
                selectFriendWindow.Content = stack;
                string selectedFriend = null;
                okBtn.Click += (s, args) => {
                    selectedFriend = combo.SelectedItem as string;
                    selectFriendWindow.DialogResult = true;
                    selectFriendWindow.Close();
                };
                if (selectFriendWindow.ShowDialog() == true && !string.IsNullOrEmpty(selectedFriend))
                {
                    // Get friend's userId
                    int friendId = Data.GetUserIdByUsername(selectedFriend);
                    if (friendId == -1)
                    {
                        MessageBox.Show("Could not find the selected friend.");
                        return;
                    }
                    // Copy note to friend's account
                    var sharedNote = new Note
                    {
                        UserId = friendId,
                        Title = noteToShare.Title,
                        Content = noteToShare.Content,
                        FilePath = noteToShare.FilePath
                    };
                    Data.AddNote(sharedNote);
                    MessageBox.Show($"Note shared with {selectedFriend}!");
                    // Add to SharedNotesList for feedback (show title)
                    SharedNotesList.Items.Add(noteToShare.Title);
                }
            }
            else
            {
                MessageBox.Show("Please select a note from 'My Notes' to share.");
            }
        }
    }
}

