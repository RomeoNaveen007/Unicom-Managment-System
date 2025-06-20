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
    internal class Subject_Service
    {
        public bool IsLecturerExistsForSubject(string subjectName)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = "SELECT COUNT(*) FROM Lecturer WHERE Special_In = @Special_In";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Special_In", subjectName);
                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public bool IsSubjectExists(string subjectName)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = "SELECT COUNT(*) FROM Subject WHERE Subject_Name = @Subject_Name";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Subject_Name", subjectName);
                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }


        public void AddSubject(Subject subject)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
                                INSERT INTO Subject (Subject_Name)
                                VALUES (@Subject_Name);";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Subject_Name", subject.Subject_Name);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Subject> Get_All_Subjects()
        {
            List<Subject> subjects = new List<Subject>();

            using (var conn = DB_Config.getConnection())
            {
                string query = "SELECT Subject_ID, Subject_Name FROM Subject";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            subjects.Add(new Subject
                            {
                                Subject_ID = Convert.ToInt32(reader["Subject_ID"]),
                                Subject_Name = reader["Subject_Name"].ToString()
                            });
                        }
                    }
                }
            }

            return subjects;
        }

        public Subject GetSubjectById(int id)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
            SELECT 
                Subject_ID, 
                Subject_Name
            FROM Subject
            WHERE Subject_ID = @Id;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Subject
                            {
                                Subject_ID = reader.GetInt32(0),
                                Subject_Name = reader.GetString(1)
                            };
                        }
                    }
                }
            }

            return null; // Return null if not found
        }


        public void Update_Subject(Subject subject)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
                                    UPDATE Subject
                                    SET 
                                        Subject_Name = @Subject_Name
                                    WHERE 
                                        Subject_ID = @Subject_ID;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Subject_ID", subject.Subject_ID);
                    cmd.Parameters.AddWithValue("@Subject_Name", subject.Subject_Name);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        MessageBox.Show($"{subject.Subject_Name} updated successfully.");
                    else
                        MessageBox.Show("Update failed. Subject not found.");
                }
            }
        }

        public void Delete_Subject(Subject del_subject)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"DELETE FROM Subject WHERE Subject_ID = @Subject_ID;";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Subject_ID", del_subject.Subject_ID);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show($"Subject '{del_subject.Subject_Name}' deleted successfully.");
                    }
                    else
                    {
                        MessageBox.Show($"Subject '{del_subject.Subject_Name}' not found.");
                    }
                }
            }
        }


    }
}
