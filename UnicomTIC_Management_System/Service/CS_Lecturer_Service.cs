using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Data.DB_Connection;

namespace UnicomTIC_Management_System.Service
{
    internal class CS_Lecturer_Service
    {
        public Dictionary<int, string> GetActiveLecturers()
        {
            Dictionary<int, string> lecturerMap = new Dictionary<int, string>();

            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = "SELECT Lecturer_ID, Lecturer_Name FROM Lecturer WHERE Lecturer_Status = 'Active';";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            lecturerMap[id] = name;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading lecturers: " + ex.Message);
            }

            return lecturerMap;
        }


        public List<string> GetCoursesByLecturer(int lecturerId)
        {
            List<string> courses = new List<string>();

            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                SELECT DISTINCT c.Course_Name
                FROM Lecturer l
                JOIN Subject s ON l.Special_In = s.Subject_Name
                JOIN Course_Subject cs ON s.Subject_ID = cs.Subject_ID
                JOIN Course c ON cs.Course_ID = c.Course_ID
                WHERE l.Lecturer_ID = @LecturerID;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@LecturerID", lecturerId);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                courses.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving course names: " + ex.Message);
            }

            return courses;
        }

        public int GetCS_ID_FromCourseName(string courseName)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                SELECT cs.CS_ID
                FROM Course c
                JOIN Course_Subject cs ON c.Course_ID = cs.Course_ID
                WHERE c.Course_Name = @CourseName;";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CourseName", courseName);
                        var result = cmd.ExecuteScalar();
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

        public int GetLecturerID_FromName(string lecturerName)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = "SELECT Lecturer_ID FROM Lecturer WHERE Lecturer_Name = @Name";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", lecturerName);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching Lecturer ID: " + ex.Message);
                return -1;
            }
        }



        public bool AddMapping(int lecturerId, int csId)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    // ......................Duplicate check
                    string checkQuery = "SELECT COUNT(*) FROM CS_Lecturer WHERE Lecturer_ID = @LecturerID AND CS_ID = @CSID";
                    using (var checkCmd = new SQLiteCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@LecturerID", lecturerId);
                        checkCmd.Parameters.AddWithValue("@CSID", csId);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("This lecturer is already assigned to the selected course.");
                            return false;
                        }
                    }

                    string insertQuery = "INSERT INTO CS_Lecturer (Lecturer_ID, CS_ID) VALUES (@LecturerID, @CSID)";
                    using (var cmd = new SQLiteCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@LecturerID", lecturerId);
                        cmd.Parameters.AddWithValue("@CSID", csId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while adding mapping: " + ex.Message);
                return false;
            }
        }
        //

       
        public bool UpdateMapping(int oldLecturerId, int oldCSId, int newLecturerId, int newCSId)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string updateQuery = @"
                UPDATE CS_Lecturer 
                SET Lecturer_ID = @NewLecturerID, CS_ID = @NewCSID
                WHERE Lecturer_ID = @OldLecturerID AND CS_ID = @OldCSID";

                    using (var cmd = new SQLiteCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewLecturerID", newLecturerId);
                        cmd.Parameters.AddWithValue("@NewCSID", newCSId);
                        cmd.Parameters.AddWithValue("@OldLecturerID", oldLecturerId);
                        cmd.Parameters.AddWithValue("@OldCSID", oldCSId);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating assignment: " + ex.Message);
                return false;
            }
        }

        public bool DeleteMapping(int lecturerId, int csId)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string deleteQuery = "DELETE FROM CS_Lecturer WHERE Lecturer_ID = @LecturerID AND CS_ID = @CSID";
                    using (var cmd = new SQLiteCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@LecturerID", lecturerId);
                        cmd.Parameters.AddWithValue("@CSID", csId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting assignment: " + ex.Message);
                return false;
            }
        }

        public DataTable GetAllMappingsWithNames()
        {
            DataTable table = new DataTable();

            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = @"
                SELECT 
                    COALESCE(l.Lecturer_Name, 'Unknown Lecturer') AS Lecturer,
                    COALESCE(c.Course_Name, 'Unknown Course') AS Course
                FROM CS_Lecturer cl
                LEFT JOIN Lecturer l ON cl.Lecturer_ID = l.Lecturer_ID
                LEFT JOIN Course_Subject cs ON cl.CS_ID = cs.CS_ID
                LEFT JOIN Course c ON cs.Course_ID = c.Course_ID;";

                    using (var adapter = new SQLiteDataAdapter(query, conn))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading lecturer-course mappings: " + ex.Message);
            }

            return table;
        }

    }
}
