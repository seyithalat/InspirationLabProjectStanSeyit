using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InspirationLabProjectStanSeyit
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        public Profile()
        {
            InitializeComponent();
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Profile changes saved! (Implement MySQL save logic here)", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StudyGroups_Click(object sender, RoutedEventArgs e)
        {
            var studyGroups = new StudyGroups();
            studyGroups.Show();
            this.Close();
        }

        private void Notes_Click(object sender, RoutedEventArgs e)
        {
            var studyMaterial = new StudyMaterial();
            studyMaterial.Show();
            this.Close();
        }

        private void BackToSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings();
            settingsWindow.Show();
            this.Close();
        }

        private void OpenMap_Click(object sender, RoutedEventArgs e)
        {
            var managementWindow = new Management();
            managementWindow.Show();
            this.Close();
        }
    }
}
