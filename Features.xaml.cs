using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace InspirationLabProjectStanSeyit
{

    public partial class Features : Window

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
                var profile = Data.GetUserProfile(Session.CurrentUserId);
                if (profile != null)
                {
                    ProfileNameTextBlock.Text = $"Name: {profile.Name}";
                    ProfileAgeTextBlock.Text = $"Age: {(profile.Age.HasValue ? profile.Age.Value.ToString() : "-")}";
                    ProfileSchoolTextBlock.Text = $"School: {profile.School}";
                    ProfileCourseTextBlock.Text = $"Course: {profile.Course}";
                    ProfileYearTextBlock.Text = $"Schoolyear: {profile.Year}";
                }
                else
                {
                    ProfileNameTextBlock.Text = "Name: -";
                    ProfileAgeTextBlock.Text = "Age: -";
                    ProfileSchoolTextBlock.Text = "School: -";
                    ProfileCourseTextBlock.Text = "Course: -";
                    ProfileYearTextBlock.Text = "Schoolyear: -";
                }
                LoadPendingInvitations();
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
        public Features()
        {
            InitializeComponent();
            Loaded += Features_Loaded;
        }

                     
        private void StudyGroupsButton_Click(object sender, RoutedEventArgs e)
        {
            var studyGroupsPage = new StudyGroups();
            studyGroupsPage.Show();
            this.Hide(); // Hide the Features window
        }

        private void FriendsButton_Click(object sender, RoutedEventArgs e)
        {
            var friendsPage = new Friends();
            friendsPage.Show();
            this.Hide(); // Hide the Features window
        }

        private void MapButton_Click(object sender, RoutedEventArgs e)
        {
            var mapPage = new Map();
            mapPage.Show();
            this.Hide(); // Hide the Features window
        }

        private void BackToSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings();
            settingsWindow.ShowDialog();
        }

        private void OpenMap_Click(object sender, RoutedEventArgs e)
        {
            var mapWindow = new Map();
            mapWindow.Show();
            this.Close();
        }

        private void SendInviteButton_Click(object sender, RoutedEventArgs e)
        {
            string email = InviteEmailTextBox.Text.Trim();
            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Add invitation to the database
            Data.AddInvitation(Session.CurrentUserId, email);
            MessageBox.Show($"Invitation sent to {email}!", "Invite Sent", MessageBoxButton.OK, MessageBoxImage.Information);
            InviteEmailTextBox.Text = string.Empty;
            LoadPendingInvitations();
        }

        private void LoadPendingInvitations()
        {
            var pending = Data.GetPendingInvitations(Session.CurrentUserId);
            PendingInvitationsList.ItemsSource = pending;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}