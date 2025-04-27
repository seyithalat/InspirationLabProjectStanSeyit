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

        private void Image1_Click(object sender, RoutedEventArgs e)
        {
            var featuresWindow = new Features();
            featuresWindow.Show();
            this.Close();
        }

        private void Image2_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new Profile();
            profileWindow.Show();
            this.Close();
        }

        private void Image3_Click(object sender, RoutedEventArgs e)
        {
            var plannerWindow = new Planner();
            plannerWindow.Show();
            this.Close();
        }


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
    }
}
