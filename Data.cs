using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using InspirationLabProjectStanSeyit.Models;
using System.IO;

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
                string query = "SELECT Id FROM users WHERE LOWER(Username) = LOWER(@username) LIMIT 1";
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

        public static void AddContactMessage(string name, string email, string subject, string message, int? userId = null)
        {
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO contactmessages (Name, Email, Subject, Message, UserId)
                                 VALUES (@Name, @Email, @Subject, @Message, @UserId)";
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Subject", subject ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Message", message);
                    cmd.Parameters.AddWithValue("@UserId", userId.HasValue ? (object)userId.Value : DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<ContactMessage> GetAllContactMessages()
        {
            var list = new List<ContactMessage>();
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM contactmessages ORDER BY SubmittedAt DESC";
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ContactMessage
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Email = reader.GetString("Email"),
                            Subject = reader.IsDBNull(reader.GetOrdinal("Subject")) ? null : reader.GetString("Subject"),
                            Message = reader.GetString("Message"),
                            SubmittedAt = reader.GetDateTime("SubmittedAt"),
                            UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? (int?)null : reader.GetInt32("UserId"),
                            Handled = reader.GetBoolean("Handled"),
                            HandledByAdminId = reader.IsDBNull(reader.GetOrdinal("HandledByAdminId")) ? (int?)null : reader.GetInt32("HandledByAdminId"),
                            HandledAt = reader.IsDBNull(reader.GetOrdinal("HandledAt")) ? (DateTime?)null : reader.GetDateTime("HandledAt")
                        });
                    }
                }
            }
            return list;
        }

        public static void MarkContactMessageHandled(int messageId, int adminId)
        {
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"UPDATE contactmessages SET Handled = 1, HandledByAdminId = @AdminId, HandledAt = NOW() WHERE Id = @Id";
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", messageId);
                    cmd.Parameters.AddWithValue("@AdminId", adminId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void BanUser(int userId)
        {
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE users SET Banned = 1 WHERE Id = @UserId";
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    
        public static List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Username, Email, Banned FROM users";
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32("Id"),
                            Username = reader.GetString("Username"),
                            Email = reader.GetString("Email"),
                            Banned = reader.GetBoolean("Banned")
                        });
                    }
                }
            }
            return users;
        }
        public static void DeleteUser(int userId)
        {
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE users SET Deleted = NOW() WHERE Id = @UserId";
                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static int CreateStudyGroup(string name, string description, int ownerId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("INSERT INTO studygroups (Name, Description, OwnerId) VALUES (@name, @desc, @ownerId); SELECT LAST_INSERT_ID();", conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@desc", description);
                cmd.Parameters.AddWithValue("@ownerId", ownerId);
                int groupId = Convert.ToInt32(cmd.ExecuteScalar());

                // Add owner as first member
                var cmd2 = new MySqlCommand("INSERT INTO studygroupmembers (GroupId, UserId) VALUES (@groupId, @ownerId);", conn);
                cmd2.Parameters.AddWithValue("@groupId", groupId);
                cmd2.Parameters.AddWithValue("@ownerId", ownerId);
                cmd2.ExecuteNonQuery();

                return groupId;
            }
        }
        public static bool AddMemberToGroup(int groupId, int userId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("INSERT IGNORE INTO studygroupmembers (GroupId, UserId) VALUES (@groupId, @userId);", conn);
                cmd.Parameters.AddWithValue("@groupId", groupId);
                cmd.Parameters.AddWithValue("@userId", userId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public static List<int> GetGroupsForUser(int userId)
        {
            var groups = new List<int>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT GroupId FROM studygroupmembers WHERE UserId = @userId;", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        groups.Add(reader.GetInt32(0));
                }
            }
            return groups;
        }
        public static List<int> GetMembersOfGroup(int groupId)
        {
            var members = new List<int>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT UserId FROM studygroupmembers WHERE GroupId = @groupId;", conn);
                cmd.Parameters.AddWithValue("@groupId", groupId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        members.Add(reader.GetInt32(0));
                }
            }
            return members;
        }
        public static bool RemoveMemberFromGroup(int groupId, int userId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM studygroupmembers WHERE GroupId = @groupId AND UserId = @userId;", conn);
                cmd.Parameters.AddWithValue("@groupId", groupId);
                cmd.Parameters.AddWithValue("@userId", userId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public static bool IsUserGroupModerator(int groupId, int userId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT OwnerId FROM studygroups WHERE Id = @groupId", conn);
                cmd.Parameters.AddWithValue("@groupId", groupId);
                var ownerId = cmd.ExecuteScalar();
                return ownerId != null && Convert.ToInt32(ownerId) == userId;
            }
        }
        public static bool RemoveMemberFromGroup(int groupId, int userIdToRemove, int actingUserId)
        {
            // Only allow if actingUserId is the moderator
            if (!IsUserGroupModerator(groupId, actingUserId))
                return false;

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM studygroupmembers WHERE GroupId = @groupId AND UserId = @userId", conn);
                cmd.Parameters.AddWithValue("@groupId", groupId);
                cmd.Parameters.AddWithValue("@userId", userIdToRemove);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public static List<StudyGroup> GetGroupsOwnedByUser(int ownerId)
        {
            var groups = new List<StudyGroup>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT Id, Name, Description FROM studygroups WHERE OwnerId = @ownerId", conn);
                cmd.Parameters.AddWithValue("@ownerId", ownerId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groups.Add(new StudyGroup
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description")
                        });
                    }
                }
            }
            return groups;
        }
        public static List<StudyGroup> GetAllGroupsForUser(int userId)
        {
            var groups = new List<StudyGroup>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT g.Id, g.Name, g.Description, g.OwnerId
                      FROM studygroups g
                      JOIN studygroupmembers m ON g.Id = m.GroupId
                      WHERE m.UserId = @userId", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groups.Add(new StudyGroup
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                            CreatedById = reader.GetInt32("OwnerId")
                        });
                    }
                }
            }
            return groups;
        }
        public static void SendGroupMessage(int groupId, int userId, string message)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("INSERT INTO studygroupmessages (GroupId, UserId, Message) VALUES (@groupId, @userId, @message)", conn);
                cmd.Parameters.AddWithValue("@groupId", groupId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@message", message);
                cmd.ExecuteNonQuery();
            }
        }
        public static List<(string Username, string Message, DateTime SentAt)> GetGroupMessages(int groupId)
        {
            var messages = new List<(string, string, DateTime)>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT u.Username, m.Message, m.SentAt
              FROM studygroupmessages m
              JOIN users u ON m.UserId = u.Id
              WHERE m.GroupId = @groupId
              ORDER BY m.SentAt ASC", conn);
                cmd.Parameters.AddWithValue("@groupId", groupId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        messages.Add((
                            reader.GetString("Username"),
                            reader.GetString("Message"),
                            reader.GetDateTime("SentAt")
                        ));
                    }
                }
            }
            return messages;
        }
        public static void UploadGroupFile(int groupId, int userId, string fileName, string filePath)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("INSERT INTO studygroupfiles (GroupId, UserId, FileName, FilePath) VALUES (@groupId, @userId, @fileName, @filePath)", conn);
                cmd.Parameters.AddWithValue("@groupId", groupId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@fileName", fileName);
                cmd.Parameters.AddWithValue("@filePath", filePath);
                cmd.ExecuteNonQuery();
            }
        }
        public static void AddStudyMinutesToUser(int userId, int minutes)
        {
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                    "UPDATE userprofiles SET StudyHours = StudyHours + @addHours WHERE UserId = @userId", conn);
                cmd.Parameters.AddWithValue("@addHours", minutes / 60.0); // Store as hours (float) or change to int if you want only full hours
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.ExecuteNonQuery();
            }
        }
        public static List<(string FileName, string FilePath, string Username, DateTime UploadedAt)> GetGroupFiles(int groupId)
        {
            var files = new List<(string, string, string, DateTime)>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    @"SELECT f.FileName, f.FilePath, u.Username, f.UploadedAt
              FROM studygroupfiles f
              JOIN users u ON f.UserId = u.Id
              WHERE f.GroupId = @groupId
              ORDER BY f.UploadedAt DESC", conn);
                cmd.Parameters.AddWithValue("@groupId", groupId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        files.Add((
                            reader.GetString("FileName"),
                            reader.GetString("FilePath"),
                            reader.GetString("Username"),
                            reader.GetDateTime("UploadedAt")
                        ));
                    }
                }
            }
            return files;
        }
        public static void UploadStudyMaterial(string title, string filePath, int createdById, int studyGroupId)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("INSERT INTO studymaterials (Title, Content, FilePath, CreatedById, StudyGroupId, CreatedAt) VALUES (@title, @content, @filePath, @createdById, @studyGroupId, NOW())", conn);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@content", fileBytes);
                cmd.Parameters.AddWithValue("@filePath", filePath);
                cmd.Parameters.AddWithValue("@createdById", createdById);
                cmd.Parameters.AddWithValue("@studyGroupId", studyGroupId);
                cmd.ExecuteNonQuery();
            }
        }

        public static (string FileName, byte[] Content) GetStudyMaterialFile(int materialId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT Title, Content FROM studymaterials WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", materialId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && !reader.IsDBNull(1))
                    {
                        return (reader.GetString("Title"), (byte[])reader["Content"]);
                    }
                }
            }
            return (null, null);
        }

        public static string GetConnectionString()
        {
            return connectionString;
        }

        public static UserProfile GetUserProfile(int userId)
        {
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM userprofiles WHERE UserId = @userId", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserProfile
                        {
                            Id = reader.GetInt32("Id"),
                            UserId = reader.GetInt32("UserId"),
                            Bio = reader["Bio"] as string,
                            Birthday = reader["Birthday"] as DateTime?,
                            Location = reader["Location"] as string,
                            Name = reader["Name"] as string,
                            Age = reader["Age"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["Age"]),
                            School = reader["School"] as string,
                            Course = reader["Course"] as string,
                            Year = reader["Year"] as string,
                            StudyHours = reader["StudyHours"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["StudyHours"])
                        };
                    }
                }
            }
            return null;
        }
        public static void SaveGameScore(int userId, string gameType, int score, DateTime playedAt)
        {
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                    "INSERT INTO gamescores (UserId, GameType, Score, PlayedAt) VALUES (@userId, @gameType, @score, @playedAt)", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@gameType", gameType);
                cmd.Parameters.AddWithValue("@score", score);
                cmd.Parameters.AddWithValue("@playedAt", playedAt);
                cmd.ExecuteNonQuery();
            }
        }
        public static void UpdateUserProfile(UserProfile profile)
        {
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                    "UPDATE userprofiles SET Bio = @bio, Birthday = @birthday, Location = @location, Name = @name, Age = @age, School = @school, Course = @course, Year = @year WHERE UserId = @userId", conn);
                cmd.Parameters.AddWithValue("@bio", (object)profile.Bio ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@birthday", (object)profile.Birthday ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@location", (object)profile.Location ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@name", (object)profile.Name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@age", (object)profile.Age ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@school", (object)profile.School ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@course", (object)profile.Course ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@year", (object)profile.Year ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@userId", profile.UserId);
                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    // Insert if not exists
                    var insertCmd = new MySql.Data.MySqlClient.MySqlCommand(
                        "INSERT INTO userprofiles (UserId, Bio, Birthday, Location, Name, Age, School, Course, Year) VALUES (@userId, @bio, @birthday, @location, @name, @age, @school, @course, @year)", conn);
                    insertCmd.Parameters.AddWithValue("@userId", profile.UserId);
                    insertCmd.Parameters.AddWithValue("@bio", (object)profile.Bio ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@birthday", (object)profile.Birthday ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@location", (object)profile.Location ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@name", (object)profile.Name ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@age", (object)profile.Age ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@school", (object)profile.School ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@course", (object)profile.Course ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@year", (object)profile.Year ?? DBNull.Value);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        public class GameScoreEntry
        {
            public string Username { get; set; }
            public int Score { get; set; }
        }

        public static List<GameScoreEntry> GetTopGameScores(string gameType, int count)
        {
            var scores = new List<GameScoreEntry>();
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySql.Data.MySqlClient.MySqlCommand(
                    "SELECT gs.Score, u.Username FROM gamescores gs JOIN users u ON gs.UserId = u.Id WHERE gs.GameType = @gameType ORDER BY gs.Score DESC, gs.PlayedAt ASC LIMIT @count", conn);
                cmd.Parameters.AddWithValue("@gameType", gameType);
                cmd.Parameters.AddWithValue("@count", count);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        scores.Add(new GameScoreEntry
                        {
                            Username = reader.GetString("Username"),
                            Score = reader.GetInt32("Score")
                        });
                    }
                }
            }
            return scores;
        }

        public static void AddTask(int userId, string title, string priority, string category, DateTime dueDate)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO tasks (UserId, Title, Priority, Category, DueDate, IsCompleted, CreatedAt)
                         VALUES (@UserId, @Title, @Priority, @Category, @DueDate, false, NOW())";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Priority", priority);
                    cmd.Parameters.AddWithValue("@Category", category);
                    cmd.Parameters.AddWithValue("@DueDate", dueDate);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateTaskCompletion(int taskId, bool isCompleted)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE tasks SET IsCompleted = @IsCompleted WHERE Id = @TaskId";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                    cmd.Parameters.AddWithValue("@IsCompleted", isCompleted);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteTask(int taskId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM tasks WHERE Id = @TaskId";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<Task> GetTasksForUser(int userId, DateTime? date = null)
        {
            var tasks = new List<Task>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT * FROM tasks 
                               WHERE UserId = @UserId 
                               AND (@Date IS NULL OR DATE(DueDate) = DATE(@Date))
                               ORDER BY DueDate ASC";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Date", (object)date ?? DBNull.Value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new Task
                            {
                                Id = reader.GetInt32("Id"),
                                UserId = reader.GetInt32("UserId"),
                                Title = reader.GetString("Title"),
                                Priority = reader.GetString("Priority"),
                                Category = reader.GetString("Category"),
                                DueDate = reader.GetDateTime("DueDate"),
                                IsCompleted = reader.GetBoolean("IsCompleted"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            });
                        }
                    }
                }
            }
            return tasks;
        }
        public static void AddInvitation(int senderId, string recipientEmail)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO invitations (sender_id, recipient_email) VALUES (@senderId, @recipientEmail)";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@senderId", senderId);
                    cmd.Parameters.AddWithValue("@recipientEmail", recipientEmail);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static List<string> GetPendingInvitations(int senderId)
        {
            var invitations = new List<string>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT recipient_email FROM invitations WHERE sender_id = @senderId AND status = 'pending'";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@senderId", senderId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            invitations.Add(reader.GetString("recipient_email"));
                        }
                    }
                }
            }
            return invitations;
        }
        public class Task
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string Title { get; set; }
            public string Priority { get; set; }
            public string Category { get; set; }
            public DateTime DueDate { get; set; }
            public bool IsCompleted { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}