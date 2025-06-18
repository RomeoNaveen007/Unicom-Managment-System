using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicomTIC_Management_System.Data.DB_Connection;
using UnicomTIC_Management_System.Model;

namespace UnicomTIC_Management_System.Service
{
    internal class User_Service
    {
        public List<User> Show_All_Users()
        {
            List<User> users = new List<User>();

            using (var conn = DB_Config.getConnection())
            {
                string query = @"SELECT User_ID, User_Name, Password, Role FROM User;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                User_ID = reader.GetInt32(0),
                                User_Name = reader.GetString(1),
                                Password = reader.GetString(2),
                                Role = reader.GetString(3)
                            });
                        }
                    }
                }
            }

            return users;
        }

    }
}
