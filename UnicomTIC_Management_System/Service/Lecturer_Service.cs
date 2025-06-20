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
    internal class Lecturer_Service
    {
        public Lecturer_Service()
        {


        }

        public void AddLecturer(Lecturer lecturer)
        {
            using (var conn = DB_Config.getConnection())
            {

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Insert User record first and get the new User_ID
                        string userQuery = @"
                    INSERT INTO [User] (User_Name, Password, Role) 
                    VALUES (@User_Name, @Password, @Role); 
                    SELECT last_insert_rowid();";

                        long userId;
                        using (var userCmd = new SQLiteCommand(userQuery, conn, transaction))
                        {
                            userCmd.Parameters.AddWithValue("@User_Name", lecturer.User_Name);
                            userCmd.Parameters.AddWithValue("@Password", lecturer.Password);
                            userCmd.Parameters.AddWithValue("@Role", lecturer.Role);

                            userId = (long)userCmd.ExecuteScalar();
                        }

                        // Insert Lecturer record using the User_ID as foreign key
                        string lecturerQuery = @"
                    INSERT INTO Lecturer 
                    (Lecturer_Name, Lecturer_Address, Lecturer_NIC, Lecturer_Status, Special_In, User_ID) 
                    VALUES 
                    (@Lecturer_Name, @Lecturer_Address, @Lecturer_NIC, @Lecturer_Status, @Special_In, @User_ID);";

                        using (var lecturerCmd = new SQLiteCommand(lecturerQuery, conn, transaction))
                        {
                            lecturerCmd.Parameters.AddWithValue("@Lecturer_Name", lecturer.Lecturer_Name);
                            lecturerCmd.Parameters.AddWithValue("@Lecturer_Address", lecturer.Lecturer_Address);
                            lecturerCmd.Parameters.AddWithValue("@Lecturer_NIC", lecturer.Lecturer_NIC);
                            lecturerCmd.Parameters.AddWithValue("@Lecturer_Status", lecturer.Lecturer_Status);
                            lecturerCmd.Parameters.AddWithValue("@Special_In", lecturer.Special_In);
                            lecturerCmd.Parameters.AddWithValue("@User_ID", userId);

                            lecturerCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show($"{lecturer.Lecturer_Name} added successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error adding lecturer: " + ex.Message);
                    }
                }
            }
        }

        public List<Lecturer> Get_All_Lecturers()
        {
            List<Lecturer> lecturerList = new List<Lecturer>();

            using (var conn = DB_Config.getConnection())
            {
                string query = @"
                                    SELECT 
                                        l.Lecturer_ID, 
                                        l.Lecturer_Name, 
                                        l.Lecturer_Address, 
                                        l.Lecturer_NIC, 
                                        l.Lecturer_Status, 
                                        l.Special_In, 
                                        u.User_Name 
                                    FROM Lecturer l
                                    LEFT JOIN [User] u ON l.User_ID = u.User_ID;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lecturerList.Add(new Lecturer
                            {
                                Lecturer_ID = reader.GetInt32(0),
                                Lecturer_Name = reader.GetString(1),
                                Lecturer_Address = reader.GetString(2),
                                Lecturer_NIC = reader.GetString(3),
                                Lecturer_Status = reader.GetString(4),
                                Special_In = reader.GetString(5),
                                User_Name = reader.IsDBNull(6) ? null : reader.GetString(6)
                            });
                        }
                    }
                }
            }

            return lecturerList;
        }

        public Lecturer Get_Lecturer_By_Id(int id)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
                                SELECT 
                                    l.Lecturer_ID, 
                                    l.Lecturer_Name, 
                                    l.Lecturer_Address, 
                                    l.Lecturer_NIC, 
                                    l.Lecturer_Status, 
                                    l.Special_In, 
                                    l.User_ID,
                                    u.User_Name
                                FROM Lecturer l
                                LEFT JOIN [User] u ON l.User_ID = u.User_ID
                                WHERE l.Lecturer_ID = @Id;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Lecturer
                            {
                                Lecturer_ID = reader.GetInt32(0),
                                Lecturer_Name = reader.GetString(1),
                                Lecturer_Address = reader.GetString(2),
                                Lecturer_NIC = reader.GetString(3),
                                Lecturer_Status = reader.GetString(4),
                                Special_In = reader.GetString(5),
                                User_ID = reader.GetInt32(6),
                                User_Name = reader.IsDBNull(7) ? null : reader.GetString(7)
                            };
                        }
                    }
                }
            }
            return null; // Return null if not found
        }



        public void Update_Lecturer(Lecturer up_lecturer)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
            UPDATE Lecturer 
            SET 
                Lecturer_Name = @Lecturer_Name,
                Lecturer_Address = @Lecturer_Address,
                Lecturer_NIC = @Lecturer_NIC,
                Lecturer_Status = @Lecturer_Status,
                Special_In = @Special_In
            WHERE Lecturer_ID = @Lecturer_ID;";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Lecturer_Name", up_lecturer.Lecturer_Name);
                    cmd.Parameters.AddWithValue("@Lecturer_Address", up_lecturer.Lecturer_Address);
                    cmd.Parameters.AddWithValue("@Lecturer_NIC", up_lecturer.Lecturer_NIC);
                    cmd.Parameters.AddWithValue("@Lecturer_Status", up_lecturer.Lecturer_Status);
                    cmd.Parameters.AddWithValue("@Special_In", up_lecturer.Special_In);
                    cmd.Parameters.AddWithValue("@Lecturer_ID", up_lecturer.Lecturer_ID);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show($"{up_lecturer.Lecturer_Name}'s record updated successfully.");
                }
            }
        }

        public void Delete_Lecturer(Lecturer del_lecturer)
        {
            using (var conn = DB_Config.getConnection())
            {
             
                string query = @"
            UPDATE Lecturer 
            SET Lecturer_Status = @Lecturer_Status 
            WHERE Lecturer_ID = @Lecturer_ID;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Lecturer_Status", del_lecturer.Lecturer_Status);
                    cmd.Parameters.AddWithValue("@Lecturer_ID", del_lecturer.Lecturer_ID);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show($"{del_lecturer.Lecturer_Name}'s record marked as deleted (status set to {del_lecturer.Lecturer_Status}).");
                }
            }
        }

        public List<Lecturer> Get_Searched_Lecturer_Name(string lecturerName)
        {
            List<Lecturer> searchedLecturers = new List<Lecturer>();

            using (var conn = DB_Config.getConnection())
            {
                string query = @"
            SELECT 
                l.Lecturer_ID, 
                l.Lecturer_Name, 
                l.Lecturer_Address, 
                l.Lecturer_NIC, 
                u.User_Name, 
                l.Lecturer_Status,
                l.Special_In
            FROM Lecturer l
            LEFT JOIN [User] u ON l.User_ID = u.User_ID
            WHERE LOWER(l.Lecturer_Name) LIKE '%' || LOWER(@Lecturer_Name) || '%';";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Lecturer_Name", lecturerName);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            searchedLecturers.Add(new Lecturer
                            {
                                Lecturer_ID = reader.GetInt32(0),
                                Lecturer_Name = reader.GetString(1),
                                Lecturer_Address = reader.GetString(2),
                                Lecturer_NIC = reader.GetString(3),
                                User_Name = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Lecturer_Status = reader.GetString(5),
                                Special_In = reader.GetString(6)
                            });
                        }
                    }
                }
            }

            return searchedLecturers;
        }




    }
}
