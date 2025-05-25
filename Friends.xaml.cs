using System;
using System.Windows;
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
            RefreshPendingRequests();
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            // Optional: Code to move images back
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            // Optional: Code to move images forward
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
                RefreshPendingRequests();
            }
            else
            {
                MessageBox.Show("You cannot send a request to yourself or a request already exists.");
            }
        }

        private void OpenChat_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chat functionality not implemented.");
        }

        private void DeleteFriend_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Delete friend functionality not implemented.");
        }

        private void RefreshPendingRequests()
        {
            var pendingRequests = Data.GetPendingFriendRequests(Session.CurrentUserId);
            FriendsListBox.ItemsSource = pendingRequests;
        }
    }
}
