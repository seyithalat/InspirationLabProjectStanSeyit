using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Input;

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

        public Management()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            Loaded += Management_Loaded;
            UpdateImageSet();
        }

        private void MapBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Suppress script errors
                var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (fiComWebBrowser == null) return;
                var objComWebBrowser = fiComWebBrowser.GetValue(MapBrowser);
                objComWebBrowser?.GetType().InvokeMember("Silent", System.Reflection.BindingFlags.SetProperty, null, objComWebBrowser, new object[] { true });

                MapBrowser.Navigating += MapBrowser_Navigating;
                MapBrowser.Navigated += MapBrowser_Navigated;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Browser initialization error: {ex.Message}", "Error");
            }
        }

        private void MapBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            try
            {
                MessageBox.Show($"Navigating to: {e.Uri}", "Debug Info");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Navigation error: {ex.Message}", "Error");
            }
        }

        private void MapBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            try
            {
                if (e.Uri != null)
                {
                    MessageBox.Show($"Successfully navigated to: {e.Uri}", "Debug Info");
                }
                else
                {
                    MessageBox.Show("Navigation completed but URI is null", "Warning");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Post-navigation error: {ex.Message}", "Error");
            }
        }

        private void Management_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateCarousel();
                LoadDefaultMap();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during initialization: {ex.Message}", "Initialization Error");
            }
        }

        private void LoadDefaultMap()
        {
            try
            {
                string defaultLocation = "University Library Rotterdam";
                string encodedLocation = Uri.EscapeDataString(defaultLocation);
                string htmlContent = $@"
                    <html>
                    <head>
                        <style>
                            body, html {{ margin: 0; padding: 0; height: 100%; }}
                            iframe {{ width: 100%; height: 100%; border: none; }}
                        </style>
                    </head>
                    <body>
                        <iframe
                            src=""https://www.google.com/maps/embed/v1/place?key={API_KEY}&q={encodedLocation}""
                            allowfullscreen>
                        </iframe>
                    </body>
                    </html>";

                string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "map.html");
                System.IO.File.WriteAllText(tempPath, htmlContent);
                MapBrowser.Navigate(new Uri(tempPath));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading map: {ex.Message}", "Map Error");
            }
        }

        private void UpdateMap(string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location))
                {
                    LoadDefaultMap();
                    return;
                }

                string encodedLocation = Uri.EscapeDataString(location);
                string htmlContent = $@"
                    <html>
                    <head>
                        <style>
                            body, html {{ margin: 0; padding: 0; height: 100%; }}
                            iframe {{ width: 100%; height: 100%; border: none; }}
                        </style>
                    </head>
                    <body>
                        <iframe
                            src=""https://www.google.com/maps/embed/v1/place?key={API_KEY}&q={encodedLocation}""
                            allowfullscreen>
                        </iframe>
                    </body>
                    </html>";

                string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "map.html");
                System.IO.File.WriteAllText(tempPath, htmlContent);
                MapBrowser.Navigate(new Uri(tempPath));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating map: {ex.Message}", "Map Error");
            }
        }

        private void RefreshLocation_Click(object sender, RoutedEventArgs e)
        {
            // Show the selected location on the map
            if (CarouselLabel1 != null && !string.IsNullOrEmpty(CarouselLabel1.Text))
            {
                UpdateMap(CarouselLabel1.Text);
            }
            else
            {
                LoadDefaultMap();
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
            UpdateMap(CarouselLabel1.Text);
        }

        private void CarouselItem2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Selected location: {CarouselLabel2.Text}", "Location Selected");
            UpdateMap(CarouselLabel2.Text);
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
        private void Image1_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Image 1 clicked!");
        }

        private void Image2_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Image 2 clicked!");
        }

        private void Image3_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Image 3 clicked!");
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

        private int startIndex = 0;

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