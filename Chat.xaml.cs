using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InspirationLabProjectStanSeyit
{
    // Value converter for message background colors.
    // Returns a light green background for sent messages and white for received messages.
   
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

   
    // Value converter for message alignment.
    // Aligns sent messages to the right and received messages to the left.
    
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

    //Chat window that handles real-time messaging between two users.
    //Features include:
    //Real-time message updates (with a 2-second refresh interval)
    //Message history display
    //Send messages with Enter+Ctrl or Send button
    //Visual distinction between sent and received messages

    public partial class Chat : Window
    {
        // Store the other user's information
        private int _otherUserId;
        private string _otherUsername;
        
        // Collection of messages for the chat window
        private ObservableCollection<ChatMessage> _messages;
        
        // Timer for auto-refreshing messages
        private System.Windows.Threading.DispatcherTimer _refreshTimer;

  
        // Initializes a new chat window with the specified user.
        
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

            // Set up auto-refresh timer (refreshes every 2 seconds)
            _refreshTimer = new System.Windows.Threading.DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(2);
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        //Loads the chat history between the current user and the other user.

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


        //Timer tick event handler that refreshes the chat messages.

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadChatHistory();
        }

        //Handles the Send button click event.
        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        //Handles the Enter+Ctrl key combination for sending messages.
        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SendMessage();
                e.Handled = true;
            }
        }

        //Sends a message to the other user.
        //Clears the input field and refreshes the chat history on success.
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

        //Scrolls the message list to the bottom to show the most recent messages.
        private void ScrollToBottom()
        {
            if (MessagesList.Items.Count > 0)
            {
                MessagesList.ScrollIntoView(MessagesList.Items[MessagesList.Items.Count - 1]);
            }
        }

        //Cleans up resources when the chat window is closed.
        //Stops the refresh timer to prevent memory leaks.
        protected override void OnClosed(EventArgs e)
        {
            _refreshTimer.Stop();
            base.OnClosed(e);
        }
    }
}