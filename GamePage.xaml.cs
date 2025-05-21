using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using InspirationLabProjectStanSeyit.Games;

namespace InspirationLabProjectStanSeyit
{
    public partial class GamePage : Window
    {
        private List<string> navImages = new List<string>
        {
            "Images/featurescarouselimage.jpg",
            "Images/profilecarouselimage.jpg",
            "Images/plannercarouselimage.jpg",
            "Images/groupscarouselimage.jpg",
            "Images/gamescarouselimage.jpg",
            "Images/notescarouselimage.jpg",
            "Images/managementcarouselimage.jpg",
            "Images/contactcarouselimage.jpg"
        };

        private List<string> navTitles = new List<string>
        {
            "Features",
            "Profile",
            "Planner",
            "Groups",
            "Games",
            "Notes",
            "Management",
            "Contact"
        };

        private int currentNavIndex = 0;

        public GamePage()
        {
            InitializeComponent();
            Loaded += GamePage_Loaded;
        }

        private void GamePage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateNavCarousel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during initialization: {ex.Message}", "Initialization Error");
            }
        }

        private void UpdateNavCarousel()
        {
            if (navImages.Count < 3) return;

            int i1 = currentNavIndex % navImages.Count;
            int i2 = (currentNavIndex + 1) % navImages.Count;
            int i3 = (currentNavIndex + 2) % navImages.Count;

            try
            {
                NavImage1.Source = new BitmapImage(new Uri(navImages[i1], UriKind.Relative));
                NavImage2.Source = new BitmapImage(new Uri(navImages[i2], UriKind.Relative));
                NavImage3.Source = new BitmapImage(new Uri(navImages[i3], UriKind.Relative));

                NavLabel1.Text = navTitles[i1];
                NavLabel2.Text = navTitles[i2];
                NavLabel3.Text = navTitles[i3];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading navigation images: {ex.Message}");
            }
        }

        private void NavPrevImage_Click(object sender, RoutedEventArgs e)
        {
            currentNavIndex = (currentNavIndex - 1 + navImages.Count) % navImages.Count;
            UpdateNavCarousel();
        }

        private void NavNextImage_Click(object sender, RoutedEventArgs e)
        {
            currentNavIndex = (currentNavIndex + 1) % navImages.Count;
            UpdateNavCarousel();
        }

        private void NavImage1_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(NavLabel1.Text);
        }

        private void NavImage2_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(NavLabel2.Text);
        }

        private void NavImage3_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(NavLabel3.Text);
        }

        private void NavigateToPage(string pageName)
        {
            Window newWindow = null;
            switch (pageName.ToLower())
            {
                case "features":
                    newWindow = new Features();
                    break;
                case "profile":
                    newWindow = new Profile();
                    break;
                case "planner":
                    newWindow = new Planner();
                    break;
                case "groups":
                    newWindow = new StudyGroups();
                    break;
                case "games":
                    newWindow = new GamePage();
                    break;
                case "notes":
                    newWindow = new StudyMaterial();
                    break;
                case "management":
                    newWindow = new Management();
                    break;
                case "contact":
                    newWindow = new Contact();
                    break;
            }

            if (newWindow != null)
            {
                newWindow.Show();
                this.Close();
            }
        }

        private void PlayTrivia_Click(object sender, RoutedEventArgs e)
        {
            var triviaGame = new TriviaGame();
            triviaGame.Show();
        }

        private void PlayMath_Click(object sender, RoutedEventArgs e)
        {
            var mathGame = new MathGame();
            mathGame.Show();
        }

        private void PlayWordScramble_Click(object sender, RoutedEventArgs e)
        {
            var wordScrambleGame = new WordScrambleGame();
            wordScrambleGame.Show();
        }
    }
}
