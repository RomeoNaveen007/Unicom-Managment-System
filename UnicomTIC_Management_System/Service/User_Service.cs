using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Data.DB_Connection;
using UnicomTIC_Management_System.Model;

namespace UnicomTIC_Management_System.Service
{
    internal class User_Service
    {
        public List<User> Show_All_Users()
        {
            List<User> users = new List<User>();

            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"SELECT User_ID, User_Name, Password, Role FROM User;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new User
                                {
                                    User_ID = reader.GetInt32(0),
                                    User_Name = reader.GetString(1),
                                    Password = reader.GetString(2),
                                    Role = reader.GetString(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
            }

            return users;
        }

        public bool Authenticate(string username, string password)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = "SELECT COUNT(*) FROM User WHERE User_Name = @User AND Password = @Pass";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@User", username);
                    cmd.Parameters.AddWithValue("@Pass", password);

                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public User GetUserByUsername(string username)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = "SELECT User_ID, User_Name, Password, Role FROM User WHERE User_Name = @Username";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                User_ID = Convert.ToInt32(reader["User_ID"]),
                                User_Name = reader["User_Name"].ToString(),
                                Password = reader["Password"].ToString(),
                                Role = reader["Role"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

    }
}
