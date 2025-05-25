using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InspirationLabProjectStanSeyit
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : Window
    {
        public Map()
        {
            InitializeComponent();
            string mapUrl = GetStaticMapUrl();
            StudyMapImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(mapUrl));
        }

        private string GetStaticMapUrl()
        {
            // Center on Mechelen
            string center = "Mechelen,Belgium";
            int zoom = 14;
            string size = "600x400";
            string apiKey = "AIzaSyDWRmX1HI6B5-3vxI2f4jVLdmUUPomh3Wc";

            // Add your study locations as markers (address or lat,lng)
            var markers = new List<string>
            {
                "Mechelen,Belgium", // City center
                "Hogeschool Thomas More Mechelen",
                "Kruidtuin Mechelen"
                // Add more locations as needed
            };

            string markerString = string.Join("|", markers.ConvertAll(Uri.EscapeDataString));
            string url = $"https://maps.googleapis.com/maps/api/staticmap?center={Uri.EscapeDataString(center)}&zoom={zoom}&size={size}&markers={markerString}&key={apiKey}";
            return url;
        }
    }
}
