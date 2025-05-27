using System.Windows;
using InspirationLabProjectStanSeyit;

namespace InspirationLabProjectStanSeyit
{
    public partial class Contact : Window
    {
        public Contact()
        {
            InitializeComponent();
        }


        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            // 🔁 Add your previous image logic here
            MessageBox.Show("Previous image clicked.");
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            // 🔁 Add your next image logic here
            MessageBox.Show("Next image clicked.");
        }

        private void ImageButton1_Click(object sender, RoutedEventArgs e)
        {
            // 🔁 Add logic for when ImageButton1 is clicked
            MessageBox.Show("Image Button 1 clicked.");
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

