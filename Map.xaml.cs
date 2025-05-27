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
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace InspirationLabProjectStanSeyit
{
    public partial class Map : Window
    {
        public Map()
        {
            InitializeComponent();
            RefreshMap();
            // Show admin panel if user is admin
            if (Data.IsUserAdmin(Session.CurrentUserId))
            {
                AdminPanel.Visibility = Visibility.Visible;
                LoadApprovedLocationsList();
            }
            else
            {
                AdminPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void RefreshMap()
        {
            string mapUrl = GetStaticMapUrl();
            StudyMapImage.Source = new BitmapImage(new Uri(mapUrl));
            if (Data.IsUserAdmin(Session.CurrentUserId))
            {
                LoadApprovedLocationsList();
            }
        }

        private string GetStaticMapUrl()
        {
            // Center on Mechelen
            string center = "Mechelen,Belgium";
            int zoom = 14;
            string size = "600x400";
            string apiKey = "AIzaSyDWRmX1HI6B5-3vxI2f4jVLdmUUPomh3Wc";

            // Get approved locations from database using Data class
            var markers = Data.GetApprovedLocations();
            // Add default locations if no approved locations exist
            if (markers.Count == 0)
            {
                markers.Add("Mechelen,Belgium"); // City center
                markers.Add("Hogeschool Thomas More Mechelen");
                markers.Add("Kruidtuin Mechelen");
            }

            string markerString = string.Join("|", markers.ConvertAll(Uri.EscapeDataString));
            string url = $"https://maps.googleapis.com/maps/api/staticmap?center={Uri.EscapeDataString(center)}&zoom={zoom}&size={size}&markers={markerString}&key={apiKey}";
            return url;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshMap();
            if (Data.IsUserAdmin(Session.CurrentUserId))
            {
                LoadApprovedLocationsList();
            }
        }

        private void SubmitLocation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(LocationNameBox.Text) ||
                    string.IsNullOrWhiteSpace(AddressBox.Text))
                {
                    MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Use the Data class to add the submission with the current user ID
                Data.AddLocationSubmission(LocationNameBox.Text, DescriptionBox.Text, AddressBox.Text, Session.CurrentUserId);

                MessageBox.Show("Location submitted successfully! It will be reviewed by an administrator.",
                              "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Clear form
                LocationNameBox.Clear();
                DescriptionBox.Clear();
                AddressBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting location: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadApprovedLocationsList()
        {
            var approved = Data.GetApprovedLocationSubmissions();
            ApprovedLocationsList.ItemsSource = approved;
        }

        private void DeleteApprovedLocation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var approved = Data.GetApprovedLocationSubmissions();
                var toDelete = approved.FirstOrDefault(s => s.Id == id);
                if (toDelete != null)
                {
                    var result = MessageBox.Show($"Are you sure you want to delete the location '{toDelete.Name}'? This cannot be undone.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        Data.DeleteLocationSubmission(toDelete.Id);
                        RefreshMap();
                        LoadApprovedLocationsList();
                    }
                }
                else
                {
                    MessageBox.Show("Could not find the location to delete.");
                }
            }
        }

        private void GoBackToFeatures_Click(object sender, RoutedEventArgs e)
        {
            var featuresWindow = new Features();
            featuresWindow.Show();
            this.Close();
        }
    }
}
