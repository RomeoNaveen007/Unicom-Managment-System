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
       

        public void Addadmin (Admin ad)        // Adding Admim in admin table and in user table........
        {
            using (var conn = DB_Config.getConnection())
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int userId;
                    using (var userCmd = new SQLiteCommand(@"
                        INSERT INTO User (User_Name, Password, Role)
                        VALUES (@User_Name, @Password, @Role);
                        SELECT last_insert_rowid();", conn, transaction)) // ✅ pass the transaction
                        {
                            userCmd.Parameters.AddWithValue("@User_Name", ad.User_Name);
                            userCmd.Parameters.AddWithValue("@Password", ad.Password);
                            userCmd.Parameters.AddWithValue("@Role", ad.Role);

                            userId = Convert.ToInt32(userCmd.ExecuteScalar());
                        }


                        using (var adminCmd = new SQLiteCommand(@"
                            INSERT INTO Admin (Admin_Name, Admin_Address, Admin_NIC, Admin_Status, User_ID)
                            VALUES (@Admin_Name, @Admin_Address, @Admin_NIC, @Admin_Status, @User_ID);", conn, transaction)) // ✅ pass the transaction
                                    {
                                        adminCmd.Parameters.AddWithValue("@Admin_Name", ad.Admin_Name);
                                        adminCmd.Parameters.AddWithValue("@Admin_Address", ad.Admin_Address);
                                        adminCmd.Parameters.AddWithValue("@Admin_NIC", ad.Admin_NIC);
                                        adminCmd.Parameters.AddWithValue("@Admin_Status", ad.Admin_Status);
                                        adminCmd.Parameters.AddWithValue("@User_ID", userId);

                                        adminCmd.ExecuteNonQuery();
                                    }
                        transaction.Commit(); // ✅ Only call this after successful operations
                        MessageBox.Show("Admin added successfully!");
                    }

                    catch (Exception ex)
                    {
                        transaction.Rollback(); // ❌ Rollback if anything fails
                        MessageBox.Show("Failed to add admin: " + ex.Message);
                    }

                }

            }
            
        }


        public List<Admin> Show_Output()
        {
            List<Admin> adminList = new List<Admin>();

            using (var conn = DB_Config.getConnection())
            {
                var cmd = new SQLiteCommand(@"
            SELECT 
                a.Admin_ID,
                a.Admin_Name,
                a.Admin_Address,
                a.Admin_NIC,
                a.Admin_Status,
                u.User_Name
            FROM Admin a
            LEFT JOIN User u ON a.User_ID = u.User_ID", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Admin admin = new Admin
                        {
                            Admin_ID = reader.GetInt32(0),
                            Admin_Name = reader.GetString(1),
                            Admin_Address = reader.GetString(2),
                            Admin_NIC = reader.GetString(3),
                            Admin_Status = reader.GetString(4),
                            User_Name = reader.GetString(5)
                        };

                        adminList.Add(admin);
                    }
                }
            }
            return adminList;
        }
        public Admin Get_Admin_id(int id)
        {
            using (var conn = DB_Config.getConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(@"SELECT a.Admin_Name,a.Admin_Address,a.Admin_NIC,u.User_Name FROM Admin a  LEFT JOIN User u ON a.User_ID = u.User_ID WHERE Admin_ID = @Admin_ID", conn))

                {
                    cmd.Parameters.AddWithValue("@Admin_ID", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Admin
                            {
                                Admin_Name = reader.GetString(0),
                                Admin_Address = reader.GetString(1),
                                Admin_NIC = reader.GetString(2),
                                User_Name = reader.GetString(3)
                            };
                        }
                    }

                }
            }

            return null;

        }


    }
}
