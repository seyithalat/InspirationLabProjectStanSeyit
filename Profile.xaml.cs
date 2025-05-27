using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InspirationLabProjectStanSeyit.Models;

namespace InspirationLabProjectStanSeyit
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        private UserProfile currentProfile;

        public Profile()
        {
            InitializeComponent();
            LoadProfile();
        }

        private void LoadProfile()
        {
            int userId = Session.CurrentUserId;
            currentProfile = Data.GetUserProfile(userId) ?? new UserProfile { UserId = userId };
            NameTextBox.Text = currentProfile.Name ?? "";
            AgeTextBox.Text = currentProfile.Age?.ToString() ?? "";
            SchoolTextBox.Text = currentProfile.School ?? "";
            CourseTextBox.Text = currentProfile.Course ?? "";
            YearTextBox.Text = currentProfile.Year ?? "";
            BioTextBox.Text = currentProfile.Bio ?? "";
            BirthdayTextBox.Text = currentProfile.Birthday?.ToString("yyyy-MM-dd") ?? "";
            LocationTextBox.Text = currentProfile.Location ?? "";
            StudyHoursTextBlock.Text = currentProfile.StudyHours.ToString("0.0");

            // Assign a random avatar
            var avatars = new[] { "/Images/avatar1.png", "/Images/avatar2.png", "/Images/avatar3.png", "/Images/avatar4.png" };
            var rand = new Random(currentProfile.UserId); // Seed with UserId for consistency
            var avatar = avatars[rand.Next(avatars.Length)];
            ProfilePicture.Source = new BitmapImage(new Uri(avatar, UriKind.Relative));

            // Sync notes and study groups count
            var notes = Data.GetNotesForUser(userId);
            var groups = Data.GetAllGroupsForUser(userId);
            NotesCountTextBlock.Text = notes.Count.ToString();
            StudyGroupsCountTextBlock.Text = groups.Count.ToString();
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
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
