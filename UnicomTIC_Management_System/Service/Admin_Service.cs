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
                string query = "INSERT IMTO Admin (Admin_Name,Admin_Address,Admin_NIC,Admin_Status) VALUES (@Admin_Name ,@Admin_Address,@Admin_NIC,@Admin_Status)";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Admin_Name", ad.Admin_Name);
                    cmd.Parameters.AddWithValue("@Admin_Address", ad.Admin_Address);
                    cmd.Parameters.AddWithValue("@Admin_NIC", ad.Admin_NIC);
                    cmd.Parameters.AddWithValue("@Admin_Status", ad.Admin_Status);
                    cmd.ExecuteNonQuery();

                }

                using (SQLiteCommand cmd = new SQLiteCommand(@"INSERT IMTO User (User_Name,Password,Role) VALUES (@User_Name ,@Password,@Role)", conn))
                {
                    cmd.Parameters.AddWithValue("@User_Name", ad.User_Name);
                    cmd.Parameters.AddWithValue("@Password", ad.Password);
                    cmd.Parameters.AddWithValue("@Role", ad.Role);
                    cmd.ExecuteNonQuery();

                }

            }
            MessageBox.Show("Admin Table Created Sucessfully !!!");
        }

        public List<Admin> show_Output()
        {
            List<Admin> admin = new List<Admin>();

            using (var conn = DB_Config.getConnection())
            {

                string query = @"SELECT * FROM Admin ;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            admin.Add(new Admin
                            {
                                Admin_Name = reader.GetString(0),
                                Admin_Address = reader.GetString(1),
                                Admin_NIC = reader.GetString(2),
                                User_Name = reader.GetString(3)
                            });


                        }
                    }
                }

            }
            return admin;

        }
        public Admin Get_Admin_id(int id)
        {
            using (var conn = DB_Config.getConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(@"SELECT * FROM Student WHERE Admin_ID = @Admin_ID", conn))

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
