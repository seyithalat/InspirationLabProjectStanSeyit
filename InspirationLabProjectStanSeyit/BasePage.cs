using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace InspirationLabProjectStanSeyit
{
    public abstract class BasePage : Window
    {
        protected List<string> featureImages = new List<string>
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

        protected List<string> imageTitles = new List<string>
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

        protected int startIndex = 0;

        protected void UpdateImageSet(Image image1, Image image2, TextBlock label1, TextBlock label2)
        {
            if (featureImages.Count < 2) return;

            int i1 = startIndex % featureImages.Count;
            int i2 = (startIndex + 1) % featureImages.Count;

            image1.Source = new BitmapImage(new Uri(featureImages[i1], UriKind.Relative));
            image2.Source = new BitmapImage(new Uri(featureImages[i2], UriKind.Relative));

            label1.Text = imageTitles[i1];
            label2.Text = imageTitles[i2];
        }

        protected void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            startIndex = (startIndex - 1 + featureImages.Count) % featureImages.Count;
            UpdateImageSet(CarouselImage1, CarouselImage2, CarouselLabel1, CarouselLabel2);
        }

        protected void NextImage_Click(object sender, RoutedEventArgs e)
        {
            startIndex = (startIndex + 1) % featureImages.Count;
            UpdateImageSet(CarouselImage1, CarouselImage2, CarouselLabel1, CarouselLabel2);
        }

        protected void CarouselItem1_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(startIndex % featureImages.Count);
        }

        protected void CarouselItem2_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage((startIndex + 1) % featureImages.Count);
        }

        protected void NavigateToPage(int index)
        {
            Window newWindow = null;
            switch (index)
            {
                case 0: // Homepage
                    newWindow = new MainWindow();
                    break;
                case 1: // Features
                    newWindow = new Features();
                    break;
                case 2: // Profile
                    newWindow = new Profile();
                    break;
                case 3: // Planner
                    newWindow = new Planner();
                    break;
                case 4: // Groups
                    newWindow = new StudyGroups();
                    break;
                case 5: // Focus Games
                    newWindow = new GamePage();
                    break;
                case 6: // Notes
                    newWindow = new StudyMaterial();
                    break;
                case 7: // Management
                    newWindow = new Management();
                    break;
                case 8: // Contact
                    newWindow = new Contact();
                    break;
            }

            if (newWindow != null)
            {
                newWindow.Show();
                this.Close();
            }
        }

        // These properties must be implemented by derived classes
        public abstract Image CarouselImage1 { get; }
        public abstract Image CarouselImage2 { get; }
        public abstract TextBlock CarouselLabel1 { get; }
        public abstract TextBlock CarouselLabel2 { get; }
    }
}
