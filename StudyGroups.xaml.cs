using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace InspirationLabProjectStanSeyit
{
    public partial class StudyGroups : Window
    {
        public StudyGroups()
        {
            InitializeComponent();
            // Optional: Load your images here if you want
            // Image1.Source = new BitmapImage(new Uri("path_to_your_image1.png"));
            // Image2.Source = new BitmapImage(new Uri("path_to_your_image2.png"));
            // Image3.Source = new BitmapImage(new Uri("path_to_your_image3.png"));
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            // Logic for previous images
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            // Logic for next images
        }

        private void InviteEmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InviteEmailTextBox.Text))
            {
                PlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                PlaceholderText.Visibility = Visibility.Collapsed;
            }
        }

        private void SendInvite_Click(object sender, RoutedEventArgs e)
        {
            string email = InviteEmailTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(email))
            {
                MessageBox.Show($"Invite sent to {email}!", "Invite Sent", MessageBoxButton.OK, MessageBoxImage.Information);
                InviteEmailTextBox.Text = "";
            }
            else
            {
                MessageBox.Show("Please enter an email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoBackToFeatures_Click(object sender, RoutedEventArgs e)
        {
            var featuresWindow = new Features();
            featuresWindow.Show();
            this.Close();
        }
    }
}