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
    internal class Course_Service
    {

        public void AddCourse(Course course)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
            INSERT INTO Course (Course_Name, Duration, Course_Status)
            VALUES (@Course_Name, @Duration, @Course_Status);";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Course_Name", course.Course_Name);
                    cmd.Parameters.AddWithValue("@Duration", course.Duration);
                    cmd.Parameters.AddWithValue("@Course_Status", course.Course_Status);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show($"{course.Course_Name} added successfully.");
                }
            }
        }

        public List<Course> GetAllCourse()
        {
            List<Course> courseList = new List<Course>();

            using (var conn = DB_Config.getConnection())
            {
                string query = @"SELECT Course_ID, Course_Name, Duration, Course_Status FROM Course;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Course course = new Course
                            {
                                Course_ID = reader.GetInt32(0),
                                Course_Name = reader.GetString(1),
                                Duration = reader.GetString(2),
                                Course_Status = reader.GetString(3)
                            };

                            courseList.Add(course);
                        }
                    }
                }
            }

            return courseList;
        }

        public Course GetCourseById(int id)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
            SELECT 
                Course_ID, 
                Course_Name, 
                Duration, 
                Course_Status
            FROM Course
            WHERE Course_ID = @Id;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Course
                            {
                                Course_ID = reader.GetInt32(0),
                                Course_Name = reader.GetString(1),
                                Duration = reader.GetString(2),
                                Course_Status = reader.GetString(3)
                            };
                        }
                    }
                }
            }

            return null; // Return null if not found
        }



        public void Update_Course(Course course)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
            UPDATE Course 
            SET 
                Course_Name = @Course_Name, 
                Duration = @Duration
            WHERE 
                Course_ID = @Course_ID;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Course_ID", course.Course_ID);
                    cmd.Parameters.AddWithValue("@Course_Name", course.Course_Name);
                    cmd.Parameters.AddWithValue("@Duration", course.Duration);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        MessageBox.Show($"{course.Course_Name} updated successfully.");
                    else
                        MessageBox.Show("Update failed. Course not found.");
                }
            }
        }


        public void Delete_Course(Course del_course)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
            UPDATE Course 
            SET Course_Status = @Course_Status 
            WHERE Course_ID = @Course_ID;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Course_Status", del_course.Course_Status);
                    cmd.Parameters.AddWithValue("@Course_ID", del_course.Course_ID);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show($"Course ID {del_course.Course_Name} IS DELETED.");
                    }
                    else
                    {
                        MessageBox.Show($"Course ID {del_course.Course_Name} not found.");
                    }
                }
            }
        }

      

    }
}
