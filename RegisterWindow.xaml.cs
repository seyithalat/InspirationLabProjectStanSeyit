using System.Windows;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

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

            // Store password as plain text (not recommended for production)
            string passwordToStore = password;

            string connStr = "Server=localhost;Port=3307;Database=inspirationlabdb;Uid=root;Pwd=;";
            using (var conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO users (Username, Email, PasswordHash, FirstName, LastName) VALUES (@username, @email, @password, @first_name, @last_name)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@password", passwordToStore);
                        cmd.Parameters.AddWithValue("@first_name", firstName);
                        cmd.Parameters.AddWithValue("@last_name", lastName);

                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show($"Registration successful! Welcome, {firstName}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1062) // Duplicate entry
                        MessageBox.Show("Username or email already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}