using System.Windows;

namespace InspirationLabProjectStanSeyit
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string email = EmailBox.Text;
            string password = PasswordBox.Password;
            string firstName = FirstNameBox.Text;
            string lastName = LastNameBox.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || 
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(firstName) || 
                string.IsNullOrWhiteSpace(lastName))
            {
                MessageBox.Show("Please fill in all fields.", "Missing Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Registration successful! Welcome, {firstName}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}