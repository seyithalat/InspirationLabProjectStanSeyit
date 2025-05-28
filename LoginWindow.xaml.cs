using System.Windows;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace InspirationLabProjectStanSeyit
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in both fields.", "Missing Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string connStr = "Server=localhost;Port=3307;Database=inspirationlabdb;Uid=root;Pwd=;";
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT Id, FirstName, PasswordHash, Banned FROM users WHERE Username = @username";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bool banned = reader.GetBoolean("Banned");
                            if (banned)
                            {
                                MessageBox.Show("The ban hammer has spoken", "Banned", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            string dbPassword = reader.GetString("PasswordHash");
                            if (dbPassword != password)
                            {
                                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            int userId = reader.GetInt32("Id");
                            string firstName = reader.GetString("FirstName");
                            Session.CurrentUserId = userId;
                            Session.CurrentUsername = username;
                            MessageBox.Show($"Welcome back, {firstName}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            var mainWindow = new MainWindow();
                            mainWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}