using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace InspirationLabProjectStanSeyit
{
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
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pendingRequests.Add(reader.GetString("Username"));
                        }
                    }
                }
            }
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

    }
}