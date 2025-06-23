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
    internal class Room_Service
    {
        public bool AddRoom(Room room)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string checkQuery = @"
                SELECT COUNT(*) 
                FROM Room 
                WHERE Room_Name = @Name AND Room_Type = @Type;";

                    using (SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Name", room.Room_Name);
                        checkCmd.Parameters.AddWithValue("@Type", room.Room_Type);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show($"Room '{room.Room_Name}' ({room.Room_Type}) already exists.");
                            return false;
                        }
                    }

                    // Step 2: Insert new room if it doesn’t exist
                    string insertQuery = "INSERT INTO Room (Room_Name, Room_Type) VALUES (@Name, @Type);";

                    using (SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@Name", room.Room_Name);
                        insertCmd.Parameters.AddWithValue("@Type", room.Room_Type);

                        int result = insertCmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add room: " + ex.Message);
                return false;
            }
        }

        public List<Room> GetAllRooms()
        {
            List<Room> roomList = new List<Room>();

            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = "SELECT Room_ID, Room_Name, Room_Type FROM Room;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Room room = new Room
                            {
                                Room_ID = reader.GetInt32(0),
                                Room_Name = reader.GetString(1),
                                Room_Type = reader.GetString(2)
                            };

                            roomList.Add(room);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving rooms: " + ex.Message);
            }

            return roomList;
        }

        public Room GetRoomById(int roomId)
        {
            Room room = null;

            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = "SELECT Room_ID, Room_Name, Room_Type FROM Room WHERE Room_ID = @Id;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", roomId);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                room = new Room
                                {
                                    Room_ID = reader.GetInt32(0),
                                    Room_Name = reader.GetString(1),
                                    Room_Type = reader.GetString(2)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving room details: " + ex.Message);
            }

            return room;
        }

        public bool UpdateRoom(Room room)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = "UPDATE Room SET Room_Name = @Name, Room_Type = @Type WHERE Room_ID = @Id;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", room.Room_Name);
                        cmd.Parameters.AddWithValue("@Type", room.Room_Type);
                        cmd.Parameters.AddWithValue("@Id", room.Room_ID);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message);
                return false;
            }
        }

        public bool DeleteRoom(int roomId)
        {
            try
            {
                using (var conn = DB_Config.getConnection())
                {
                    string query = "DELETE FROM Room WHERE Room_ID = @Id;";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", roomId);
                        return cmd.ExecuteNonQuery() > 0;
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
