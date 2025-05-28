using System.Windows;
using InspirationLabProjectStanSeyit;


namespace InspirationLabProjectStanSeyit
{
    public partial class Contact : Window
    {
        public Contact()
        {
            InitializeComponent();
            UpdateImageSet();
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

        // <== Here is the newly added Submit button click handler
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string email = EmailTextBox.Text;
            string subject = SubjectTextBox.Text;
            string message = MessageTextBox.Text;

            // Simple confirmation popup
            MessageBox.Show($"Thank you, {name}! Your message has been submitted.",
                "Submission Successful", MessageBoxButton.OK, MessageBoxImage.Information);

            // Clear the form
            NameTextBox.Clear();
            EmailTextBox.Clear();
            SubjectTextBox.Clear();
            MessageTextBox.Clear();
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
