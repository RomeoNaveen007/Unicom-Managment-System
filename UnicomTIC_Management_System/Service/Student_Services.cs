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
    internal class Student_Services
    {
        public List<string> GetActiveCourseNames()
        {
            List<string> courseNames = new List<string>();

            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                SELECT DISTINCT COALESCE(c.Course_Name, 'Unknown Course') 
                FROM Course_Subject cs
                LEFT JOIN Course c ON cs.Course_ID = c.Course_ID";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                                courseNames.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading course names: " + ex.Message);
            }

            return courseNames;
        }

        public List<string> GetActiveBatchNames()
        {
            var batchNames = new List<string>();

            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = "SELECT Batch_Name FROM Batch WHERE Batch_Status = 'Active'";

                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                batchNames.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading active batch names: " + ex.Message);
            }

            return batchNames;
        }

        public int GetCS_ID_FromCourseName(string courseName)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                SELECT cs.CS_ID
                FROM Course_Subject cs
                LEFT JOIN Course c ON cs.Course_ID = c.Course_ID
                WHERE c.Course_Name = @CourseName";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CourseName", courseName);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching CS_ID: " + ex.Message);
                return -1;
            }
        }

        public string GetBatchID_FromBatchName(string batchName)
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
                        return result != null ? result.ToString() : null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching Batch ID: " + ex.Message);
                return null;
            }
        }

        public bool AddStudentWithUser(Student student, User user)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    
                    using (var transaction = conn.BeginTransaction())
                    {
                        // ..................................... Add user
                        string userQuery = "INSERT INTO [User] (User_Name, Password, Role) VALUES (@UserName, @Password, @Role); SELECT last_insert_rowid();";
                        using (var userCmd = new SQLiteCommand(userQuery, conn))
                        {
                            userCmd.Parameters.AddWithValue("@UserName", user.User_Name);
                            userCmd.Parameters.AddWithValue("@Password", user.Password);
                            userCmd.Parameters.AddWithValue("@Role", user.Role);
                            object result = userCmd.ExecuteScalar();
                            if (result == null)
                                throw new Exception("User insert failed.");

                            user.User_ID = Convert.ToInt32(result);
                            student.User_ID = user.User_ID;
                        }

                        // .......................................Add student
                        string studentQuery = @"
                    INSERT INTO Student (Student_Name, Student_Address, Student_NIC, Student_Status, CS_ID, Batch_ID, User_ID)
                    VALUES (@Name, @Address, @NIC, @Status, @CS_ID, @Batch_ID, @User_ID)";

                        using (var studentCmd = new SQLiteCommand(studentQuery, conn))
                        {
                            studentCmd.Parameters.AddWithValue("@Name", student.Student_Name);
                            studentCmd.Parameters.AddWithValue("@Address", student.Student_Address);
                            studentCmd.Parameters.AddWithValue("@NIC", student.Student_NIC);
                            studentCmd.Parameters.AddWithValue("@Status", student.Student_Status);
                            studentCmd.Parameters.AddWithValue("@CS_ID", student.CS_ID);
                            studentCmd.Parameters.AddWithValue("@Batch_ID", student.Batch_ID);
                            studentCmd.Parameters.AddWithValue("@User_ID", student.User_ID);
                            studentCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add student with user: " + ex.Message);
                return false;
            }
        }

        public DataTable GetAllStudentsWithNames()
        {
            var table = new DataTable();

            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                SELECT s.Student_ID, s.Student_Name, s.Student_Address, s.Student_NIC, 
                       s.Student_Status, b.Batch_Name, c.Course_Name
                FROM Student s
                LEFT JOIN Batch b ON s.Batch_ID = b.Batch_ID
                LEFT JOIN Course_Subject cs ON s.CS_ID = cs.CS_ID
                LEFT JOIN Course c ON cs.Course_ID = c.Course_ID";

                    using (var adapter = new SQLiteDataAdapter(query, conn))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving student data: " + ex.Message);
            }

            return table;
        }

        public Student GetStudentById(int id)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                SELECT s.*, u.User_Name, c.Course_Name,b.Batch_Name
                FROM Student s
                LEFT JOIN [User] u ON s.User_ID = u.User_ID
                LEFT JOIN Course_Subject cs ON s.CS_ID = cs.CS_ID
                LEFT JOIN Course c ON cs.Course_ID = c.Course_ID
                LEFT JOIN Batch b ON s.Batch_ID = b.Batch_ID
                WHERE s.Student_ID = @ID";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Student
                                {
                                    Student_ID = reader.GetInt32(reader.GetOrdinal("Student_ID")),
                                    Student_Name = reader["Student_Name"].ToString(),
                                    Student_Address = reader["Student_Address"].ToString(),
                                    Student_NIC = reader["Student_NIC"].ToString(),
                                    Student_Status = reader["Student_Status"].ToString(),
                                    Batch_ID = reader["Batch_ID"].ToString(),
                                    CS_ID = Convert.ToInt32(reader["CS_ID"]),
                                    User_ID = Convert.ToInt32(reader["User_ID"]),
                                   
                                    User_Name = reader["User_Name"]?.ToString(),
                                    Course_Name = reader["Course_Name"]?.ToString(),
                                    Batch_Name = reader["Batch_Name"]?.ToString() 
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving student: " + ex.Message);
            }

            return null;
        }
        public int GetUserIDByStudentID(int studentId)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = "SELECT User_ID FROM Student WHERE Student_ID = @StudentId";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving User ID: " + ex.Message);
                return -1;
            }
        }

        public bool UpdateStudentDetails(Student student)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                UPDATE Student
                SET Student_Name = @Name,
                    Student_Address = @Address,
                    Student_NIC = @NIC,
                    Student_Status = @Status,
                    CS_ID = @CS_ID,
                    Batch_ID = @Batch_ID
                WHERE Student_ID = @StudentID";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", student.Student_Name);
                        cmd.Parameters.AddWithValue("@Address", student.Student_Address);
                        cmd.Parameters.AddWithValue("@NIC", student.Student_NIC);
                        cmd.Parameters.AddWithValue("@Status", student.Student_Status);
                        cmd.Parameters.AddWithValue("@CS_ID", student.CS_ID);
                        cmd.Parameters.AddWithValue("@Batch_ID", student.Batch_ID);
                        cmd.Parameters.AddWithValue("@StudentID", student.Student_ID);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating student details: " + ex.Message);
                return false;
            }
        }

        public bool DeactivateStudentById(int studentId)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = "UPDATE Student SET Student_Status = 'Inactive' WHERE Student_ID = @StudentID";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", studentId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deactivating student: " + ex.Message);
                return false;
            }
        }

    }
}
