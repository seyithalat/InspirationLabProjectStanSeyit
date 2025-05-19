using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace InspirationLabProjectStanSeyit
{
    public partial class Features : Window
    {


        private int startIndex = 0;

        public Features()
        {
            InitializeComponent();
            Loaded += Features_Loaded;
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
        private void InviteEmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // If textbox is empty, show placeholder, otherwise hide it
            if (string.IsNullOrWhiteSpace(InviteEmailTextBox.Text))
            {
                PlaceholderText.Visibility = Visibility.Visible;
            }
            else
            {
                PlaceholderText.Visibility = Visibility.Collapsed;
            }
        }

        private void SendInvite_Click(object sender, RoutedEventArgs e)
        {
            string email = InviteEmailTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(email))
            {
                MessageBox.Show($"Invitation sent to: {email}", "Invite Sent", MessageBoxButton.OK, MessageBoxImage.Information);
                InviteEmailTextBox.Text = string.Empty; // Clear the textbox
            }
            else
            {
                MessageBox.Show("Please enter a valid email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CharacterButton_Click(object sender, RoutedEventArgs e)
        {
            var characterPage = new Character();
            characterPage.Show();
            this.Hide(); // Hide the Features window
        }



        private void Features_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateImageSet();
        }



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
