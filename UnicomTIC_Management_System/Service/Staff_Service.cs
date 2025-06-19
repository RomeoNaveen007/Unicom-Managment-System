using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Data.DB_Connection;
using UnicomTIC_Management_System.Model;
using UnicomTIC_Management_System.Controller;
using static System.Windows.Forms.AxHost;



namespace UnicomTIC_Management_System.Service
{
    internal class Staff_Service
    {
        public Staff_Service()
        {

        }

        public void GetAll_staff(Staff staff)
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
                            user_cmd.Parameters.AddWithValue("@User_Name", staff.User_Name);
                            user_cmd.Parameters.AddWithValue("@Password", staff.Password);
                            user_cmd.Parameters.AddWithValue("@Role", staff.Role);

                            userId = (long)user_cmd.ExecuteScalar(); // Get the last inserted User_ID
                        }

                        
                        string staff_query = @"INSERT INTO Staff (Staff_Name, Staff_Address, Staff_NIC, Staff_Status,User_Name, User_ID) 
                                   VALUES (@Staff_Name, @Staff_Address, @Staff_NIC, @Staff_Status,@User_Name, @User_ID);";

                        using (var staff_cmd = new SQLiteCommand(staff_query, conn, transaction))
                        {
                            staff_cmd.Parameters.AddWithValue("@Staff_Name", staff.Staff_Name);
                            staff_cmd.Parameters.AddWithValue("@Staff_Address", staff.Staff_Address);
                            staff_cmd.Parameters.AddWithValue("@Staff_NIC", staff.Staff_NIC);
                            staff_cmd.Parameters.AddWithValue("@Staff_Status", staff.Staff_Status);
                            staff_cmd.Parameters.AddWithValue("@User_Name", staff.User_Name);
                            staff_cmd.Parameters.AddWithValue("@User_ID", userId); // Use foreign key reference

                            staff_cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show($"{staff.Staff_Name} added to staff table successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }

        }

        public List<Staff> Get_All()
        {
            List<Staff> staffList = new List<Staff>();

            using (var conn = DB_Config.getConnection())
            {
                string query = @"
            SELECT s.Staff_ID, s.Staff_Name, s.Staff_Address, s.Staff_NIC, u.User_Name , s.Staff_Status
            FROM Staff s
            LEFT JOIN [User] u ON s.User_ID = u.User_ID;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            staffList.Add(new Staff
                            {
                                Staff_ID = reader.GetInt32(0),
                                Staff_Name = reader.GetString(1),
                                Staff_Address = reader.GetString(2),
                                Staff_NIC = reader.GetString(3),
                                User_Name = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Staff_Status = reader.GetString(5)
                            });
                        }
                    }
                }
            }

            return staffList;
        }


        public Staff Get_Staff_id(int id)
        {
            using (var conn = DB_Config.getConnection())
            {
                
                string query = @"
            SELECT s.Staff_ID, s.Staff_Name, s.Staff_Address, s.Staff_NIC, u.User_Name
            FROM Staff s
            LEFT JOIN [User] u ON s.User_ID = u.User_ID
            WHERE s.Staff_ID = @Id";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Staff
                            {
                                Staff_ID = reader.GetInt32(0),
                                Staff_Name = reader.GetString(1),
                                Staff_Address = reader.GetString(2),
                                Staff_NIC = reader.GetString(3),
                                User_Name = reader.IsDBNull(4) ? null : reader.GetString(4)
                            };
                        }
                    }
                }
            }
            return null;
        }


        public void staff_Update(Staff up_staff)
        {
            using (var conn = DB_Config.getConnection())
            {
               

                string Up_query = @" UPDATE Staff SET Staff_Name = @Staff_Name,
                                                Staff_Address = @Staff_Address,
                                                Staff_NIC = @Staff_NIC 
                             WHERE Staff_ID = @Staff_ID ; ";

                using (SQLiteCommand cmd = new SQLiteCommand(Up_query, conn))
                {
                    cmd.Parameters.AddWithValue("@Staff_Name", up_staff.Staff_Name);
                    cmd.Parameters.AddWithValue("@Staff_Address", up_staff.Staff_Address);
                    cmd.Parameters.AddWithValue("@Staff_NIC", up_staff.Staff_NIC);
                    cmd.Parameters.AddWithValue("@Staff_ID", up_staff.Staff_ID);  // Only once

                    cmd.ExecuteNonQuery();

                    MessageBox.Show($"{up_staff.Staff_Name} Row is Updated successfully...");
                }
            }
        }

        public void Delete_Staff(Staff del_stff)
        {
            using (var conn = DB_Config.getConnection())
            {
                string del_query = @" UPDATE Staff SET Staff_Status = @Staff_Status
                                     WHERE Staff_ID = @Staff_ID ; ";

                using (SQLiteCommand cmd = new SQLiteCommand(del_query, conn))
                {
                    cmd.Parameters.AddWithValue("@Staff_Status", del_stff.Staff_Status);
                    cmd.Parameters.AddWithValue("@Staff_ID", del_stff.Staff_ID);  // Only once

                    cmd.ExecuteNonQuery();

                    MessageBox.Show($"{del_stff.Staff_Name} Row is Deleted successfully...");


                }

            }
        }
    }
}
