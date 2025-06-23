using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTIC_Management_System.Data.DB_Connection;

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


    }
}
