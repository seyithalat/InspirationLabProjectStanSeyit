using System;
using System.Windows;
using System.Windows.Controls;
using InspirationLabProjectStanSeyit.Models;
using MySql.Data.MySqlClient;

namespace InspirationLabProjectStanSeyit
{
    public partial class Friends : Window
    {
        // Connection string for phpMyAdmin MySQL database (same as Data.cs)
        private static string connectionString = "Server=localhost;Port=3307;Database=inspirationlabdb;Uid=root;Pwd=;";

        public Friends()
        {
            InitializeComponent();
            Console.WriteLine($"Current User ID: {Session.CurrentUserId}"); // Debug log
            RefreshPendingRequests();
            RefreshFriendsList();
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            // Optional: Code to move images back
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            // Optional: Code to move images forward
        }

        private void RefreshPendingRequests()
        {
            var pendingRequests = Data.GetPendingFriendRequests(Session.CurrentUserId);
            Console.WriteLine($"Number of pending requests: {pendingRequests.Count}"); // Debug log
            foreach (var request in pendingRequests)
            {
                Console.WriteLine($"Pending request from: {request}"); // Debug log
            }
            PendingRequestsListBox.ItemsSource = pendingRequests;
        }

        private void RefreshFriendsList()
        {
            var friends = Data.GetFriends(Session.CurrentUserId);
            FriendsListBox.ItemsSource = friends;
        }

        private void AcceptRequest_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                string friendUsername = button.Tag.ToString();
                if (Data.AcceptFriendRequest(Session.CurrentUserId, friendUsername))
                {
                    MessageBox.Show($"Friend request from {friendUsername} accepted!");
                    RefreshPendingRequests();
                    RefreshFriendsList();
                }
                else
                {
                    MessageBox.Show("Failed to accept friend request.");
                }
            }
        }

        private void DeclineRequest_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                string friendUsername = button.Tag.ToString();
                if (Data.DeclineFriendRequest(Session.CurrentUserId, friendUsername))
                {
                    MessageBox.Show($"Friend request from {friendUsername} declined.");
                    RefreshPendingRequests();
                }
                else
                {
                    MessageBox.Show("Failed to decline friend request.");
                }
            }
        }

        private void AddFriend_Click(object sender, RoutedEventArgs e)
        {
            int currentUserId = Session.CurrentUserId;
            string input = AddFriendTextBox.Text.Trim();

            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Please enter a full name, email, or username.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Data.UserExists(input))
            {
                MessageBox.Show("The username doesn't exist.");
                return;
            }

            if (Data.SendFriendRequest(currentUserId, input))
            {
                MessageBox.Show("Friend request sent!");
                AddFriendTextBox.Text = "";
                RefreshPendingRequests();
            }
            else
            {
                MessageBox.Show("You cannot send a request to yourself or a request already exists.");
            }
        }

        private void OpenChat_Click(object sender, RoutedEventArgs e)
        {
            if (FriendsListBox.SelectedItem != null)
            {
                string selectedFriend = FriendsListBox.SelectedItem.ToString();
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id FROM users WHERE Username = @username LIMIT 1";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", selectedFriend);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int friendId = reader.GetInt32("Id");
                                var chatWindow = new Chat(friendId, selectedFriend);
                                chatWindow.Show();
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a friend to chat with.");
            }
        }

        private void DeleteFriend_Click(object sender, RoutedEventArgs e)
        {
            if (FriendsListBox.SelectedItem != null)
            {
                string selectedFriend = FriendsListBox.SelectedItem.ToString();
                var result = MessageBox.Show(
                    $"Are you sure you want to remove {selectedFriend} from your friends list?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (Data.DeleteFriend(Session.CurrentUserId, selectedFriend))
                    {
                        MessageBox.Show($"{selectedFriend} has been removed from your friends list.");
                        RefreshFriendsList();
                    }
                    else
                    {
                        MessageBox.Show("Failed to remove friend. Please try again.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a friend to delete.");
            }
        }

        private void GoBackToMain_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
