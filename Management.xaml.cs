using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace InspirationLabProjectStanSeyit
{
    public partial class Management : Window
    {
        private List<string> locationImages = new List<string>
        {
            "Images/location1.jpg",
            "Images/location2.jpg",
            "Images/location3.jpg",
            "Images/location4.jpg"
        };

        private List<string> locationNames = new List<string>
        {
            "Main Campus Library",
            "Science Wing",
            "Online Groups",
            "Study Cafe"
        };

        private int currentCarouselIndex = 0;

        public Management()
        {
            InitializeComponent();
            Loaded += Management_Loaded;
        }

        private void Management_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCarousel();
        }

        private void UpdateCarousel()
        {
            if (locationImages.Count < 2) return;

            int index1 = currentCarouselIndex % locationImages.Count;
            int index2 = (currentCarouselIndex + 1) % locationImages.Count;

            try
            {
                CarouselImage1.Source = new BitmapImage(new Uri(locationImages[index1], UriKind.Relative));
                CarouselImage2.Source = new BitmapImage(new Uri(locationImages[index2], UriKind.Relative));

                CarouselLabel1.Text = locationNames[index1];
                CarouselLabel2.Text = locationNames[index2];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading images: {ex.Message}");
            }
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            currentCarouselIndex = (currentCarouselIndex - 1 + locationImages.Count) % locationImages.Count;
            UpdateCarousel();
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            currentCarouselIndex = (currentCarouselIndex + 1) % locationImages.Count;
            UpdateCarousel();
        }

        private void CreateGroup_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("New study group created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SendInvite_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Invitation sent successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CarouselItem1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Selected location: {CarouselLabel1.Text}", "Location Selected");
        }

        private void CarouselItem2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Selected location: {CarouselLabel2.Text}", "Location Selected");
        }
    }
}