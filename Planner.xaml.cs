using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement previous image if needed
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement next image if needed
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

    }
}