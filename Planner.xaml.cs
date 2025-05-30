using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Media;

namespace InspirationLabProjectStanSeyit
{
    

    public class Task
    {
        public string Title { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }

    public partial class Planner : Window
    {
        private bool isWeekView = false;
        private Grid weekViewGrid;

        public Planner()
        {
            InitializeComponent();
            HighlightToday();
            UpdateMonthYearText();
            InitializeComboBoxes();
            CreateWeekViewGrid();
            LoadTasksForSelectedDate();
        }

        private void InitializeComboBoxes()
        {
            PriorityComboBox.SelectedIndex = 1; // Default to Medium Priority
            CategoryComboBox.SelectedIndex = 0; // Default to Study
        }

        private void CreateWeekViewGrid()
        {
            weekViewGrid = new Grid();
            weekViewGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) }); // Header row
            weekViewGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Content row

            // Add day headers
            string[] days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            for (int i = 0; i < 7; i++)
            {
                weekViewGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                
                // Add day header
                TextBlock dayHeader = new TextBlock
                {
                    Text = days[i],
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#61be63"))
                };
                Grid.SetRow(dayHeader, 0);
                Grid.SetColumn(dayHeader, i);
                weekViewGrid.Children.Add(dayHeader);

                // Add day content panel
                StackPanel dayPanel = new StackPanel
                {
                    Background = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(1)
                };
                Grid.SetRow(dayPanel, 1);
                Grid.SetColumn(dayPanel, i);
                weekViewGrid.Children.Add(dayPanel);
            }
        }

        private void HighlightToday()
        {
            PlannerCalendar.SelectedDate = DateTime.Today;
            PlannerCalendar.DisplayDate = DateTime.Today;
        }

        private void LoadTasksForSelectedDate()
        {
            if (PlannerCalendar.SelectedDate.HasValue)
            {
                var date = PlannerCalendar.SelectedDate.Value.Date;
                var tasks = Data.GetTasksForUser(Session.CurrentUserId, date);
                TaskListView.ItemsSource = tasks;
                // Update header
                if (date == DateTime.Today)
                {
                    TaskListHeader.Text = "Today's Tasks";
                }
                else
                {
                    TaskListHeader.Text = $"Tasks for {date:dddd, MMMM d}";
                }
            }
            else
            {
                TaskListView.ItemsSource = new List<Data.Task>();
                TaskListHeader.Text = "Tasks";
            }
        }

        private void PlannerCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlannerCalendar.SelectedDate.HasValue)
            {
                LoadTasksForSelectedDate();
                if (isWeekView)
                {
                    UpdateWeekView(PlannerCalendar.SelectedDate.Value.Date);
                }
            }
        }

        private void UpdateWeekView(DateTime selectedDate)
        {
            // Clear existing content
            for (int i = 1; i < weekViewGrid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < weekViewGrid.ColumnDefinitions.Count; j++)
                {
                    var panel = weekViewGrid.Children.OfType<StackPanel>()
                        .FirstOrDefault(p => Grid.GetRow(p) == i && Grid.GetColumn(p) == j);
                    if (panel != null)
                    {
                        panel.Children.Clear();
                    }
                }
            }

            // Get the start of the week
            DateTime weekStart = selectedDate.AddDays(-(int)selectedDate.DayOfWeek);

            // Update each day panel
            for (int i = 0; i < 7; i++)
            {
                DateTime currentDate = weekStart.AddDays(i);
                var dayPanel = weekViewGrid.Children.OfType<StackPanel>()
                    .FirstOrDefault(p => Grid.GetRow(p) == 1 && Grid.GetColumn(p) == i);

                if (dayPanel != null)
                {
                    // Add date header
                    TextBlock dateHeader = new TextBlock
                    {
                        Text = currentDate.ToString("MMM d"),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 5, 0, 5),
                        FontWeight = FontWeights.Bold
                    };
                    dayPanel.Children.Add(dateHeader);

                    // Add tasks for this day
                    var dayTasks = Data.GetTasksForUser(Session.CurrentUserId, currentDate);
                    foreach (var task in dayTasks)
                    {
                        Border taskBorder = new Border
                        {
                            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f8f9fa")),
                            CornerRadius = new CornerRadius(5),
                            Margin = new Thickness(5),
                            Padding = new Thickness(5)
                        };

                        StackPanel taskPanel = new StackPanel();
                        taskPanel.Children.Add(new TextBlock
                        {
                            Text = task.Title,
                            TextWrapping = TextWrapping.Wrap
                        });
                        taskPanel.Children.Add(new TextBlock
                        {
                            Text = task.Priority,
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#61be63")),
                            FontSize = 10
                        });

                        taskBorder.Child = taskPanel;
                        dayPanel.Children.Add(taskBorder);
                    }
                }
            }
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (PlannerCalendar.SelectedDate.HasValue)
            {
                DateTime selectedDate = PlannerCalendar.SelectedDate.Value.Date;
                string title = TaskTitleTextBox.Text.Trim();
                string priority = (PriorityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Medium Priority";
                string category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Study";

                if (!string.IsNullOrEmpty(title))
                {
                    Data.AddTask(Session.CurrentUserId, title, priority, category, selectedDate);
                    TaskTitleTextBox.Clear();
                    LoadTasksForSelectedDate();
                    if (isWeekView)
                    {
                        UpdateWeekView(selectedDate);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a date first.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TaskCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is Data.Task task)
            {
                Data.UpdateTaskCompletion(task.Id, checkBox.IsChecked ?? false);
                LoadTasksForSelectedDate();
                if (isWeekView)
                {
                    UpdateWeekView(task.DueDate);
                }
            }
        }

        private void MonthView_Click(object sender, RoutedEventArgs e)
        {
            isWeekView = false;
            UpdateCalendarView();
        }

        private void WeekView_Click(object sender, RoutedEventArgs e)
        {
            isWeekView = true;
            UpdateCalendarView();
        }

        private void UpdateCalendarView()
        {
            // Always show the calendar
            PlannerCalendar.Visibility = Visibility.Visible;
            if (isWeekView)
            {
                WeekViewHost.Visibility = Visibility.Visible;
                WeekViewHost.Content = weekViewGrid;
                if (PlannerCalendar.SelectedDate.HasValue)
                {
                    UpdateWeekView(PlannerCalendar.SelectedDate.Value);
                }
            }
            else
            {
                WeekViewHost.Visibility = Visibility.Collapsed;
                WeekViewHost.Content = null;
            }
        }

        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            if (isWeekView)
            {
                PlannerCalendar.DisplayDate = PlannerCalendar.DisplayDate.AddDays(-7);
            }
            else
            {
                PlannerCalendar.DisplayDate = PlannerCalendar.DisplayDate.AddMonths(-1);
            }
            UpdateMonthYearText();
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            if (isWeekView)
            {
                PlannerCalendar.DisplayDate = PlannerCalendar.DisplayDate.AddDays(7);
            }
            else
            {
                PlannerCalendar.DisplayDate = PlannerCalendar.DisplayDate.AddMonths(1);
            }
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

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void OpenMap_Click(object sender, RoutedEventArgs e)
        {
            var managementWindow = new Management();
            managementWindow.Show();
            this.Close();
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int taskId)
            {
                if (MessageBox.Show("Delete this task?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Data.DeleteTask(taskId);
                    LoadTasksForSelectedDate();
                }
            }
        }
    }
}