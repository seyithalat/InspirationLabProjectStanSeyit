using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InspirationLabProjectStanSeyit.Models;

namespace InspirationLabProjectStanSeyit
{

    //Interaction logic for ContactMessagesAdmin.xaml

    public partial class ContactMessagesAdmin : Window
    {
        public ContactMessagesAdmin()
        {
            // Only allow admins
            if (!Data.IsUserAdmin(Session.CurrentUserId))
            {
                MessageBox.Show("Access denied. You must be an administrator to view this page.",
                                "Access Denied",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                Close();
                return;
            }
            InitializeComponent();
            LoadMessages();
        }

        private void LoadMessages()
        {
            var messages = Data.GetAllContactMessages();
            MessagesGrid.ItemsSource = messages;
        }

        private void MarkHandled_Click(object sender, RoutedEventArgs e)
        {
            if (MessagesGrid.SelectedItem is ContactMessage msg)
            {
                Data.MarkContactMessageHandled(msg.Id, Session.CurrentUserId);
                LoadMessages();
            }
            else
            {
                MessageBox.Show("Select a message to mark as handled.");
            }
        }

        private void BanUser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UserSelectionDialog();
            dialog.ShowDialog();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadMessages();
        }
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
