using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace InspirationLabProjectStanSeyit
{
    public partial class Planner : Window
    {
        private Dictionary<DateTime, string> notes = new Dictionary<DateTime, string>();

        public Planner()
        {
            InitializeComponent();
            HighlightToday();
            UpdateMonthYearText();
        }

        private void HighlightToday()
        {
            PlannerCalendar.SelectedDate = DateTime.Today;
            PlannerCalendar.DisplayDate = DateTime.Today;
        }

        private void PlannerCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlannerCalendar.SelectedDate.HasValue)
            {
                DateTime selectedDate = PlannerCalendar.SelectedDate.Value.Date;
                if (notes.ContainsKey(selectedDate))
                {
                    SavedNoteText.Text = notes[selectedDate];
                }
                else
                {
                    SavedNoteText.Text = "(No notes for this date yet.)";
                }
            }
        }

        private void SaveNote_Click(object sender, RoutedEventArgs e)
        {
            if (PlannerCalendar.SelectedDate.HasValue)
            {
                DateTime selectedDate = PlannerCalendar.SelectedDate.Value.Date;
                string note = NoteTextBox.Text.Trim();

                if (!string.IsNullOrEmpty(note))
                {
                    notes[selectedDate] = note;
                    SavedNoteText.Text = note;
                    NoteTextBox.Clear();
                    MessageBox.Show("Note saved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a date first.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            PlannerCalendar.DisplayDate = PlannerCalendar.DisplayDate.AddMonths(-1);
            UpdateMonthYearText();
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            PlannerCalendar.DisplayDate = PlannerCalendar.DisplayDate.AddMonths(1);
            UpdateMonthYearText();
        }

        private void PlannerCalendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            UpdateMonthYearText();
        }

        private void UpdateMonthYearText()
        {
            MonthYearText.Text = PlannerCalendar.DisplayDate.ToString("MMMM yyyy");
        }


        private void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            if (PlannerCalendar.SelectedDate.HasValue)
            {
                DateTime selectedDate = PlannerCalendar.SelectedDate.Value.Date;
                if (notes.ContainsKey(selectedDate))
                {
                    notes.Remove(selectedDate);
                    SavedNoteText.Text = "(No notes for this date yet.)";
                    MessageBox.Show("Note deleted!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("There is no note to delete for this date.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a date first.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        
        //image carousel
        
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
        //image carousel end
    }
}