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

namespace InspirationLabProjectStanSeyit
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void DarkModeToggle_Checked(object sender, RoutedEventArgs e)
        {
            // Apply dark theme
            var darkTheme = new ResourceDictionary();
            darkTheme.Source = new Uri("Darktheme.xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(darkTheme);
            foreach (Window window in Application.Current.Windows)
            {
                window.Resources.MergedDictionaries.Clear();
                window.Resources.MergedDictionaries.Add(darkTheme);
            }
        }

        private void DarkModeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            // Apply light theme
            var lightTheme = new ResourceDictionary();
            lightTheme.Source = new Uri("lighttheme.xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(lightTheme);
            foreach (Window window in Application.Current.Windows)
            {
                window.Resources.MergedDictionaries.Clear();
                window.Resources.MergedDictionaries.Add(lightTheme);
            }
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete your account? This action cannot be undone.",
                                         "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                Data.DeleteUser(Session.CurrentUserId);
                MessageBox.Show("Your account has been deleted.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                Session.CurrentUserId = 0;
                Session.CurrentUsername = null;
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
