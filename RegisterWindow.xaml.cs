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

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all fields.", "Missing Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"Thanks for registering, {username}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            var loginWindow = new LoginWindow();
            Application.Current.MainWindow.Show(); 
            this.Close();                          
        }
    }
}