using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InspirationLabProjectStanSeyit.Models;

namespace InspirationLabProjectStanSeyit
{
  
    public partial class Profile : Window
    {
        private UserProfile currentProfile;

        public Profile()
        {
            InitializeComponent();
            int userId = Session.CurrentUserId;
            currentProfile = Data.GetUserProfile(userId);
            if (currentProfile == null)
            {
                // If no profile exists, create a new one
                currentProfile = new UserProfile { UserId = userId };
            }
            LoadProfileToUI();
            Loaded += Profile_Loaded;
        }
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

        private void Profile_Loaded(object sender, RoutedEventArgs e)
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
        



        private void LoadProfileToUI()
        {
            if (currentProfile == null) return;
            NameTextBox.Text = currentProfile.Name ?? string.Empty;
            AgeTextBox.Text = currentProfile.Age?.ToString() ?? string.Empty;
            SchoolTextBox.Text = currentProfile.School ?? string.Empty;
            CourseTextBox.Text = currentProfile.Course ?? string.Empty;
            YearTextBox.Text = currentProfile.Year ?? string.Empty;
            BioTextBox.Text = currentProfile.Bio ?? string.Empty;
            BirthdayTextBox.Text = currentProfile.Birthday?.ToString("yyyy-MM-dd") ?? string.Empty;
            LocationTextBox.Text = currentProfile.Location ?? string.Empty;
            StudyHoursTextBlock.Text = currentProfile.StudyHours.ToString("0.0");

            // Assign a random avatar
            var avatars = new[] { "/Images/avatar1.png", "/Images/avatar2.png", "/Images/avatar3.png", "/Images/avatar4.png" };
            var rand = new Random(currentProfile.UserId); // Seed with UserId for consistency
            var avatar = avatars[rand.Next(avatars.Length)];
            ProfilePicture.Source = new BitmapImage(new Uri(avatar, UriKind.Relative));

            // Sync notes and study groups count
            var notes = Data.GetNotesForUser(currentProfile.UserId);
            var groups = Data.GetAllGroupsForUser(currentProfile.UserId);
            NotesCountTextBlock.Text = notes.Count.ToString();
            StudyGroupsCountTextBlock.Text = groups.Count.ToString();
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (currentProfile == null)
            {
                MessageBox.Show("Profile could not be loaded. Please try again or contact support.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            currentProfile.Name = NameTextBox.Text;
            if (int.TryParse(AgeTextBox.Text, out var age))
                currentProfile.Age = age;
            else
                currentProfile.Age = null;
            currentProfile.School = SchoolTextBox.Text;
            currentProfile.Course = CourseTextBox.Text;
            currentProfile.Year = YearTextBox.Text;
            currentProfile.Bio = BioTextBox.Text;
            if (DateTime.TryParse(BirthdayTextBox.Text, out var bday))
                currentProfile.Birthday = bday;
            else
                currentProfile.Birthday = null;
            currentProfile.Location = LocationTextBox.Text;
            Data.UpdateUserProfile(currentProfile);
            MessageBox.Show("Profile changes saved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StudyGroups_Click(object sender, RoutedEventArgs e)
        {
            var studyGroups = new StudyGroups();
            studyGroups.Show();
            this.Close();
        }

        private void Notes_Click(object sender, RoutedEventArgs e)
        {
            var studyMaterial = new StudyMaterial();
            studyMaterial.Show();
            this.Close();
        }

        private void BackToSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings();
            settingsWindow.Show();
            this.Close();
        }

        private void OpenMap_Click(object sender, RoutedEventArgs e)
        {
            var managementWindow = new Management();
            managementWindow.Show();
            this.Close();
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings();
            settingsWindow.ShowDialog();
        }

        private DateTime? sessionStartTime = null;

        private void StartStudySession()
        {
            sessionStartTime = DateTime.Now;
            // Optionally: Show a timer in the UI
        }

        private void EndStudySession()
        {
            if (sessionStartTime.HasValue)
            {
                var duration = DateTime.Now - sessionStartTime.Value;
                int minutes = (int)duration.TotalMinutes;
                Data.AddStudyMinutesToUser(Session.CurrentUserId, minutes);
                sessionStartTime = null;
            }
        }
    }
}
