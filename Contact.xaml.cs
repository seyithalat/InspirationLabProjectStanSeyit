using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using InspirationLabProjectStanSeyit;


namespace InspirationLabProjectStanSeyit
{
    public partial class Contact : Window
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

        private void Contact_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateNavCarousel();
                // Hide the ReviewContactMessages button if the user is not an admin
                ReviewContactMessagesButton.Visibility = Data.IsUserAdmin(Session.CurrentUserId) ? Visibility.Visible : Visibility.Collapsed;
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
        public Contact()
        {
            InitializeComponent();
            Loaded += Contact_Loaded;
        }


        // Example: On Submit button click
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string subject = SubjectTextBox.Text.Trim();
            string message = MessageTextBox.Text.Trim();
            int? userId = Session.CurrentUserId; // Or null if not logged in

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Data.AddContactMessage(name, email, subject, message, userId);

            MessageBox.Show("Your message has been sent!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Optionally clear the form
            NameTextBox.Clear();
            EmailTextBox.Clear();
            SubjectTextBox.Clear();
            MessageTextBox.Clear();
        }

        private void ReviewContactMessages_Click(object sender, RoutedEventArgs e)
        {
            var adminWindow = new ContactMessagesAdmin();
            adminWindow.Show();
        }
    }
}
