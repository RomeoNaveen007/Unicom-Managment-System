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
using UnicomTIC_Management_System.Service;

namespace UnicomTIC_Management_System.Service
{
    internal class Mark_Service
    {
       
       

        public List<string> GetCoursesFromCourseSubject()
        {
            var list = new List<string>();
            using (var conn = DB_Config.getConnection())
            {
                string query = "SELECT DISTINCT c.Course_Name FROM Course_Subject cs LEFT JOIN Course c ON cs.Course_ID = c.Course_ID";
                using (var cmd = new SQLiteCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(reader.GetString(0));
            }
            return list;
        }

        public List<string> GetSubjectsByCourse(string courseName)
        {
            var list = new List<string>();
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
            SELECT DISTINCT s.Subject_Name
            FROM Course_Subject cs
            LEFT JOIN Course c ON cs.Course_ID = c.Course_ID
            LEFT JOIN Subject s ON cs.Subject_ID = s.Subject_ID
            WHERE c.Course_Name = @CourseName";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseName", courseName);
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(reader.GetString(0));
                }
            }
            return list;
        }

        public List<string> GetExamTypes(string courseName, string subjectName)
        {
            var list = new List<string>();
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
            SELECT DISTINCT e.Exam_type
            FROM Exam e
            LEFT JOIN Course_Subject cs ON e.CS_ID = cs.CS_ID
            LEFT JOIN Course c ON cs.Course_ID = c.Course_ID
            LEFT JOIN Subject s ON cs.Subject_ID = s.Subject_ID
            WHERE c.Course_Name = @Course AND s.Subject_Name = @Subject AND e.Exam_Status = 'Active'";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Course", courseName);
                    cmd.Parameters.AddWithValue("@Subject", subjectName);
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            list.Add(reader.GetString(0));
                }
            }
            return list;
        }

        public List<string> GetStudentNames(string courseName, string subjectName, string examType)
        {
            var students = new List<string>();
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
            SELECT DISTINCT s.Student_Name
            FROM Student s
            INNER JOIN Exam e ON s.CS_ID = e.CS_ID AND s.Batch_ID = e.Batch_ID
            INNER JOIN Course_Subject cs ON e.CS_ID = cs.CS_ID
            INNER JOIN Course c ON cs.Course_ID = c.Course_ID
            INNER JOIN Subject sb ON cs.Subject_ID = sb.Subject_ID
            WHERE c.Course_Name = @Course
              AND sb.Subject_Name = @Subject
              AND e.Exam_type = @ExamType
              AND e.Exam_Status = 'Active'
              AND s.Student_Status = 'Active'";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Course", courseName);
                    cmd.Parameters.AddWithValue("@Subject", subjectName);
                    cmd.Parameters.AddWithValue("@ExamType", examType);
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            students.Add(reader.GetString(0));
                }
            }
            return students;
        }

        public int GetExamID(string course, string subject, string examType)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
            SELECT e.Exam_ID
            FROM Exam e
            LEFT JOIN Course_Subject cs ON e.CS_ID = cs.CS_ID
            LEFT JOIN Course c ON cs.Course_ID = c.Course_ID
            LEFT JOIN Subject s ON cs.Subject_ID = s.Subject_ID
            WHERE c.Course_Name = @Course AND s.Subject_Name = @Subject AND e.Exam_type = @Type";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Course", course);
                    cmd.Parameters.AddWithValue("@Subject", subject);
                    cmd.Parameters.AddWithValue("@Type", examType);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
        }

        public int GetStudentID(string studentName)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = "SELECT Student_ID FROM Student WHERE Student_Name = @Name AND Student_Status = 'Active'";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", studentName);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
        }

        public bool IsDuplicateMark(int examId, int studentId)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = "SELECT COUNT(*) FROM Marks WHERE Exam_ID = @ExamID AND Student_ID = @StudentID";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ExamID", examId);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }
        public bool AddMark(int examId, int studentId, int score)
        {
            if (IsDuplicateMark(examId, studentId))
            {
                MessageBox.Show("Mark already exists for this student and exam.");
                return false;
            }

            using (var conn = DB_Config.getConnection())
            {
                string query = "INSERT INTO Marks (Exam_ID, Student_ID, Score) VALUES (@ExamID, @StudentID, @Score)";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ExamID", examId);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@Score", score);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public DataTable GetAllMarks()
        {
            var table = new DataTable();
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
            SELECT 
                m.Exam_ID, e.Exam_type,
                c.Course_Name, s.Subject_Name,
                st.Student_Name, m.Score
            FROM Marks m
            LEFT JOIN Exam e ON m.Exam_ID = e.Exam_ID
            LEFT JOIN Course_Subject cs ON e.CS_ID = cs.CS_ID
            LEFT JOIN Course c ON cs.Course_ID = c.Course_ID
            LEFT JOIN Subject s ON cs.Subject_ID = s.Subject_ID
            LEFT JOIN Student st ON m.Student_ID = st.Student_ID";

                using (var adapter = new SQLiteDataAdapter(query, conn))
                {
                    adapter.Fill(table);
                }
            }
            return table;
        }

        public bool UpdateMark(int examId, int studentId, int newScore)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = "UPDATE Marks SET Score = @Score WHERE Exam_ID = @ExamID AND Student_ID = @StudentID";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Score", newScore);
                    cmd.Parameters.AddWithValue("@ExamID", examId);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteMark(int examId, int studentId)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = "DELETE FROM Marks WHERE Exam_ID = @ExamID AND Student_ID = @StudentID";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ExamID", examId);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

    }
}
