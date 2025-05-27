using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using InspirationLabProjectStanSeyit.Models;

namespace InspirationLabProjectStanSeyit
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        
        // UI Properties
        public string SenderName { get; set; }
        public bool IsSender { get; set; }
    }

    public static class Data
    {
        private static string connectionString = "Server=localhost;Port=3307;Database=inspirationlabdb;Uid=root;Pwd=;";
        public static bool UserExists(string input)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // Use the correct column names from your users table:
                string query = "SELECT id FROM users WHERE Username = @input OR Email = @input OR FirstName = @input OR LastName = @input LIMIT 1";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@input", input);
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.Read(); // true if user exists
                    }
                }
            }
        }
        public static void AddLocationSubmission(string name, string description, string address, int submittedById)
        {
            string connectionString = "Server=localhost;Port=3307;Database=inspirationlabdb;Uid=root;Pwd=;";

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO locationsubmissions (Name, Description, Address, Status, SubmittedAt, SubmittedById)
                         VALUES (@Name, @Description, @Address, @Status, @SubmittedAt, @SubmittedById)";
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Description", description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@Status", "Pending");
                    cmd.Parameters.AddWithValue("@SubmittedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@SubmittedById", submittedById);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static bool TestConnection()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    conn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                // Optionally log or display the error
                Console.WriteLine("Connection failed: " + ex.Message);
                return false;
            }
        }
        public static List<string> GetPendingFriendRequests(int userId)
        {
            var pendingRequests = new List<string>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // Get usernames of users who sent a pending friend request to this user
                string query = @"
                SELECT u.Username
                FROM friends f
                JOIN users u ON f.user_id = u.Id
                WHERE f.friend_id = @userId AND f.status = 'Pending'";
                
                Console.WriteLine($"Executing query for user ID: {userId}"); // Debug log
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string username = reader.GetString("Username");
                            Console.WriteLine($"Found pending request from: {username}"); // Debug log
                            pendingRequests.Add(username);
                        }
                    }
                }
            }
            Console.WriteLine($"Total pending requests found: {pendingRequests.Count}"); // Debug log
            return pendingRequests;
        }
        public static bool SendFriendRequest(int userId, string input)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // Get the friend_id from the users table
                string getUserIdQuery = "SELECT Id FROM users WHERE Username = @input OR Email = @input OR FirstName = @input OR LastName = @input LIMIT 1";
                int friendId = -1;
                using (var cmd = new MySqlCommand(getUserIdQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@input", input);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            friendId = reader.GetInt32("Id");
                        }
                        else
                        {
                            Console.WriteLine("User not found for friend request.");
                            return false; // User not found
                        }
                    }
                }

                // Prevent sending a request to yourself
                if (friendId == userId)
                {
                    Console.WriteLine("Cannot send friend request to yourself.");
                    return false;
                }

                // Check if a request already exists
                string checkQuery = "SELECT id FROM friends WHERE user_id = @user_id AND friend_id = @friend_id";
                using (var cmd = new MySqlCommand(checkQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    cmd.Parameters.AddWithValue("@friend_id", friendId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine("Friend request already exists.");
                            return false; // Request already exists
                        }
                    }
                }

                // Insert the friend request
                Console.WriteLine($"user_id: {userId}, friend_id: {friendId}");
                string insertQuery = "INSERT INTO friends (user_id, friend_id, status, created_at, updated_at) VALUES (@user_id, @friend_id, 'Pending', NOW(), NOW())";
                using (var cmd = new MySqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    cmd.Parameters.AddWithValue("@friend_id", friendId);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
        }

        public static bool AcceptFriendRequest(int userId, string friendUsername)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // First get the friend's ID
                string getFriendIdQuery = "SELECT Id FROM users WHERE Username = @username LIMIT 1";
                int friendId = -1;
                using (var cmd = new MySqlCommand(getFriendIdQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@username", friendUsername);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            friendId = reader.GetInt32("Id");
                        }
                        else
                        {
                            return false; // Friend not found
                        }
                    }
                }

                // Update the friend request status to Accepted
                string updateQuery = @"
                    UPDATE friends 
                    SET status = 'Accepted', updated_at = NOW() 
                    WHERE user_id = @friend_id AND friend_id = @user_id AND status = 'Pending'";
                
                using (var cmd = new MySqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@friend_id", friendId);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool DeclineFriendRequest(int userId, string friendUsername)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // First get the friend's ID
                string getFriendIdQuery = "SELECT Id FROM users WHERE Username = @username LIMIT 1";
                int friendId = -1;
                using (var cmd = new MySqlCommand(getFriendIdQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@username", friendUsername);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            friendId = reader.GetInt32("Id");
                        }
                        else
                        {
                            return false; // Friend not found
                        }
                    }
                }

                // Delete the friend request
                string deleteQuery = @"
                    DELETE FROM friends 
                    WHERE user_id = @friend_id AND friend_id = @user_id AND status = 'Pending'";
                
                using (var cmd = new MySqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@friend_id", friendId);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static List<string> GetFriends(int userId)
        {
            var friends = new List<string>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // Get usernames of accepted friends
                string query = @"
                    SELECT u.Username
                    FROM friends f
                    JOIN users u ON (f.user_id = u.Id OR f.friend_id = u.Id)
                    WHERE (f.user_id = @userId OR f.friend_id = @userId)
                    AND f.status = 'Accepted'
                    AND u.Id != @userId";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            friends.Add(reader.GetString("Username"));
                        }
                    }
                }
            }
            return friends;
        }

        public static bool DeleteFriend(int userId, string friendUsername)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // First get the friend's ID
                string getFriendIdQuery = "SELECT Id FROM users WHERE Username = @username LIMIT 1";
                int friendId = -1;
                using (var cmd = new MySqlCommand(getFriendIdQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@username", friendUsername);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            friendId = reader.GetInt32("Id");
                        }
                        else
                        {
                            return false; // Friend not found
                        }
                    }
                }

                // Delete the friend relationship (in both directions)
                string deleteQuery = @"
                    DELETE FROM friends 
                    WHERE (user_id = @user_id AND friend_id = @friend_id)
                    OR (user_id = @friend_id AND friend_id = @user_id)";
                
                using (var cmd = new MySqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    cmd.Parameters.AddWithValue("@friend_id", friendId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool SendMessage(int senderId, int receiverId, string content)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    INSERT INTO messages (sender_id, receiver_id, content, sent_at, is_read)
                    VALUES (@sender_id, @receiver_id, @content, NOW(), false)";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@sender_id", senderId);
                    cmd.Parameters.AddWithValue("@receiver_id", receiverId);
                    cmd.Parameters.AddWithValue("@content", content);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
        }

        public static List<ChatMessage> GetChatHistory(int userId, int otherUserId)
        {
            var messages = new List<ChatMessage>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT m.*, 
                           s.Username as SenderName
                    FROM messages m
                    JOIN users s ON m.sender_id = s.Id
                    WHERE (m.sender_id = @user_id AND m.receiver_id = @other_user_id)
                    OR (m.sender_id = @other_user_id AND m.receiver_id = @user_id)
                    ORDER BY m.sent_at ASC";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    cmd.Parameters.AddWithValue("@other_user_id", otherUserId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var message = new ChatMessage
                            {
                                Id = reader.GetInt32("Id"),
                                SenderId = reader.GetInt32("sender_id"),
                                ReceiverId = reader.GetInt32("receiver_id"),
                                Content = reader.GetString("content"),
                                Timestamp = reader.GetDateTime("sent_at"),
                                IsRead = reader.GetBoolean("is_read"),
                                SenderName = reader.GetString("SenderName"),
                                IsSender = reader.GetInt32("sender_id") == userId
                            };
                            messages.Add(message);
                        }
                    }
                }

                // Mark messages as read
                if (messages.Any())
                {
                    string updateQuery = @"
                        UPDATE messages 
                        SET is_read = true 
                        WHERE receiver_id = @user_id 
                        AND sender_id = @other_user_id 
                        AND is_read = false";
                    
                    using (var cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.Parameters.AddWithValue("@other_user_id", otherUserId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return messages;
        }
        public static bool AddNote(Note note)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO notes (UserId, Title, Content, FilePath, CreatedAt, UpdatedAt)
                         VALUES (@UserId, @Title, @Content, @FilePath, NOW(), NOW())";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", note.UserId);
                    cmd.Parameters.AddWithValue("@Title", note.Title);
                    cmd.Parameters.AddWithValue("@Content", note.Content ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FilePath", note.FilePath ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
        }
        public static bool DeleteNote(int noteId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM notes WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", noteId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static List<Note> GetNotesForUser(int userId)
        {
            var notes = new List<Note>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM notes WHERE UserId = @UserId ORDER BY CreatedAt DESC";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notes.Add(new Note
                            {
                                Id = reader.GetInt32("Id"),
                                UserId = reader.GetInt32("UserId"),
                                Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString("Title"),
                                Content = reader.IsDBNull(reader.GetOrdinal("Content")) ? null : (byte[])reader["Content"],
                                FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString("FilePath"),
                                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.MinValue : reader.GetDateTime("CreatedAt"),
                                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime("UpdatedAt")
                            });
                        }
                    }
                }
            }
            return notes;
        }
        public static int GetUnreadMessageCount(int userId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT COUNT(*) as unread_count
                    FROM messages
                    WHERE receiver_id = @user_id AND is_read = false";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32("unread_count");
                        }
                    }
                }
            }
            return 0;
        }
        public static int GetUserIdByUsername(string username)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Id FROM users WHERE Username = @username LIMIT 1";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32("Id");
                        }
                    }
                }
            }
            return -1; // Not found
        }
        public static List<LocationSubmission> GetPendingLocationSubmissions()
        {
            var list = new List<LocationSubmission>();
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM locationsubmissions WHERE Status = 'Pending'";
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LocationSubmission
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                            Address = reader.GetString("Address"),
                            Status = reader.GetString("Status"),
                            SubmittedAt = reader.GetDateTime("SubmittedAt"),
                            // Add SubmittedById if you want to show it
                        });
                    }
                }
            }
            return list;
        }

        public static void UpdateLocationSubmissionStatus(int id, string status)
        {
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE locationsubmissions SET Status = @Status WHERE Id = @Id";
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<string> GetApprovedLocations()
        {
            var locations = new List<string>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Address FROM locationsubmissions WHERE Status = 'Approved'";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string address = reader.GetString("Address");
                        locations.Add(address);
                    }
                }
            }
            return locations;
        }

        public static bool IsUserAdmin(int userId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Role FROM users WHERE Id = @userId";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string role = reader.GetString("Role");
                            return role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                        }
                    }
                }
            }
            return false;
        }

        public static void DeleteLocationSubmission(int id)
        {
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM locationsubmissions WHERE Id = @Id";
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<LocationSubmission> GetApprovedLocationSubmissions()
        {
            var list = new List<LocationSubmission>();
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM locationsubmissions WHERE Status = 'Approved'";
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LocationSubmission
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                            Address = reader.GetString("Address"),
                            Status = reader.GetString("Status"),
                            SubmittedAt = reader.GetDateTime("SubmittedAt"),
                        });
                    }
                }
            }
            return list;
        }
    }
}