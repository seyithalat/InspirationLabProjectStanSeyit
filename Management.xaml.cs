using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Data.SqlClient;

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
        //Comment

        private List<string> locationNames = new List<string>
        {
            "Main Campus Library",
            "Science Wing",
            "Online Groups",
            "Study Cafe"
        };

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

        private readonly HttpClient _httpClient;
        private const string API_KEY = "AIzaSyDWRmX1HI6B5-3vxI2f4jVLdmUUPomh3Wc";
        private const string MAPS_API_URL = "https://www.google.com/maps/embed/v1/place";

        private int currentCarouselIndex = 0;
        private int currentNavIndex = 0;

        public Management()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            Loaded += Management_Loaded;
            
            // Only show the admin button if the user is an admin
            if (ReviewSubmissionsButton != null)
            {
                ReviewSubmissionsButton.Visibility = Data.IsUserAdmin(Session.CurrentUserId) 
                    ? Visibility.Visible 
                    : Visibility.Collapsed;
            }
        }

        private string GetStaticMapUrl()
        {
            // Center on Mechelen
            string center = "Mechelen,Belgium";
            int zoom = 14;
            string size = "600x400";
            string apiKey = "AIzaSyDWRmX1HI6B5-3vxI2f4jVLdmUUPomh3Wc";

            // Get approved locations from database using Data class
            var markers = Data.GetApprovedLocations();

            // Add default locations if no approved locations exist
            if (markers.Count == 0)
            {
                markers.Add("Mechelen,Belgium"); // City center
                markers.Add("Hogeschool Thomas More Mechelen");
                markers.Add("Kruidtuin Mechelen");
            }

            string markerString = string.Join("|", markers.ConvertAll(Uri.EscapeDataString));
            string url = $"https://maps.googleapis.com/maps/api/staticmap?center={Uri.EscapeDataString(center)}&zoom={zoom}&size={size}&markers={markerString}&key={apiKey}";
            return url;
        }

        private void Management_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateCarousel();
                UpdateNavCarousel();
                string mapUrl = GetStaticMapUrl();
                StudyMapImage.Source = new BitmapImage(new Uri(mapUrl));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during initialization: {ex.Message}", "Initialization Error");
            }
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

        private void ReviewSubmissions_Click(object sender, RoutedEventArgs e)
        {
            var adminWindow = new LocationSubmissionsAdmin();
            adminWindow.Show();
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
    }
}