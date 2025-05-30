using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using InspirationLabProjectStanSeyit.Games;
using InspirationLabProjectStanSeyit.Models;
using System.Linq;

namespace InspirationLabProjectStanSeyit
{
    public partial class GamePage : Window
    {

        

        public GamePage()
        {
            InitializeComponent();
            UpdateImageSet();
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
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


        private int currentNavIndex = 0;

      

        private void GamePage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateNavCarousel();
                LoadLeaderboards();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during initialization: {ex.Message}", "Initialization Error");
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

        public GamePage()
        {
            InitializeComponent();
            Loaded += GamePage_Loaded;
        }
        private void NavigateToPage(string pageName)
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

        private void EndGame()
        {
            // If you have a score variable here, use it. Otherwise, remove or update this method as needed.
            // Data.SaveGameScore(Session.CurrentUserId, "MemoryGame", score, DateTime.Now);
            // Show game over UI, etc.
        }

        private void LoadLeaderboards()
        {
            var mathScores = Data.GetTopGameScores("Math", 5);
            var triviaScores = Data.GetTopGameScores("Trivia", 5);
            var wordScrambleScores = Data.GetTopGameScores("WordScramble", 5);

            MathLeaderboard.ItemsSource = mathScores.Select((s, i) => $"{i + 1}. {s.Username} - {s.Score} pts").ToList();
            TriviaLeaderboard.ItemsSource = triviaScores.Select((s, i) => $"{i + 1}. {s.Username} - {s.Score} pts").ToList();
            WordScrambleLeaderboard.ItemsSource = wordScrambleScores.Select((s, i) => $"{i + 1}. {s.Username} - {s.Score} pts").ToList();
        }
    }
}
