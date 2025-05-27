using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InspirationLabProjectStanSeyit
{
    
    public class MessageBackgroundConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isSender = (bool)value;
            return isSender ? new SolidColorBrush(Color.FromRgb(220, 248, 198)) : new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MessageAlignmentConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isSender = (bool)value;
            return isSender ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class Chat : Window
    {
        private int _otherUserId;
        private string _otherUsername;
        private ObservableCollection<ChatMessage> _messages;
        private System.Windows.Threading.DispatcherTimer _refreshTimer;

        public Chat(int otherUserId, string otherUsername)
        {
            InitializeComponent();
            _otherUserId = otherUserId;
            _otherUsername = otherUsername;
            _messages = new ObservableCollection<ChatMessage>();

            // Set up the window
            ChatHeader.Text = $"Chat with {_otherUsername}";
            MessagesList.ItemsSource = _messages;

            // Load chat history
            LoadChatHistory();

            // Set up auto-refresh timer
            _refreshTimer = new System.Windows.Threading.DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(2);
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        private void LoadChatHistory()
        {
            var messages = Data.GetChatHistory(Session.CurrentUserId, _otherUserId);
            _messages.Clear();
            foreach (var message in messages)
            {
                _messages.Add(message);
            }
            ScrollToBottom();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadChatHistory();
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SendMessage();
                e.Handled = true;
            }
        }

        private void SendMessage()
        {
            string content = MessageInput.Text.Trim();
            if (!string.IsNullOrEmpty(content))
            {
                if (Data.SendMessage(Session.CurrentUserId, _otherUserId, content))
                {
                    MessageInput.Clear();
                    LoadChatHistory();
                }
                else
                {
                    MessageBox.Show("Failed to send message. Please try again.");
                }
            }
        }

        private void ScrollToBottom()
        {
            if (MessagesList.Items.Count > 0)
            {
                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _refreshTimer.Stop();
            base.OnClosed(e);
        }

    }
}