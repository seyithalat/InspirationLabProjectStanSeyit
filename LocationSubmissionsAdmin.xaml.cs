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
using System.Windows.Shapes;
using InspirationLabProjectStanSeyit.Models;

namespace InspirationLabProjectStanSeyit
{
    public partial class LocationSubmissionsAdmin : Window
    {
        // Connection string for phpMyAdmin MySQL database (same as Data.cs)
        public LocationSubmissionsAdmin()
        {
            // Check if the current user is an admin
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
            LoadSubmissions();
        }

        private void LoadSubmissions()
        {
            var pending = Data.GetPendingLocationSubmissions();
            SubmissionsGrid.ItemsSource = pending;
        }

        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            if (SubmissionsGrid.SelectedItem is LocationSubmission submission)
            {
                Data.UpdateLocationSubmissionStatus(submission.Id, "Approved");
                LoadSubmissions();
            }
            else
            {
                MessageBox.Show("Select a submission to approve.");
            }
        }

        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            if (SubmissionsGrid.SelectedItem is LocationSubmission submission)
            {
                Data.UpdateLocationSubmissionStatus(submission.Id, "Rejected");
                LoadSubmissions();
            }
            else
            {
                MessageBox.Show("Select a submission to reject.");
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadSubmissions();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (SubmissionsGrid.SelectedItem is LocationSubmission submission)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the location '{submission.Name}'? This cannot be undone.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Data.DeleteLocationSubmission(submission.Id);
                    LoadSubmissions();
                }
            }
            else
            {
                MessageBox.Show("Select a submission to delete.");
            }
        }
    }
}
