using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace InspirationLabProjectStanSeyit
{
    public partial class Features : Window
    {
        public Features()
        {
            InitializeComponent();
        }

        private void StudyGroupsButton_Click(object sender, RoutedEventArgs e)
        {
            var studyGroupsPage = new StudyGroups();
            studyGroupsPage.Show();
            this.Hide(); // Hide the Features window
        }

        private void FriendsButton_Click(object sender, RoutedEventArgs e)
        {
            var friendsPage = new Friends();
            friendsPage.Show();
            this.Hide(); // Hide the Features window
        }

        private void MapButton_Click(object sender, RoutedEventArgs e)
        {
            var mapPage = new Map();
            mapPage.Show();
            this.Hide(); // Hide the Features window
        }

        private void BackToSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings();
            settingsWindow.Show();
            this.Close();
        }

        private void OpenMap_Click(object sender, RoutedEventArgs e)
        {
            var mapWindow = new Map();
            mapWindow.Show();
            this.Close();
        }
    }
}