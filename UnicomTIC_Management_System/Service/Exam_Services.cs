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
    internal class Exam_Services
    {
        public List<string> GetCourseNames()
        {
            var list = new List<string>();

            using (var conn = DB_Config.getConnection())
            {
                string query = @"SELECT DISTINCT c.Course_Name 
                         FROM Course_Subject cs
                         LEFT JOIN Course c ON cs.Course_ID = c.Course_ID";

                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(reader.GetString(0));
                }
            }

            return list;
        }

        public List<string> GetSubjectsForCourse(string courseName)
        {
            var list = new List<string>();

            using (var conn = DB_Config.getConnection())
            {
                string query = @"
                        SELECT s.Subject_Name
                        FROM Course_Subject cs
                        LEFT JOIN Course c ON cs.Course_ID = c.Course_ID
                        LEFT JOIN Subject s ON cs.Subject_ID = s.Subject_ID
                        WHERE c.Course_Name = @CourseName";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseName", courseName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add(reader.GetString(0));
                    }
                }
            }

            return list;
        }

        public int GetCS_ID(string courseName, string subjectName)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
                    SELECT cs.CS_ID
                    FROM Course_Subject cs
                    LEFT JOIN Course c ON cs.Course_ID = c.Course_ID
                    LEFT JOIN Subject s ON cs.Subject_ID = s.Subject_ID
                    WHERE c.Course_Name = @Course AND s.Subject_Name = @Subject";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Course", courseName);
                    cmd.Parameters.AddWithValue("@Subject", subjectName);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
        }
        public int GetBatchID_FromBatchName(string batchName)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = "SELECT Batch_ID FROM Batch WHERE Batch_Name = @BatchName";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BatchName", batchName);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving Batch ID: " + ex.Message);
                return -1;
            }
        }

        public bool IsDuplicateExam(Exam exam)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                SELECT COUNT(*)
                FROM Exam
                WHERE Exam_type = @Type
                  AND Exam_Date = @Date
                  AND CS_ID = @CS_ID
                  AND Batch_ID = @Batch_ID";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Type", exam.Exam_type);
                        cmd.Parameters.AddWithValue("@Date", exam.Exam_Date);
                        cmd.Parameters.AddWithValue("@CS_ID", exam.CS_ID);
                        cmd.Parameters.AddWithValue("@Batch_ID", exam.Batch_ID);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking for duplicates: " + ex.Message);
                return true; // Assume duplicate on error, for safety
            }
        }

        public bool AddExam(Exam exam)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                INSERT INTO Exam (Exam_type, Exam_Date, Exam_Duration, CS_ID, Batch_ID,Exam_Status) 
                VALUES (@Type, @Date, @Duration, @CS_ID, @Batch_ID,@Exam_Status )";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Type", exam.Exam_type);
                        cmd.Parameters.AddWithValue("@Date", exam.Exam_Date);
                        cmd.Parameters.AddWithValue("@Duration", exam.Exam_Duration);
                        cmd.Parameters.AddWithValue("@CS_ID", exam.CS_ID);
                        cmd.Parameters.AddWithValue("@Batch_ID", exam.Batch_ID);
                        cmd.Parameters.AddWithValue("@Exam_Status", exam.Exam_Status);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding exam: " + ex.Message);
                return false;
            }
        }

        public DataTable GetAllExamsWithDetails(bool includeInactive = false)
        {
            var table = new DataTable();

            try
            {
                using (var conn = DB_Config.getConnection())
                {
                   
                    string query = @"
                SELECT 
                    e.Exam_ID,
                    e.Exam_type,
                    e.Exam_Date,
                    e.Exam_Duration,
                    c.Course_Name,
                    s.Subject_Name,
                    b.Batch_Name,
                    e.CS_ID,
                    e.Batch_ID,
                    e.Exam_Status
                FROM Exam e
                LEFT JOIN Course_Subject cs ON e.CS_ID = cs.CS_ID
                LEFT JOIN Course c ON cs.Course_ID = c.Course_ID
                LEFT JOIN Subject s ON cs.Subject_ID = s.Subject_ID
                LEFT JOIN Batch b ON e.Batch_ID = b.Batch_ID";

                    if (!includeInactive)
                        query += " WHERE e.Exam_Status = 'Active'";

                    using (var adapter = new SQLiteDataAdapter(query, conn))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading exams: " + ex.Message);
            }

            return table;
        }



        public Exam GetExamById(int examId)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                SELECT e.*, c.Course_Name, s.Subject_Name, b.Batch_Name
                FROM Exam e
                LEFT JOIN Course_Subject cs ON e.CS_ID = cs.CS_ID
                LEFT JOIN Course c ON cs.Course_ID = c.Course_ID
                LEFT JOIN Subject s ON cs.Subject_ID = s.Subject_ID
                LEFT JOIN Batch b ON e.Batch_ID = b.Batch_ID
                WHERE e.Exam_ID = @ExamID";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ExamID", examId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Exam
                                {
                                    Exam_ID = Convert.ToInt32(reader["Exam_ID"]),
                                    Exam_type = reader["Exam_type"].ToString(),
                                    Exam_Date = reader["Exam_Date"].ToString(),
                                    Exam_Duration = reader["Exam_Duration"].ToString(),
                                    CS_ID = Convert.ToInt32(reader["CS_ID"]),
                                    Batch_ID = Convert.ToInt32(reader["Batch_ID"]),
                                    Course_Name = reader["Course_Name"]?.ToString(),
                                    Subject_Name = reader["Subject_Name"]?.ToString(),
                                    Batch_Name = reader["Batch_Name"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving exam details: " + ex.Message);
            }

            return null;
        }

        public bool UpdateExam(Exam updatedExam)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                UPDATE Exam 
                SET Exam_type = @Type, 
                    Exam_Date = @Date, 
                    Exam_Duration = @Duration,
                    CS_ID = @CS_ID,
                    Batch_ID = @Batch_ID
                WHERE Exam_ID = @ExamID";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Type", updatedExam.Exam_type);
                        cmd.Parameters.AddWithValue("@Date", updatedExam.Exam_Date);
                        cmd.Parameters.AddWithValue("@Duration", updatedExam.Exam_Duration);
                        cmd.Parameters.AddWithValue("@CS_ID", updatedExam.CS_ID);
                        cmd.Parameters.AddWithValue("@Batch_ID", updatedExam.Batch_ID);
                        cmd.Parameters.AddWithValue("@ExamID", updatedExam.Exam_ID);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating exam: " + ex.Message);
                return false;
            }
        }

        public bool DeleteExamById(int examId)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = "UPDATE Exam SET Exam_Status = 'Inactive' WHERE Exam_ID = @ExamID";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ExamID", examId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deactivating exam: " + ex.Message);
                return false;
            }
        }


    }
}
