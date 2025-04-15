using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace InspirationLabProjectStanSeyit
{
    public partial class MainWindow : Window
    {
        // ?? Carousel images
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

        private int startIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            Loaded += Window_Loaded;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Hide();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Hide();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateImageSet();
        }

        private void UpdateImageSet()
        {
            if (imagePaths.Count < 3)
            {
                MessageBox.Show("You need at least 3 images for the carousel to work properly.", "Image Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Image1.Source = new BitmapImage(new Uri(imagePaths[startIndex % imagePaths.Count], UriKind.Relative));
                Image2.Source = new BitmapImage(new Uri(imagePaths[(startIndex + 1) % imagePaths.Count], UriKind.Relative));
                Image3.Source = new BitmapImage(new Uri(imagePaths[(startIndex + 2) % imagePaths.Count], UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load images. Make sure they exist in the Images folder.\n\n" + ex.Message,
                                "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
    }
}
