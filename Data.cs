using System;
using MySql.Data.MySqlClient;

namespace InspirationLabProjectStanSeyit
{
    public static class Data
    {
        private static string connectionString = "Server=localhost;Port=3307;Database=inspirationlabdb;Uid=root;Pwd=;";

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
    }
}