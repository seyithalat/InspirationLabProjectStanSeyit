using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using InspirationLabProjectStanSeyit.Models;

namespace InspirationLabProjectStanSeyit
{
    public partial class UserSelectionDialog : Window
    {
        public UserSelectionDialog()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            var users = Data.GetAllUsers(); // You will need to implement this method in Data.cs
            UsersGrid.ItemsSource = users;
        }

        private void BanSelectedUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User user)
            {
                var result = MessageBox.Show($"Are you sure you want to ban user '{user.Username}' (ID {user.Id})? This cannot be undone.",
                                             "Confirm Ban",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Data.BanUser(user.Id);
                    MessageBox.Show($"User '{user.Username}' has been banned.");
                    LoadUsers();
                }
            }
            else
            {
                MessageBox.Show("Select a user to ban.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
