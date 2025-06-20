using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Data.DB_Connection;
using UnicomTIC_Management_System.Model;

namespace UnicomTIC_Management_System.Service
{
    internal class Admin_Service
    {
        public Admin_Service()
        {

        }
        public void Addadmin(Admin admin)
        {
            using (var conn = DB_Config.getConnection())
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string user_query = @"INSERT INTO [User] (User_Name, Password, Role) 
                                        VALUES (@User_Name, @Password, @Role); 
                                        SELECT last_insert_rowid();";

                        long userId;

                        using (var user_cmd = new SQLiteCommand(user_query, conn, transaction))
                        {
                            user_cmd.Parameters.AddWithValue("@User_Name", admin.User_Name);
                            user_cmd.Parameters.AddWithValue("@Password", admin.Password);
                            user_cmd.Parameters.AddWithValue("@Role", admin.Role);

                            userId = (long)user_cmd.ExecuteScalar(); // Fetch newly created User_ID
                        }

                        string admin_query = @"INSERT INTO Admin (Admin_Name, Admin_Address, Admin_NIC, Admin_Status, User_Name, User_ID) 
                                        VALUES (@Admin_Name, @Admin_Address, @Admin_NIC, @Admin_Status, @User_Name, @User_ID);";

                        using (var admin_cmd = new SQLiteCommand(admin_query, conn, transaction))
                        {
                            admin_cmd.Parameters.AddWithValue("@Admin_Name", admin.Admin_Name);
                            admin_cmd.Parameters.AddWithValue("@Admin_Address", admin.Admin_Address);
                            admin_cmd.Parameters.AddWithValue("@Admin_NIC", admin.Admin_NIC);
                            admin_cmd.Parameters.AddWithValue("@Admin_Status", admin.Admin_Status);
                            admin_cmd.Parameters.AddWithValue("@User_Name", admin.User_Name);
                            admin_cmd.Parameters.AddWithValue("@User_ID", userId);

                            admin_cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show($"{admin.Admin_Name} added to admin table successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        public bool IsUsernameTaken(string username)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = "SELECT COUNT(*) FROM [User] WHERE LOWER(User_Name) = LOWER(@User_Name)";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@User_Name", username);

                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }


        public List<Admin> Get_All()
        {
            List<Admin> adminList = new List<Admin>();

            using (var conn = DB_Config.getConnection())
            {
                string query = @"SELECT a.Admin_ID, a.Admin_Name, a.Admin_Address, a.Admin_NIC, u.User_Name, a.Admin_Status
                            FROM Admin a
                            LEFT JOIN [User] u ON a.User_ID = u.User_ID;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            adminList.Add(new Admin
                            {
                                Admin_ID = reader.GetInt32(0),
                                Admin_Name = reader.GetString(1),
                                Admin_Address = reader.GetString(2),
                                Admin_NIC = reader.GetString(3),
                                User_Name = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Admin_Status = reader.GetString(5)
                            });
                        }
                    }
                }
            }

            return adminList;
        }

        public Admin Get_Admin_By_ID(int id)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"SELECT a.Admin_ID, a.Admin_Name, a.Admin_Address, a.Admin_NIC, u.User_Name
                            FROM Admin a
                            LEFT JOIN [User] u ON a.User_ID = u.User_ID
                            WHERE a.Admin_ID = @Id";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Admin
                            {
                                Admin_ID = reader.GetInt32(0),
                                Admin_Name = reader.GetString(1),
                                Admin_Address = reader.GetString(2),
                                Admin_NIC = reader.GetString(3),
                                User_Name = reader.IsDBNull(4) ? null : reader.GetString(4)
                            };
                        }
                    }
                }
            }

            return null;
        }

        public void Updateadmin(Admin up_admin)
        {
            using (var conn = DB_Config.getConnection())
            {
                string up_query = @"UPDATE Admin SET 
                            Admin_Name = @Admin_Name,
                            Admin_Address = @Admin_Address,
                            Admin_NIC = @Admin_NIC
                            WHERE Admin_ID = @Admin_ID;";

                using (SQLiteCommand cmd = new SQLiteCommand(up_query, conn))
                {
                    cmd.Parameters.AddWithValue("@Admin_Name", up_admin.Admin_Name);
                    cmd.Parameters.AddWithValue("@Admin_Address", up_admin.Admin_Address);
                    cmd.Parameters.AddWithValue("@Admin_NIC", up_admin.Admin_NIC);
                    cmd.Parameters.AddWithValue("@Admin_ID", up_admin.Admin_ID);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show($"{up_admin.Admin_Name} has been updated successfully.");
                }
            }
        }

        public void Deleteadmin(Admin del_admin)
        {
            using (var conn = DB_Config.getConnection())
            {
                string del_query = @"UPDATE Admin SET Admin_Status = @Admin_Status 
                                WHERE Admin_ID = @Admin_ID;";

                using (SQLiteCommand cmd = new SQLiteCommand(del_query, conn))
                {
                    cmd.Parameters.AddWithValue("@Admin_Status", del_admin.Admin_Status);
                    cmd.Parameters.AddWithValue("@Admin_ID", del_admin.Admin_ID);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show($"{del_admin.Admin_Name} has been marked as Inactive.");
                }
            }
        }

        public List<Admin> Search_Admin_By_Name(string adminName)
        {
            List<Admin> search_admins = new List<Admin>();

            using (var conn = DB_Config.getConnection())
            {
                string query = @"SELECT a.Admin_ID, a.Admin_Name, a.Admin_Address, a.Admin_NIC, 
                                u.User_Name, a.Admin_Status
                            FROM Admin a
                            LEFT JOIN [User] u ON a.User_ID = u.User_ID
                            WHERE LOWER(a.Admin_Name) LIKE '%' || LOWER(@Admin_Name) || '%'";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Admin_Name", adminName);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            search_admins.Add(new Admin
                            {
                                Admin_ID = reader.GetInt32(0),
                                Admin_Name = reader.GetString(1),
                                Admin_Address = reader.GetString(2),
                                Admin_NIC = reader.GetString(3),
                                User_Name = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Admin_Status = reader.GetString(5)
                            });
                        }
                    }
                }
            }

            return search_admins;
        }

    }
}
