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
    internal class Batch_Service
    {

        public void AddBatch(Batch batch)
        {
            using (var conn = DB_Config.getConnection())
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = @"
                    INSERT INTO Batch (Batch_Name, Year, Batch_Status)
                    VALUES (@Batch_Name, @Year, @Batch_Status);";

                        using (var cmd = new SQLiteCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Batch_Name", batch.Batch_Name);
                            cmd.Parameters.AddWithValue("@Year", batch.Year);
                            cmd.Parameters.AddWithValue("@Batch_Status", "Active"); // default status

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show($"{batch.Batch_Name} added successfully.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error adding batch: " + ex.Message);
                    }
                }
            }
        }

        public List<Batch> GetAllBatches()
        {
            List<Batch> batchList = new List<Batch>();

            using (var conn = DB_Config.getConnection())
            {
                string query = @"SELECT Batch_ID, Batch_Name, Year, Batch_Status FROM Batch;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Batch batch = new Batch
                            {
                                Batch_ID = reader.GetInt32(0),
                                Batch_Name = reader.GetString(1),
                                Year = reader.GetInt32(2),
                                Batch_Status = reader.IsDBNull(3) ? null : reader.GetString(3)
                            };

                            batchList.Add(batch);
                        }
                    }
                }
            }

            return batchList;
        }

        public Batch GetBatchById(int id)
        {
            using (var conn = DB_Config.getConnection())
            {
                string query = @"
            SELECT 
                Batch_ID, 
                Batch_Name, 
                Year,
                Batch_Status
            FROM Batch
            WHERE Batch_ID = @Id;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Batch
                            {
                                Batch_ID = reader.GetInt32(0),
                                Batch_Name = reader.GetString(1),
                                Year = reader.GetInt32(2),
                                Batch_Status = reader.IsDBNull(3) ? null : reader.GetString(3)
                            };
                        }
                    }
                }
            }

            return null; // Return null if not found
        }

        public void UpdateBatch(Batch batch)
        {
            using (var conn = DB_Config.getConnection())
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = @"
                    UPDATE Batch
                    SET Batch_Name = @Batch_Name,
                        Year = @Year
                    WHERE Batch_ID = @Batch_ID;";

                        using (var cmd = new SQLiteCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Batch_Name", batch.Batch_Name);
                            cmd.Parameters.AddWithValue("@Year", batch.Year);
                            cmd.Parameters.AddWithValue("@Batch_ID", batch.Batch_ID);

                            int rows = cmd.ExecuteNonQuery();

                            if (rows > 0)
                            {
                                MessageBox.Show($"{batch.Batch_Name} updated successfully.");
                            }
                            else
                            {
                                MessageBox.Show($"Batch with ID {batch.Batch_ID} not found.");
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error updating batch: " + ex.Message);
                    }
                }
            }
        }

        public void Delete_Batch(Batch del_batch)
        {
            using (var conn = DB_Config.getConnection())
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = @"
                    UPDATE Batch 
                    SET Batch_Status = @Batch_Status 
                    WHERE Batch_ID = @Batch_ID;";

                        using (var cmd = new SQLiteCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Batch_Status", del_batch.Batch_Status);
                            cmd.Parameters.AddWithValue("@Batch_ID", del_batch.Batch_ID);

                            int rows = cmd.ExecuteNonQuery();

                            if (rows > 0)
                            {
                                MessageBox.Show($"Batch '{del_batch.Batch_Name}' is marked as deleted.");
                            }
                            else
                            {
                                MessageBox.Show($"Batch with ID {del_batch.Batch_ID} not found.");
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error deleting batch: " + ex.Message);
                    }
                }
            }
        }


    }
}
