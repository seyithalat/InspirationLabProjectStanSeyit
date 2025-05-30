using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using InspirationLabProjectStanSeyit.Models;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace InspirationLabProjectStanSeyit
{
    public partial class StudyMaterial : Window
    {
        private List<string> navImages = new List<string>
{
    "Images/featurescarouselimage.jpg",
    "Images/profilecarouselimage.jpg",
    "Images/plannercarouselimage.jpg",
    "Images/groupscarouselimage.jpg",
    "Images/gamescarouselimage.jpg",
    "Images/notescarouselimage.jpg",
    "Images/managementcarouselimage.jpg",
    "Images/contactcarouselimage.jpg"
};

        private List<string> navTitles = new List<string>
{
    "Features",
    "Profile",
    "Planner",
    "Groups",
    "Games",
    "Notes",
    "Management",
    "Contact"
};

        private int currentNavIndex = 0;

        private void Features_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateNavCarousel();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during initialization: {ex.Message}", "Initialization Error");
            }
        }

        private void UpdateNavCarousel()
        {
            if (navImages.Count < 3) return;

            int i1 = currentNavIndex % navImages.Count;
            int i2 = (currentNavIndex + 1) % navImages.Count;
            int i3 = (currentNavIndex + 2) % navImages.Count;

            try
            {
                NavImage1.Source = new BitmapImage(new Uri(navImages[i1], UriKind.Relative));
                NavImage2.Source = new BitmapImage(new Uri(navImages[i2], UriKind.Relative));
                NavImage3.Source = new BitmapImage(new Uri(navImages[i3], UriKind.Relative));

                NavLabel1.Text = navTitles[i1];
                NavLabel2.Text = navTitles[i2];
                NavLabel3.Text = navTitles[i3];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading navigation images: {ex.Message}");
            }
        }

        private void NavPrevImage_Click(object sender, RoutedEventArgs e)
        {
            currentNavIndex = (currentNavIndex - 1 + navImages.Count) % navImages.Count;
            UpdateNavCarousel();
        }

        private void NavNextImage_Click(object sender, RoutedEventArgs e)
        {
            currentNavIndex = (currentNavIndex + 1) % navImages.Count;
            UpdateNavCarousel();
        }

        private void NavImage1_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(NavLabel1.Text);
        }

        private void NavImage2_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(NavLabel2.Text);
        }

        private void NavImage3_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(NavLabel3.Text);
        }
        private void NavigateToPage(string pageName)
        {
            Window newWindow = null;
            switch (pageName.ToLower())
            {
                case "features":
                    newWindow = new Features();
                    break;
                case "profile":
                    newWindow = new Profile();
                    break;
                case "planner":
                    newWindow = new Planner();
                    break;
                case "groups":
                    newWindow = new StudyGroups();
                    break;
                case "games":
                    newWindow = new GamePage();
                    break;
                case "notes":
                    newWindow = new StudyMaterial();
                    break;
                case "management":
                    newWindow = new Management();
                    break;
                case "contact":
                    newWindow = new Contact();
                    break;
            }

            if (newWindow != null)
            {
                newWindow.Show();
                this.Close();
            }
        }
        private void StudyMaterial_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateNavCarousel();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during initialization: {ex.Message}", "Initialization Error");
            }
        }
        public StudyMaterial()
        {
            InitializeComponent();
            MyNotesList.DisplayMemberPath = "Title";
            LoadMyNotes();
            LoadSharedNotes();
            Loaded += StudyMaterial_Loaded;
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

        private void LoadSharedNotes()
        {
            SharedNotesList.Items.Clear();
            var sharedNotes = Data.GetAllSharedNotesForUser(Session.CurrentUserId);
            foreach (var note in sharedNotes)
            {
                SharedNotesList.Items.Add(note);
            }
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
                            Content = File.ReadAllBytes(file),
                            FilePath = file,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
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

        private void OpenSharedNote_Click(object sender, RoutedEventArgs e)
        {
            if (SharedNotesList.SelectedItem is Note note && File.Exists(note.FilePath))
            {
                Process.Start(new ProcessStartInfo(note.FilePath) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("Please select a valid shared note to open.");
            }
        }

        private void DeleteSharedNote_Click(object sender, RoutedEventArgs e)
        {
            if (SharedNotesList.SelectedItem is Note note)
            {
                if (MessageBox.Show("Delete this shared note from your list?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Data.DeleteSharedNote(note.Id, Session.CurrentUserId);
                    LoadSharedNotes();
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
                    // Share note with friend
                    Data.ShareNote(noteToShare.Id, friendId);
                    MessageBox.Show($"Note shared with {selectedFriend}!");
                }
            }
            else
            {
                MessageBox.Show("Please select a note from 'My Notes' to share.");
            }
        }

        private void RefreshNotes_Click(object sender, RoutedEventArgs e)
        {
            LoadMyNotes();
            LoadSharedNotes();
        }
    }
}

