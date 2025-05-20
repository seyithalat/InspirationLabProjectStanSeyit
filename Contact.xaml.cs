using System.Windows;

namespace InspirationLabProjectStanSeyit
{
    public partial class Contact : Window
    {
        public Contact()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string email = EmailTextBox.Text;
            string subject = SubjectTextBox.Text;
            string message = MessageTextBox.Text;

            MessageBox.Show($"Thank you, {name}!\n\nSubject: {subject}\nMessage received.",
                "Submission Successful", MessageBoxButton.OK, MessageBoxImage.Information);

            NameTextBox.Clear();
            EmailTextBox.Clear();
            SubjectTextBox.Clear();
            MessageTextBox.Clear();
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
    }
}

