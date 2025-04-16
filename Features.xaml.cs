using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace InspirationLabProjectStanSeyit
{
    public partial class Features : Window
    {
        private List<string> featureImages = new List<string>
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
            "Homepage",
            "Features",
            "Profile",
            "Planner",
            "Groups",
            "Focus Games",
            "Notes",
            "Management",
            "Contact"
        };

        private int startIndex = 0;

        public Features()
        {
            InitializeComponent();
            Loaded += Features_Loaded;
        }

        private void Features_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateImageSet();
        }

        private void UpdateImageSet()
        {
            if (featureImages.Count < 3) return;

            int i1 = startIndex % featureImages.Count;
            int i2 = (startIndex + 1) % featureImages.Count;
            int i3 = (startIndex + 2) % featureImages.Count;

            Image1.Source = new BitmapImage(new Uri(featureImages[i1], UriKind.Relative));
            Image2.Source = new BitmapImage(new Uri(featureImages[i2], UriKind.Relative));
            Image3.Source = new BitmapImage(new Uri(featureImages[i3], UriKind.Relative));

            Label1.Text = imageTitles[i1];
            Label2.Text = imageTitles[i2];
            Label3.Text = imageTitles[i3];
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            startIndex = (startIndex - 1 + featureImages.Count) % featureImages.Count;
            UpdateImageSet();
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            startIndex = (startIndex + 1) % featureImages.Count;
            UpdateImageSet();
        }

        private void ImageButton1_Click(object sender, RoutedEventArgs e)
        {
            // Example: Navigate somewhere (back to MainWindow)
            var main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
