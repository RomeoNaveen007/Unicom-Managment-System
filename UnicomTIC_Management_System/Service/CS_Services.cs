using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Data.DB_Connection;
using UnicomTIC_Management_System.Model;

namespace UnicomTIC_Management_System.Service
{
    internal class CS_Services
    {
        public DataTable LoadCoursesIntoComboBox()
        {
            DataTable dt = new DataTable();

            using (var conn = DB_Config.getConnection())
            {
                string query = "SELECT Course_ID, Course_Name FROM Course WHERE Course_Status = 'Active';";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        public DataTable LoadSubjectsForComboBox()
        {
            DataTable dt = new DataTable();

            using (var conn = DB_Config.getConnection())
            {
                string query = "SELECT Subject_ID, Subject_Name FROM Subject;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        public void AddCourseSubject(int courseId, string courseName, int subjectId, string subjectName)
        {
            using (var conn = DB_Config.getConnection())
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    string query = @"
                INSERT INTO Course_Subject (Course_ID, Subject_ID)
                VALUES (@Course_ID, @Subject_ID);";

                    using (var cmd = new SQLiteCommand(query, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Course_ID", courseId);
                        cmd.Parameters.AddWithValue("@Subject_ID", subjectId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show($"Course-Subject mapping added: {courseName} ➝ {subjectName}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Failed to add Course-Subject entry: " + ex.Message);
                }
            }
        }


        public List<Course_Subject> GetAllCourseSubjects()
        {
            List<Course_Subject> courseSubjects = new List<Course_Subject>();

            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                SELECT 
                    cs.CS_ID,
                    cs.Course_ID,
                    cs.Subject_ID,
                    c.Course_Name,
                    s.Subject_Name
                FROM Course_Subject cs
                LEFT JOIN Course c ON cs.Course_ID = c.Course_ID
                LEFT JOIN Subject s ON cs.Subject_ID = s.Subject_ID;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            courseSubjects.Add(new Course_Subject
                            {
                                CS_ID = reader.GetInt32(0),
                                Cousre_ID = reader.GetInt32(1),
                                Course_Name = reader.IsDBNull(2) ? null : reader.GetString(3),
                                Subject_ID = reader.GetInt32(3).ToString(),
                                Subject_Name = reader.IsDBNull(4) ? null : reader.GetString(4)
                            });
                        }
                    }
                }
            }
            catch (SQLiteException sqlEx)
            {
                MessageBox.Show("Database error while retrieving course-subject data:\n" + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error:\n" + ex.Message);
            }

            return courseSubjects;
        }


        public Course_Subject Get_CourseSubject_By_Id(int csId)
        {
            Course_Subject courseSubject = null;

            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                SELECT 
                    cs.CS_ID,
                    cs.Course_ID,
                    cs.Subject_ID,
                    c.Course_Name,
                    s.Subject_Name
                FROM Course_Subject cs
                LEFT JOIN Course c ON cs.Course_ID = c.Course_ID
                LEFT JOIN Subject s ON cs.Subject_ID = s.Subject_ID
                WHERE cs.CS_ID = @CS_ID;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CS_ID", csId);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                courseSubject = new Course_Subject
                                {
                                    CS_ID = reader.GetInt32(0),
                                    Cousre_ID = reader.GetInt32(1),
                                    Subject_ID = reader.GetInt32(2).ToString(),
                                    Course_Name = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    Subject_Name = reader.IsDBNull(4) ? null : reader.GetString(4)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving course-subject details: " + ex.Message);
            }

            return courseSubject;
        }


        public bool UpdateCourseSubject(Course_Subject updatedCS)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                UPDATE Course_Subject
                SET Course_ID = @Course_ID,
                    Subject_ID = @Subject_ID
                WHERE CS_ID = @CS_ID;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Course_ID", updatedCS.Cousre_ID);
                        cmd.Parameters.AddWithValue("@Subject_ID", updatedCS.Subject_ID);
                        cmd.Parameters.AddWithValue("@CS_ID", updatedCS.CS_ID);
                        int result = cmd.ExecuteNonQuery();

                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message);
                return false;
            }
        }

        public bool DeleteCourseSubject(int csId)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = "DELETE FROM Course_Subject WHERE CS_ID = @CS_ID;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CS_ID", csId);
                        int result = cmd.ExecuteNonQuery();

                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed: " + ex.Message);
                return false;
            }
        }


    }
}
