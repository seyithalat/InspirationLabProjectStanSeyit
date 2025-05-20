using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace InspirationLabProjectStanSeyit
{
    public partial class GamePage : Window
    {
        private List<string> gameImages = new List<string>
        {
            "Images/gamescarouselimage.jpg",
            "Images/profilecarouselimage.jpg",
            "Images/plannercarouselimage.jpg"
        };

        private List<string> gameTitles = new List<string>
        {
            "Focus Game",
            "Profile Challenge",
            "Planner Puzzle"
        };

        private int startIndex = 0;

        public GamePage()
        {
            InitializeComponent();
            Loaded += GamePage_Loaded;
        }

        private void GamePage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateImageSet();
        }

        private void UpdateImageSet()
        {
            if (gameImages.Count < 3) return;

            int i1 = startIndex % gameImages.Count;
            int i2 = (startIndex + 1) % gameImages.Count;
            int i3 = (startIndex + 2) % gameImages.Count;

            Image1.Source = new BitmapImage(new Uri(gameImages[i1], UriKind.Relative));
            Image2.Source = new BitmapImage(new Uri(gameImages[i2], UriKind.Relative));
            Image3.Source = new BitmapImage(new Uri(gameImages[i3], UriKind.Relative));

            Label1.Text = gameTitles[i1];
            Label2.Text = gameTitles[i2];
            Label3.Text = gameTitles[i3];
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            startIndex = (startIndex - 1 + gameImages.Count) % gameImages.Count;
            UpdateImageSet();
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            startIndex = (startIndex + 1) % gameImages.Count;
            UpdateImageSet();
        }

        private void Image1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"You clicked on: {Label1.Text}", "Game Selected");
        }

        private void Image2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"You clicked on: {Label2.Text}", "Game Selected");
        }

        private void Image3_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"You clicked on: {Label3.Text}", "Game Selected");
        }
    }
}
