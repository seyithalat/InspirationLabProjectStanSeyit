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
        private readonly List<string> imagePaths = new List<string>
        {
            "Images/homepagecarouselimage.jpg",
            "Images/featurescarouselimage.jpg",
            "Images/profilecarouselimage.jpg",
            "Images/plannercarouselimage.jpg",
            "Images/groupscarouselimage.jpg",
            "Images/gamescarouselimage.jpg",
            "Images/notescarouselimage.jpg",
            "Images/managementcarouselimage.jpg",
            "Images/contactcarouselimage.jpg"
        };

        private List<string> imageTitles = new List<string>
        {
            "Features",
            "Profile",
            "Planner",
            "Groups",
            "Focus Games",
            "Notes",
            "Management",
            "Contact",
            "Settings"
        };

        private int startIndex = 0;

        public Settings()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateImageSet();
        }

        private void DarkModeToggle_Checked(object sender, RoutedEventArgs e)
        {
            var darkResource = new ResourceDictionary
            {
                Source = new Uri("DarkTheme.xaml", UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(darkResource);
        }

        private void DarkModeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            var lightResource = new ResourceDictionary
            {
                Source = new Uri("LightTheme.xaml", UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(lightResource);
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete your account? This action cannot be undone.",
                                      "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                // TODO: Call backend or service to delete account
                MessageBox.Show("Your account has been deleted.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        public void UpdateImageSet()
        {
            if (imagePaths.Count < 3) return;

            int i1 = startIndex % imagePaths.Count;
            int i2 = (startIndex + 1) % imagePaths.Count;
            int i3 = (startIndex + 2) % imagePaths.Count;

            Image1.Source = new BitmapImage(new Uri(imagePaths[i1], UriKind.Relative));
            Image2.Source = new BitmapImage(new Uri(imagePaths[i2], UriKind.Relative));
            Image3.Source = new BitmapImage(new Uri(imagePaths[i3], UriKind.Relative));

            Label1.Text = imageTitles[i1];
            Label2.Text = imageTitles[i2];
            Label3.Text = imageTitles[i3];
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            startIndex = (startIndex - 1 + imagePaths.Count) % imagePaths.Count;
            UpdateImageSet();
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            startIndex = (startIndex + 1) % imagePaths.Count;
            UpdateImageSet();
        }

        private void Image1_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(startIndex % imagePaths.Count);
        }

        private void Image2_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage((startIndex + 1) % imagePaths.Count);
        }

        private void Image3_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage((startIndex + 2) % imagePaths.Count);
        }

        private void NavigateToPage(int index)
        {
            Window newWindow = null;

            switch (index)
            {
                case 0: // Features
                    newWindow = new Features();
                    break;
                case 1: // Profile
                    newWindow = new Profile();
                    break;
                case 2: // Planner
                    newWindow = new Planner();
                    break;
                case 3: // Groups
                    newWindow = new StudyGroups();
                    break;
                case 4: // Focus Games
                    newWindow = new GamePage();
                    break;
                case 5: // Notes
                    newWindow = new StudyMaterial();
                    break;
                case 6: // Management
                    newWindow = new Management();
                    break;
                case 7: // Contact
                    newWindow = new Contact();
                    break;
                case 8: // Settings
                    newWindow = new Settings();
                    break;
                default:
                    newWindow = new Features();
                    break;
            }

            if (newWindow != null)
            {
                newWindow.Show();
                this.Close();
            }
        }
    }
}
